using System;
using System.Linq;
using GalaSoft.MvvmLight;
using Modbus.Data;

namespace ModbusRegisterViewer.ViewModel.SlaveSimulator
{
    public class ActivityLogViewModel<TData> : ViewModelBase
        where TData : struct
    {
        private readonly DateTime _timestamp;
        private readonly ReadWrite _readWrite;
        private readonly int _startingIndex;
        private TData[] _data;

        public ActivityLogViewModel(DateTime timestamp,  int startingIndex,  TData[] data, ReadWrite readWrite, bool isZeroBased)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            _timestamp = timestamp;
            _timestamp = timestamp;
            _dataType = dataType;
            _readWrite = readWrite;
            _data = data;
            _startingIndex = startingIndex;
            _isZeroBased = isZeroBased;
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public ModbusDataType Type
        {
            get { return _dataType; }
        }

        public int StartingAddress
        {
            get
            {
                if (IsZeroBased)
                    return _startingIndex;

                return _startingIndex + 1;
            }
        }

        public int Count
        {
            get { return _data.Length; }
        }

        public abstract string Data { get; }
        //{
        //    get
        //    {
        //        switch (Type)
        //        {
        //            case ModbusDataType.Coil:
        //                return "Coil Data";

        //            case ModbusDataType.Input:
        //                return "Input";

        //            case ModbusDataType.HoldingRegister:
        //            case ModbusDataType.InputRegister:

        //                var hexNumbers = _data.Select(r => Convert.ToString(r, 16).PadLeft(4, '0')).ToArray();

        //                return string.Join(" ", hexNumbers);

        //            default:
        //                return "";
        //        }
        //    }
            
        //}

        public ReadWrite ReadWrite
        {
            get { return _readWrite; }
        }

        private bool _isZeroBased;
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set
            {
                _isZeroBased = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => StartingAddress);
            }
        }
    }
}
