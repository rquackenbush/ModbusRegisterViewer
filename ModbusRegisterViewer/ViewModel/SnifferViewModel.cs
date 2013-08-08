using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FtdAdapter;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using Modbus.Device;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    public class SnifferViewModel : ViewModelBase
    {
        private const string Filter = "Xml Files(*.xml)|*.xml";

        private Task _task;
        private FtdUsbPort _port;
        //private ModbusPromiscuousSerialSlave _slave;
        private ModbusPromiscuousSerialSlave _slave;
        private ObservableCollection<PacketViewModel> _packets = new ObservableCollection<PacketViewModel>();

        public event EventHandler SelectionChanged;

        private PacketViewModel _selectedPacket;

        private readonly PacketSnifferStateMachine _stateMachine = new PacketSnifferStateMachine();

        public SnifferViewModel()
        {
            if (this.IsInDesignMode)
            {
                //Populate some dummy data
                _packets.Add(PacketViewModel.CreateValidPacket(DateTime.Now.Subtract(TimeSpan.FromSeconds(5)), new byte[] { 55, 16, 1, 0, 6, 7 }, MessageDirection.Request, null ));
                _packets.Add(PacketViewModel.CreateValidPacket(DateTime.Now.Subtract(TimeSpan.FromSeconds(5)), new byte[] { 55, 16, 4, 5, 6, 7, 1, 0 }, MessageDirection.Response, _packets[0]));
            }

            this.StartCommand = new RelayCommand(Start, CanStart);
            this.StopCommand = new RelayCommand(Stop, CanStop);
            this.ClearCommand = new RelayCommand(Clear, CanClear);
            this.OpenCommand = new RelayCommand(Open, CanOpen);
            this.SaveCommand = new RelayCommand(Save, CanSave);
            this.CloseCommand = new RelayCommand(Close, CanClose);
            
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private void Open()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() != true)
                return;

            var capture =  DataContractUtilities.FromFile<SnifferCapture>(dialog.FileName);

            this.Clear();


            this.Packets = new ObservableCollection<PacketViewModel>(capture.Packets.Select(p => p.FromModel()));
            
        }

        private bool CanOpen()
        {
            return _task == null;
        }

        private void Save()
        {
            var dialog = new SaveFileDialog()
                {
                    Filter = Filter
                };

            if (dialog.ShowDialog() != true)
                return;

            var capture = this.Packets.ToModel();

            DataContractUtilities.ToFile(dialog.FileName, capture);
        }

        private bool CanSave()
        {
            return _task == null;
        }

        private void Close()
        {
            
        }

        private bool CanClose()
        {
            return _task == null;
        }

        private void Start()
        {
            _port = new FtdUsbPort();

            // configure serial port
            _port.BaudRate = 19200;
            _port.DataBits = 8;
            _port.Parity = FtdParity.Even;
            _port.StopBits = FtdStopBits.One;
            _port.OpenByIndex(0);

            _port.ReadTimeout = 3000;

            _slave = ModbusPromiscuousSerialSlave.CreateRtu(_port);

            //When a message is received
            _slave.MessageSniffed += OnMessageSniffed;

            //When an invalid message is detected.
            _slave.InvalidMessage += OnInvalidMessage;

            //Spin up the listener in its own thread
            _task = new Task(() =>
            {
                try
                {
                    _slave.Listen();
                }
                catch (Exception ex)
                {
                    //This will exception out when the port is killed
                    Console.WriteLine(ex.ToString());
                }
                finally
                {

                    _task = null;

                }
            });

            _task.Start();
        }

        private bool CanStart()
        {
            return _task == null;
        }

        private void Stop()
        {
            try
            {
                if (_slave != null)
                {
                    //When a message is received
                    _slave.MessageSniffed -= OnMessageSniffed;
                    _slave.InvalidMessage -= OnInvalidMessage;

                    _slave.Dispose();
                    _slave = null;
                }

                if (_port != null)
                {
                    if (_port.IsOpen)
                    {
                        _port.DiscardInBuffer();

                        _port.Close();
                    }

                    _port.Dispose();
                    _port = null;
                }

                Console.WriteLine("Close Completed");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private bool CanStop()
        {
            return _task != null;
        }

        private void Clear()
        {
            //Ditch all of these.
            this.Packets.Clear();
        }

        private bool CanClear()
        {
            return true;
        }

        protected void RaiseSelectionChanged()
        {
            var handler = this.SelectionChanged;

            if (handler == null)
                return;

            handler(this, EventArgs.Empty);
        }

        public PacketViewModel SelectedPacket
        {
            get { return _selectedPacket; }
            set
            {
                if (_selectedPacket != value)
                {
                    _selectedPacket = value;
                    RaisePropertyChanged(() => SelectedPacket);

                    RaiseSelectionChanged();
                }
            }
        }

        private void OnInvalidMessage(object sender, InvalidData e)
        {
            var packetViewModel = PacketViewModel.CreateInvalidPacket(DateTime.Now, e.Message, e.Reason);

            DispatcherHelper.CheckBeginInvokeOnUI(() => _packets.Add(packetViewModel));
        }

        private void OnMessageSniffed(object sender, RawSlaveData e)
        {
            var packetViewModel = _stateMachine.ProcessMessage(e.Message, DateTime.Now);
            
            DispatcherHelper.CheckBeginInvokeOnUI(() => _packets.Add(packetViewModel));
        }

        public ObservableCollection<PacketViewModel> Packets
        {
            get { return _packets; }
            private set
            {
                if (_packets != value)
                {
                    _packets = value;
                    RaisePropertyChanged(() => Packets);
                }
            }
        }
    }
}
