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
using Unme.Common;

namespace ModbusRegisterViewer.ViewModel
{
    public class SnifferViewModel : ViewModelBase
    {
        private const string Filter = "Modbus Capture Files(*.mbcap)|*.mbcap";

        public event EventHandler SelectionChanged;

        private Task _task;
        private FtdUsbPort _port;
        private ObservableCollection<PacketViewModel> _packets = new ObservableCollection<PacketViewModel>();

        private PromiscuousListener _listener;
        private PacketViewModel _selectedPacket;

        private string _capturePath;
        private CaptureFileWriter _writer;

        public SnifferViewModel()
        {
            if (this.IsInDesignMode)
            {
                //Populate some dummy data
                //_packets.Add(PacketViewModel.CreateValidPacket(4000, new byte[] { 55, 16, 1, 0, 6, 7 }, MessageDirection.Request, null ));
                //_packets.Add(PacketViewModel.CreateValidPacket(4003, new byte[] { 55, 16, 4, 5, 6, 7, 1, 0 }, MessageDirection.Response, _packets[0]));
            }

            this.StartCommand = new RelayCommand(Start, CanStart);
            this.StopCommand = new RelayCommand(Stop, CanStop);
            this.ClearCommand = new RelayCommand(Clear, CanClear);
            this.OpenCommand = new RelayCommand(Open, CanOpen);
            this.CloseCommand = new RelayCommand(Close, CanClose);
            
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private void Open(string path)
        {
            
            var packets = new ObservableCollection<PacketViewModel>();

            using (var reader = new CaptureFileReader(path))
            {
                var stateMachine = new PacketSnifferStateMachine(20, reader.TicksPerMillisecond);

                var sample = reader.Read();

                while (sample != null)
                {
                    var packet = stateMachine.ProcessSample(sample);

                    if (packet != null)
                    {
                        packets.Add(packet);
                    }
                    
                    sample = reader.Read();
                }
            }

            this.Packets = packets;

            this.SelectedPacket = this.Packets.FirstOrDefault();
        }

        private void Open()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() != true)
                return;

            Open(dialog.FileName);
        }

        private bool CanOpen()
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
            var dialog = new SaveFileDialog()
                {
                    Filter = Filter
                };

            if (dialog.ShowDialog() != true)
                return;

            //Save the filename
            _capturePath = dialog.FileName;

            //Try to create the file first
            _writer = new CaptureFileWriter(dialog.FileName, TimeSpan.TicksPerMillisecond);

            _port = new FtdUsbPort();

            // configure serial port
            _port.BaudRate = 19200;
            _port.DataBits = 8;
            _port.Parity = FtdParity.Even;
            _port.StopBits = FtdStopBits.One;
            _port.OpenByIndex(0);

            _port.ReadTimeout = 3000;

            _listener = new PromiscuousListener(_port);

            _listener.Sample += OnSample;

            //Spin up the listener in its own thread
            _task = new Task(() =>
            {
                try
                {
                    _listener.Listen();
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
                if (_writer != null)
                {
                    //Console.WriteLine("Samples: {0}", _writer.SampleCount);
                }

                if (_listener != null)
                {
                    //When a message is received
                    _listener.Sample -= OnSample;

                    DisposableUtility.Dispose(ref _listener);
                }

                if (_port != null)
                {
                    if (_port.IsOpen)
                    {
                        _port.DiscardInBuffer();
                    }

                    DisposableUtility.Dispose(ref _port);
                }

                //Ditch the writer
                DisposableUtility.Dispose(ref _writer);

                //Console.WriteLine("Close Completed");
                
                if (!string.IsNullOrWhiteSpace(_capturePath))
                {
                    Open(_capturePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void OnSample(object sender, SampleEventArgs e)
        {
            if (_writer != null)
            {
                _writer.WriteSample(e.Sample);
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
