using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class UINT8FieldTypeService : FieldTypeServiceBase
    {
         internal UINT8FieldTypeService() 
            : base(FieldType.UINT8)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new UINT8RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 1;
        }
    }
}
