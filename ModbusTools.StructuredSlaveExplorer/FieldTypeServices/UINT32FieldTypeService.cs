using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class UINT32FieldTypeService : FieldTypeServiceBase
    {
        internal UINT32FieldTypeService() 
            : base(FieldType.UINT32)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new UINT32RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 4;
        }
    }
}
