using System;
using GalaSoft.MvvmLight;

namespace ModbusRegisterViewer.ViewModel.RegisterViewer
{
    public class WriteableRegisterViewModel : ViewModelBase 
    {
        private readonly ushort _registerNumber;
        private ushort _value;
        private bool _isDirty;
        
        public WriteableRegisterViewModel(ushort registerNumber, ushort value)
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
            set
            {
                if (_value != value)
                {
                    _value = value;

                    RaisePropertyChanged(() => Value);
                    RaisePropertyChanged(() => MSB);
                    RaisePropertyChanged(() => LSB);
                    RaisePropertyChanged(() => Hex);
                    RaisePropertyChanged(() => Binary);

                    this.IsDirty = true;
                }
            }
        }

        public byte MSB
        {
            get { return (byte) (_value >> 8); }
            set
            {
                ushort temp = value;

                temp <<= 8;

                temp += LSB;

                this.Value = temp;
            }
        }

        public byte LSB
        {
            get { return (byte) _value; }
            set
            {
                ushort temp = value;

                temp += MSB;
                
                this.Value = temp;
            }
        }

        public string Hex
        {
            get { return string.Format("0x{0:x4}", _value); }
        }

        public string Binary
        {
            get { return Convert.ToString(_value, 2).PadLeft(16, '0').Insert(8, " "); }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    RaisePropertyChanged(() => IsDirty);
                }
            }
        }
    }
}
