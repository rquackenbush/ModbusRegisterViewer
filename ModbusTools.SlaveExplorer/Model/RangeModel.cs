﻿using ModbusTools.Common;

namespace ModbusTools.SlaveExplorer.Model
{
    public class RangeModel
    {
        public string Name { get; set; }

        public ushort StartIndex { get; set; }

        public RegisterType RegisterType { get; set; }

        public FieldModel[] Fields { get; set; }

        public ushort BlockSize { get; set; }

        public ushort NumberOfRegisters { get; set; }
    }
}
