using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ModbusRegisterViewer.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ushort _value;
        private readonly ushort _registerNumber;

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
    }
}
