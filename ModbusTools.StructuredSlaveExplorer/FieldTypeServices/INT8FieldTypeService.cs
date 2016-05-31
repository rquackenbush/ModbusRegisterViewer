using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class INT8FieldTypeService : FieldTypeServiceBase
    {
        internal INT8FieldTypeService() 
            : base(FieldType.INT8)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new INT8RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 1;
        }
    }
}
