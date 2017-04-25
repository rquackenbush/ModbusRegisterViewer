using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using ModbusTools.SlaveSimulator.Model;
using NModbus;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        private readonly SlaveStorage _slaveStorage = new SlaveStorage();

        private readonly ObservableCollection<SlaveRegisterViewModel> _holdingRegisters = new ObservableCollection<SlaveRegisterViewModel>();
        private readonly ObservableCollection<SlaveRegisterViewModel> _inputRegisters = new ObservableCollection<SlaveRegisterViewModel>();

        public readonly ObservableCollection<ActivityViewModel> _activities = new ObservableCollection<ActivityViewModel>();

        public SlaveViewModel()
        {
            _slaveStorage.HoldingRegisters.StorageOperationOccurred += HoldingRegisterOperation;
            _slaveStorage.InputRegisters.StorageOperationOccurred += InputRegisterOperation;
            _slaveStorage.CoilDiscretes.StorageOperationOccurred += CoilDiscreteOperation;
            _slaveStorage.CoilInputs.StorageOperationOccurred += CoilInputOperation;

            for (ushort registerIndex = 0; registerIndex < ushort.MaxValue; registerIndex++)
            {
                _holdingRegisters.Add(new SlaveRegisterViewModel(_slaveStorage.HoldingRegisters, registerIndex) { IsZeroBased = true });
            }

            for (ushort registerIndex = 0; registerIndex < ushort.MaxValue; registerIndex++)
            {
                _inputRegisters.Add(new SlaveRegisterViewModel(_slaveStorage.InputRegisters, registerIndex) { IsZeroBased = true });
            }
        }

        private void AddActivity(string pointType, PointOperation operation, ushort startingAddress, ushort[] values)
        {
            var hexNumbers = values
                .Select(r => Convert.ToString(r, 16).PadLeft(4, '0'))
                .ToArray();

            string formattedValue = string.Join(" ", hexNumbers);

            AddActivity($"{operation} {pointType}", startingAddress, formattedValue);
        }

        private void AddActivity(string pointType, PointOperation operation, ushort startingAddress, bool[] values)
        {
            var individualValues = values
                .Select(r => r ? "1" : "0")
                .ToArray();

            string formattedValue = string.Join(" ", individualValues);

            AddActivity($"{operation} {pointType}", startingAddress, formattedValue);
        }

        private void AddActivity(string operation, ushort startingAddress, string values)
        {
            var activity = new ActivityViewModel(DateTime.Now, operation, startingAddress, values);

            DispatcherHelper.CheckBeginInvokeOnUI(() => Activities.Add(activity));
        }

        void HoldingRegisterOperation(object sender, StorageEventArgs<ushort> e)
        {
            AddActivity("Holding Register", e.Operation, e.StartingAddress, e.Points);
        }

        void InputRegisterOperation(object sender, StorageEventArgs<ushort> e)
        {
            AddActivity("Input Register", e.Operation, e.StartingAddress, e.Points);
        }

        void CoilDiscreteOperation(object sender, StorageEventArgs<bool> e)
        {
            AddActivity("Discrete Coil", e.Operation, e.StartingAddress, e.Points);
        }

        void CoilInputOperation(object sender, StorageEventArgs<bool> e)
        {
            AddActivity("Discrete Input", e.Operation, e.StartingAddress, e.Points);
        }

        public ObservableCollection<ActivityViewModel> Activities
        {
            get { return _activities; }
        }

        public byte SlaveIdMin
        {
            get { return 1; }
        }

        public byte SlaveIdMax
        {
            get { return 247; }
        }

        private byte _slaveId;
        public byte SlaveId
        {
            get { return _slaveId; }
            set
            {
                _slaveId = value; 
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<SlaveRegisterViewModel> HoldingRegisters
        {
            get { return _holdingRegisters; }
        }

        public ObservableCollection<SlaveRegisterViewModel> InputRegisters
        {
            get { return _inputRegisters; }
        }

        public IModbusSlave CreateModbusSlave()
        {
            var factory = new ModbusFactory();

            //Attach our custom storage
            return factory.CreateSlave(SlaveId, _slaveStorage);
        }
    }
}
