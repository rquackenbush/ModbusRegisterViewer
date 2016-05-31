using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class BIT8FieldTypeService : BitFieldTypeServiceBase
    {
        public BIT8FieldTypeService() 
            : base(FieldType.BIT8, 8)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new BIT8RuntimeField(fieldModel);
        }
    }
}
