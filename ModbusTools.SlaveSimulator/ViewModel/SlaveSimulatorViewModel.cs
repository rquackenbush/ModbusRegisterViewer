using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveSimulator.Model;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveSimulatorViewModel : ViewModelBase
    {
        public event EventHandler<SlaveEvent> SlaveCreated;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly ObservableCollection<SlaveViewModel> _slaves = new ObservableCollection<SlaveViewModel>();

        private SlaveSimulatorHost _simulator;
        
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
            return _simulator == null;
        }

        private void RaiseSlaveCreated(SlaveViewModel slave)
        {
            var handler = SlaveCreated;

            if (handler == null)
                return;

            handler(this, new SlaveEvent(slave));
        }

        private void Start()
        {
            var factory = _modbusAdapters.GetFactory();

            var holdingRegisters = new SparseRegisterStorage();

            _simulator = new SlaveSimulatorHost(factory.Create(), _slaves.Select(s => s.GetSlave())
            , 1.5, 4.0);
        }

        public bool CanCloseSlave()
        {
            return _simulator == null;
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
            if (_simulator != null)
                return false;

            if (!_modbusAdapters.IsItemSelected)
                return false;

            if (Slaves.Count == 0)
                return false;

            return true;
        }

        private void Stop()
        {
            if (_simulator != null)
            {
                _simulator.Dispose();

                _simulator = null;
            }
        }

        private bool CanStop()
        {
            return _simulator != null;
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
