using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class INT32FieldTypeService : FieldTypeServiceBase
    {
        internal INT32FieldTypeService() 
            : base(FieldType.INT32)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new INT32RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 4;
        }
    }
}
