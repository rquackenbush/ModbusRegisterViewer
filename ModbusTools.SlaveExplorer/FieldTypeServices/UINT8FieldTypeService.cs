using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal class UINT8FieldTypeService : FieldTypeServiceBase
    {
         internal UINT8FieldTypeService() 
            : base(FieldType.UINT8)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new UINT8RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 1;
        }
    }
}
