using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
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