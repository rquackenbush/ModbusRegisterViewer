﻿using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal class BIT8FieldTypeService : BitFieldTypeServiceBase
    {
        public BIT8FieldTypeService() 
            : base(FieldType.BIT8, 8)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new BIT8RuntimeField(fieldModel);
        }
    }
}
