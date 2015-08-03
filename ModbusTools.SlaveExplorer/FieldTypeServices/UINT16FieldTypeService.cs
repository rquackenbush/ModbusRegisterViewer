using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal class UINT16FieldTypeService : FieldTypeServiceBase
    {
        internal UINT16FieldTypeService()
            : base(FieldType.UINT16)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new UINT16RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 2;
        }
    }
}
