using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class BIT16FieldTypeService : BitFieldTypeServiceBase
    {
        public BIT16FieldTypeService() 
            : base(FieldType.BIT16, 16)
        {
            
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new BIT16RuntimeField(fieldModel);
        }
    }
}