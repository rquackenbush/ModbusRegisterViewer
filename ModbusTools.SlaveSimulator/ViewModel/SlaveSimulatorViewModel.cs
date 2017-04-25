using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using NModbus;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveSimulatorViewModel : ViewModelBase
    {
        public event EventHandler<SlaveEvent> SlaveCreated;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly ObservableCollection<SlaveViewModel> _slaves = new ObservableCollection<SlaveViewModel>();

        private IModbusSlaveNetwork _slaveNetwork;

        private Task _listenTask;
        
        public SlaveSimulatorViewModel()
        {
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            AddSlaveCommand = new RelayCommand(AddSlave, CanAddSlave);

            AddSlave();
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand AddSlaveCommand { get; private set; }

        private void AddSlave()
        {
            byte slaveId = 1;

            //Check to see if we already have a slave
            if (_slaves.Any())
            {
                var max = _slaves.Max(s => s.SlaveId);

                slaveId = (byte)(max + 1);
            }

            //Create the new slave
            var slave = new SlaveViewModel()
            {
                SlaveId = slaveId
            };

            //Let the world know that we have a new slave! Wait...
            RaiseSlaveCreated(slave);

            //Add it
            _slaves.Add(slave);
        }

        private bool CanAddSlave()
        {
            //We can only add a slave when we're NOT running
            return _slaveNetwork == null;
        }

        private void RaiseSlaveCreated(SlaveViewModel slave)
        {
            SlaveCreated?.Invoke(this, new SlaveEvent(slave));
        }

        private void Start()
        {
            IMasterContextFactory factory = _modbusAdapters.GetFactory();

            _slaveNetwork = factory.CreateSlaveNetwork();

            foreach (var slave in _slaves)
            {
                _slaveNetwork.AddSlave(slave.CreateModbusSlave());
            }

            _listenTask = Task.Factory.StartNew(async () => 
            {
                while (_slaveNetwork != null)
                {
                    try
                    {
                        var slaveNetwork = _slaveNetwork;

                        if (slaveNetwork != null)
                        {
                            await slaveNetwork.ListenAsync();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        //TODO: Log this

                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public bool CanCloseSlave()
        {
            return _slaveNetwork == null;
        }

        public void OnSlaveClosed(SlaveViewModel slave)
        {
            if (Slaves.Contains(slave))
            {
                Slaves.Remove(slave);
            }
        }

        private bool CanStart()
        {
            if (_slaveNetwork != null)
                return false;

            if (!_modbusAdapters.IsItemSelected)
                return false;

            if (Slaves.Count == 0)
                return false;

            return true;
        }

        private void Stop()
        {
            if (_slaveNetwork != null)
            {
                _slaveNetwork.Dispose();

                _slaveNetwork = null;
            }

            _listenTask = null;
        }

        private bool CanStop()
        {
            return _slaveNetwork != null;
        }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public ObservableCollection<SlaveViewModel> Slaves
        {
            get { return _slaves; }
        }
    }
}
