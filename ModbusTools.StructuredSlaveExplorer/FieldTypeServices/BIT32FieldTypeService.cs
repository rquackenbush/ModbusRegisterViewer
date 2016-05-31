using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    public class BIT32FieldTypeService : BitFieldTypeServiceBase
    {
        public BIT32FieldTypeService() 
            : base(FieldType.BIT32, 32)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new BIT32RuntimeField(fieldModel);
        }
    }
}