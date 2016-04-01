﻿using System;
using System.Linq;
using System.Windows.Controls;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class BIT8RuntimeField : BITRuntimeFieldBase
    {
        public BIT8RuntimeField(FieldModel fieldModel) 
            : base(fieldModel, 8)
        {
        }

        public override void SetBytes(byte[] data)
        {
            var value = data[0];

            for (byte index = 0; index < 8; index++)
            {
                var mask = (byte) (1 << index);

                var isSet = (value & mask) > 0;

                AllFieldEditors[index].Visual.IsChecked = isSet;
            }
        }

        public override byte[] GetBytes()
        {
            byte value = 0;

            for (byte index = 0; index < 8; index++)
            {
                if (AllFieldEditors[index].Visual.IsChecked == true)
                {
                    var mask = (byte)(1 << index);

                    value |= mask;
                }
            }

            return value.ToSingletonArray();
        }       
    }
}
