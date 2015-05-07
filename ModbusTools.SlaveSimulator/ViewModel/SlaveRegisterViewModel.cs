using System;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveSimulator.Model;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveRegisterViewModel : RegisterViewModel
    {
        private readonly IRegisterStorage _registerStorage;

        public SlaveRegisterViewModel(IRegisterStorage registerStorage, ushort registerIndex) 
            : base(registerIndex)
        {
            if (registerStorage == null) throw new ArgumentNullException("registerStorage");
            _registerStorage = registerStorage;
        }

        public override ushort Value
        {
            get { return _registerStorage.ReadSingle(RegisterIndex); }
            set
            {
                _registerStorage.WriteSingle(RegisterIndex, value);
                OnValueUpdated();
            }
        }

        public void OnValueChanged()
        {
            OnValueUpdated();
        }
    }
}
