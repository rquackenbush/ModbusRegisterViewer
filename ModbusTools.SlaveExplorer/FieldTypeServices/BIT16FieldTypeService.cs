using System;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;
using ModbusTools.SlaveExplorer.View;
using ModbusTools.SlaveExplorer.ViewModel;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
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