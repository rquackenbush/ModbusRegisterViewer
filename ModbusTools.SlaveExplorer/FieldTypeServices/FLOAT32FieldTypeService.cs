using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal class FLOAT32FieldTypeService : FieldTypeServiceBase
    {
          public FLOAT32FieldTypeService() 
            : base(FieldType.FLOAT32)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new FLOAT32RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 4;
        }
    }
}
