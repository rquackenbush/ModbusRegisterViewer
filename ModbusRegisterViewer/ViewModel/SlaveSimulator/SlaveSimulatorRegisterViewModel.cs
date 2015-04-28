using System;
using Modbus.Data;
using ModbusTools.Common.ViewModel;

namespace ModbusRegisterViewer.ViewModel.SlaveSimulator
{
    public class SlaveSimulatorRegisterViewModel : RegisterViewModel
    {
        private readonly ModbusDataCollection<ushort> _registers;

        public SlaveSimulatorRegisterViewModel(ModbusDataCollection<ushort> registers, ushort registerIndex)
            : base(registerIndex)
        {
            if (registers == null) throw new ArgumentNullException("registers");

            _registers = registers;
        }

        public override ushort Value
        {
            get { return _registers[RegisterIndex + 1]; }
            set
            {
                if (_registers[RegisterIndex + 1] != value)
                {
                    _registers[RegisterIndex + 1] = value;

                    OnValueUpdated();
                }
            }
        }

        internal new void OnValueUpdated()
        {
            HasBeenTouched = true;

            base.OnValueUpdated();
        }

        private bool _hasBeenTouched;
        public bool HasBeenTouched
        {
            get { return _hasBeenTouched; }
            set
            {
                _hasBeenTouched = value; 
                RaisePropertyChanged();
            }
        }
    }
}
