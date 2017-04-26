using ModbusTools.Common.ViewModel;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    public class SlaveExplorerRegisterViewModel : RegisterViewModel, IPointViewModel<ushort>
    {
        public void SetValue(ushort value)
        {
            Value = value;
            IsDirty = false;
        }

        public void Initialize(ushort address, ushort value)
        {
            Address = address;
            Value = value;
        }
    }
}
