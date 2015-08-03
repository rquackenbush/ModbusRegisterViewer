using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
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
