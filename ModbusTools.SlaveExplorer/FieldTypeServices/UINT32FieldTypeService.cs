using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal class UINT32FieldTypeService : FieldTypeServiceBase
    {
        internal UINT32FieldTypeService() 
            : base(FieldType.UINT32)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new UINT32RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 4;
        }
    }
}
