using System;
using System.Globalization;
using GalaSoft.MvvmLight;
using ModbusTools.SlaveViewer.Model;

namespace ModbusTools.SlaveViewer.ViewModel
{
    public class WriteableRegisterViewModel : ViewModelBase 
    {
        private readonly ushort _registerNumber;
        private ushort _value;
        private bool _isDirty;
        private readonly DescriptionStore _descriptionStore;
        
        internal WriteableRegisterViewModel(ushort registerNumber, ushort value, DescriptionStore descriptionStore)
        {
            _registerNumber = registerNumber;
            _value = value;
            _descriptionStore = descriptionStore;
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
                    RaisePropertyChanged(() => Signed);

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
            set
            {
                ushort converted;

                if (value != null)
                {
                    value = value.Replace("0x", "");
                }

                if (ushort.TryParse(value, NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber, null, out converted))
                {
                    Value = converted;
                }
                else
                {
                    RaisePropertyChanged();
                }
            }
        }

        public string Binary
        {
            get { return Convert.ToString(_value, 2).PadLeft(16, '0').Insert(8, " "); }
            set
            {
                try
                {
                    if (value != null)
                        value = value.Replace(" ", "");

                    Value = Convert.ToUInt16(value, 2);
                }
                catch (Exception)
                {
                    RaisePropertyChanged();
                }
                
            }
        }

        public short Signed
        {
            get { return (short) this.Value; }
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

        public string Description
        {
            get { return _descriptionStore[_registerNumber]; }
            set
            {
                _descriptionStore[_registerNumber] = value;
                RaisePropertyChanged(() => Description);
            }
        }

        internal void RaiseDescriptionPropertyChanged()
        {
            RaisePropertyChanged(() => Description);
        }
    }
}
