using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal class INT16FieldTypeService : FieldTypeServiceBase
    {
        internal INT16FieldTypeService() 
            : base(FieldType.INT16)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new INT16RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 2;
        }
    }
}
