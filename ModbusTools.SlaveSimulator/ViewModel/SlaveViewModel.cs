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
        private byte _slaveId;

        private readonly SlaveStorage _slaveStorage = new SlaveStorage();

        private readonly ObservableCollection<SlaveRegisterViewModel> _holdingRegisters = new ObservableCollection<SlaveRegisterViewModel>();
        private readonly ObservableCollection<SlaveRegisterViewModel> _inputRegisters = new ObservableCollection<SlaveRegisterViewModel>();
        private readonly ObservableCollection<CoilViewModel> _coilDiscretes = new ObservableCollection<CoilViewModel>();
        private readonly ObservableCollection<CoilViewModel> _coilInputs = new ObservableCollection<CoilViewModel>();

        public readonly ObservableCollection<ActivityViewModel> _activities = new ObservableCollection<ActivityViewModel>();

        public SlaveViewModel()
        {
            _slaveStorage.HoldingRegisters.StorageOperationOccurred += HoldingRegisterOperation;
            _slaveStorage.InputRegisters.StorageOperationOccurred += InputRegisterOperation;
            _slaveStorage.CoilDiscretes.StorageOperationOccurred += CoilDiscreteOperation;
            _slaveStorage.CoilInputs.StorageOperationOccurred += CoilInputOperation;

            for (ushort pointIndex = 0; pointIndex < ushort.MaxValue; pointIndex++)
            {
                _holdingRegisters.Add(new SlaveRegisterViewModel(_slaveStorage.HoldingRegisters, pointIndex));
                _inputRegisters.Add(new SlaveRegisterViewModel(_slaveStorage.InputRegisters, pointIndex));
                _coilDiscretes.Add(new CoilViewModel(_slaveStorage.CoilDiscretes, pointIndex));
                _coilInputs.Add(new CoilViewModel(_slaveStorage.CoilInputs, pointIndex));
            }
           
        }

        private void AddActivity(string pointType, PointOperation operation, ushort startingAddress, ushort[] values)
        {
            var hexNumbers = values
                .Select(r => Convert.ToString(r, 16).PadLeft(4, '0'))
                .ToArray();

            AddActivity($"{operation} {pointType}", startingAddress, hexNumbers);
        }

        private void AddActivity(string pointType, PointOperation operation, ushort startingAddress, bool[] values)
        {
            var individualValues = values
                .Select(r => r ? "1" : "0")
                .ToArray();

            AddActivity($"{operation} {pointType}", startingAddress, individualValues);
        }

        private void AddActivity(string operation, ushort startingAddress, string[] values)
        {
            var activity = new ActivityViewModel(DateTime.Now, operation, startingAddress, values);

            DispatcherHelper.CheckBeginInvokeOnUI(() => Activities.Add(activity));
        }

        void HoldingRegisterOperation(object sender, StorageEventArgs<ushort> e)
        {
            AddActivity("Holding Register", e.Operation, e.StartingAddress, e.Points);

            if (e.Operation == PointOperation.Write)
            {
                for (int index = 0; index < e.Points.Length; index++)
                {
                    _holdingRegisters[index + e.StartingAddress].OnValueChanged();
                }
            }
        }

        void InputRegisterOperation(object sender, StorageEventArgs<ushort> e)
        {
            AddActivity("Input Register", e.Operation, e.StartingAddress, e.Points);

            if (e.Operation == PointOperation.Write)
            {
                for (int index = 0; index < e.Points.Length; index++)
                {
                    _inputRegisters[index + e.StartingAddress].OnValueChanged();
                }
            }
        }

        void CoilDiscreteOperation(object sender, StorageEventArgs<bool> e)
        {
            AddActivity("Discrete Coil", e.Operation, e.StartingAddress, e.Points);

            if (e.Operation == PointOperation.Write)
            {
                for (int index = 0; index < e.Points.Length; index++)
                {
                    _coilDiscretes[index + e.StartingAddress].OnValueChanged();
                }
            }
        }

        void CoilInputOperation(object sender, StorageEventArgs<bool> e)
        {
            AddActivity("Discrete Input", e.Operation, e.StartingAddress, e.Points);

            if (e.Operation == PointOperation.Write)
            {
                for (int index = 0; index < e.Points.Length; index++)
                {
                    _coilInputs[index + e.StartingAddress].OnValueChanged();
                }
            }
        }

        public void ClearActivity()
        {
            _activities.Clear();
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

        public ObservableCollection<CoilViewModel> CoilDiscretes
        {
            get { return _coilDiscretes; }
        }

        public ObservableCollection<CoilViewModel> CoilInputs
        {
            get { return _coilInputs; }
        }

        public IModbusSlave CreateModbusSlave()
        {
            var factory = new ModbusFactory();

            //Attach our custom storage
            return factory.CreateSlave(SlaveId, _slaveStorage);
        }
    }
}

