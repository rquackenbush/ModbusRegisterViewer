using System;

namespace ModbusRegisterViewer.ViewModel.RegisterViewer
{
    public class RegisterViewModel
    {
        private readonly ushort _registerNumber;
        private readonly ushort _value;

        public RegisterViewModel(ushort registerNumber, ushort value)
        {
            _registerNumber = registerNumber;
            _value = value;
        }

        public ushort RegisterNumber
        {
            get { return _registerNumber; }
        }

        public ushort Value
        {
            get { return _value; }
        }

        public byte MSB
        {
            get { return (byte) (_value >> 8); }
        }

        public byte LSB
        {
            get { return (byte) _value; }
        }

        public string Hex
        {
            get { return string.Format("0x{0:x4}", _value); }
        }

        public string Binary
        {
            get { return Convert.ToString(_value, 2).PadLeft(16, '0').Insert(8, " "); }
        }

    }
}
