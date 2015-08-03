using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
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
