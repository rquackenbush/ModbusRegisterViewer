using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveSimulator.Model;
using ModbusTools.SlaveSimulator.Model.FunctionHandlers;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveSimulatorViewModel : ViewModelBase
    {
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private SlaveSimulatorHost _simulator;

        public SlaveSimulatorViewModel()
        {
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        private void Start()
        {
            var factory = _modbusAdapters.GetFactory();

            var holdingRegisters = new SparseRegisterStorage();

            _simulator = new SlaveSimulatorHost(factory.Create(), new Slave[]
            {
                new Slave(1, new IModbusFunctionHandler[]
                {
                    new ReadHoldingRegistersFunctionHandler(holdingRegisters),                     
                    new WriteHoldingRegistersFunctionHandler(holdingRegisters), 
                }), 
            }, 2.5, 4.0);
        }

        private bool CanStart()
        {
            if (_simulator != null)
                return false;

            if (!_modbusAdapters.IsItemSelected)
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
    }
}
