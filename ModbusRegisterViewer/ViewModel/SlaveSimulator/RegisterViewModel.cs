using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Modbus.Data;

namespace ModbusRegisterViewer.ViewModel.SlaveSimulator
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ushort _registerNumber;
        private readonly ModbusDataCollection<ushort> _registers;

        public RegisterViewModel(ModbusDataCollection<ushort> registers,  ushort registerNumber)
        {
            _registers = registers;
            _registerNumber = registerNumber;
        }

        public ushort RegisterNumber
        {
            get { return _registerNumber; }
        }

        public ushort Value
        {
            get { return _registers[RegisterNumber]; }
            set
            {
                if (_registers[RegisterNumber] != value)
                {
                    _registers[RegisterNumber] = value;

                    OnValueUpdated();
                }
            }
        }

        internal void OnValueUpdated()
        {

            RaisePropertyChanged(() => Value);
            RaisePropertyChanged(() => MSB);
            RaisePropertyChanged(() => LSB);
            RaisePropertyChanged(() => Hex);
            RaisePropertyChanged(() => Binary); 
        }

        public byte MSB
        {
            get { return (byte) (Value >> 8); }
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
            get { return (byte) _registers[RegisterNumber]; }
            set
            {
                ushort temp = value;

                temp += MSB;
                
                this.Value = temp;
            }
        }

        public string Hex
        {
            get { return string.Format("0x{0:x4}", Value); }
        }

        public string Binary
        {
            get { return Convert.ToString(Value, 2).PadLeft(16, '0').Insert(8, " "); }
        }

       
    }
}
