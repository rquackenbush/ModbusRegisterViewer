using System;
using System.Globalization;
using GalaSoft.MvvmLight;

namespace ModbusTools.Common.ViewModel
{
    public class RegisterViewModel : ViewModelBase 
    {
        private readonly ushort _registerIndex;
        private ushort _value;
        private bool _isDirty;

        public RegisterViewModel(ushort registerIndex)
        {
            _registerIndex = registerIndex;
        }
        
        public RegisterViewModel(ushort registerIndex, ushort value) 
            : this(registerIndex)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the 0 based index of the register.
        /// </summary>
        public ushort RegisterIndex
        {
            get { return _registerIndex; }
        }

        /// <summary>
        /// Gets the 0 or 1 based register number based on preferences.
        /// </summary>
        public int RegisterNumber
        {
            get
            {
                if (IsZeroBased)
                    return _registerIndex;

                return _registerIndex + 1;
            }
        }

        public virtual ushort Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;

                    OnValueUpdated();

                    IsDirty = true;
                }
            }
        }

        protected void OnValueUpdated()
        {
            RaisePropertyChanged(() => Value);
            RaisePropertyChanged(() => MSB);
            RaisePropertyChanged(() => LSB);
            RaisePropertyChanged(() => Hex);
            RaisePropertyChanged(() => Binary);
            RaisePropertyChanged(() => Signed);
        }

        public byte MSB
        {
            get { return (byte) (Value >> 8); }
            set
            {
                ushort temp = value;

                temp <<= 8;

                temp += LSB;

                Value = temp;
            }
        }

        public byte LSB
        {
            get { return (byte) Value; }
            set { Value = (ushort) ((ushort) value + (ushort) (Value & 0xff00)); }
        }

        public string Hex
        {
            get { return string.Format("0x{0:x4}", Value); }
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
            get { return Convert.ToString(Value, 2).PadLeft(16, '0').Insert(8, " "); }
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
            get { return (short) Value; }
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

        private bool _isZeroBased;
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set
            {
                _isZeroBased = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => RegisterNumber);
            }
        }

       
    }
}
