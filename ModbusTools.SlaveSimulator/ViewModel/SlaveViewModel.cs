using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using ModbusTools.SlaveSimulator.Model;
using ModbusTools.SlaveSimulator.Model.FunctionHandlers;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        private readonly SparseRegisterStorage _holdingRegisterStorage = new SparseRegisterStorage();
        private readonly SparseRegisterStorage _inputRegisterStorage = new SparseRegisterStorage();

        private readonly RegisterStorageNotifier _holdingRegisterNotifier;
        private readonly RegisterStorageNotifier _inputRegisterNotifier;

        private readonly ObservableCollection<SlaveRegisterViewModel> _holdingRegisters = new ObservableCollection<SlaveRegisterViewModel>();
        private readonly ObservableCollection<SlaveRegisterViewModel> _inputRegisters = new ObservableCollection<SlaveRegisterViewModel>();

        public readonly ObservableCollection<ActivityViewModel> _activities = new ObservableCollection<ActivityViewModel>();

        public SlaveViewModel()
        {
            //Set up the holding registers
            _holdingRegisterNotifier = new RegisterStorageNotifier(_holdingRegisterStorage);

            _holdingRegisterNotifier.DataWasRead += HoldingRegister_DataWasRead;
            _holdingRegisterNotifier.DataWasWritten += HoldingRegister_DataWasWritten;

            //Set up the input registers
            _inputRegisterNotifier = new RegisterStorageNotifier(_inputRegisterStorage);

            _inputRegisterNotifier.DataWasRead += InputRegister_DataWasRead;


            for (ushort registerIndex = 0; registerIndex < ushort.MaxValue; registerIndex++)
            {
                _holdingRegisters.Add(new SlaveRegisterViewModel(_holdingRegisterStorage, registerIndex) {IsZeroBased = IsZeroBased});
            }

            for (ushort registerIndex = 0; registerIndex < ushort.MaxValue; registerIndex++)
            {
                _inputRegisters.Add(new SlaveRegisterViewModel(_inputRegisterStorage, registerIndex) {IsZeroBased = IsZeroBased});
            }
        }

        private void InputRegister_DataWasRead(object sender, RegisterStorageEventArgs e)
        {
            var activity = new RegisterActivityViewModel(DateTime.Now, "Input Register Read", e.StartingAddress, e.Values, false);

            DispatcherHelper.CheckBeginInvokeOnUI(() => Activities.Add(activity));
        }

        void HoldingRegister_DataWasWritten(object sender, RegisterStorageEventArgs e)
        {
            var activity = new RegisterActivityViewModel(DateTime.Now, "Holding Register Write", e.StartingAddress, e.Values, false);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Activities.Add(activity);

                for (int index = 0; index < e.Values.Length; index++)
                {
                    HoldingRegisters[index + e.StartingAddress].OnValueChanged();
                }
            });
        }

        void HoldingRegister_DataWasRead(object sender, RegisterStorageEventArgs e)
        {
            var activity = new RegisterActivityViewModel(DateTime.Now, "Holding Register Read", e.StartingAddress, e.Values, false);

            DispatcherHelper.CheckBeginInvokeOnUI(() => Activities.Add(activity));
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

        private bool _isZeroBased;
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set
            {
                _isZeroBased = value;
                RaisePropertyChanged();

                foreach (var register in HoldingRegisters)
                {
                    register.IsZeroBased = value;
                }

                foreach (var register in InputRegisters)
                {
                    register.IsZeroBased = value;
                }

                foreach (var activity in Activities)
                {
                    activity.IsZeroBased = value;
                }               
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

        public ISlave GetSlave()
        {
            return new Slave(SlaveId,
                new IModbusFunctionHandler[]
                {
                    new ReadHoldingRegistersFunctionHandler(_holdingRegisterNotifier), 
                    new WriteHoldingRegistersFunctionHandler(_holdingRegisterNotifier), 
                    new ReadInputRegistersFunctionHandler(_inputRegisterNotifier)
                });
        }
    }
}
