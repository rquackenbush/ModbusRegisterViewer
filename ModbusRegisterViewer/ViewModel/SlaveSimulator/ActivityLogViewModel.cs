using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Modbus.Data;
using Modbus.Utility;

namespace ModbusRegisterViewer.ViewModel.SlaveSimulator
{
    public class ActivityLogViewModel : ViewModelBase
    {
        private readonly DateTime _timestamp;
        private readonly DiscriminatedUnion<ReadOnlyCollection<bool>, ReadOnlyCollection<ushort>> _data;
        private readonly ModbusDataType _dataType;
        private readonly ReadWrite _readWrite;
        private readonly int _startingIndex;

        public ActivityLogViewModel(DateTime timestamp, ModbusDataType dataType, int startingIndex,  DiscriminatedUnion<ReadOnlyCollection<bool>, ReadOnlyCollection<ushort>> data, ReadWrite readWrite, bool isZeroBased)
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
            get { return _data.B.Count; }
        }

        public string Data
        {
            get
            {
                switch (Type)
                {
                    case ModbusDataType.Coil:
                        return "Coil Data";

                    case ModbusDataType.Input:
                        return "Input";

                    case ModbusDataType.HoldingRegister:
                    case ModbusDataType.InputRegister:

                        var hexNumbers = _data.B.Select(r => Convert.ToString(r, 16).PadLeft(4, '0')).ToArray();

                        return string.Join(" ", hexNumbers);

                    default:
                        return "";
                }
            }
            
        }

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
