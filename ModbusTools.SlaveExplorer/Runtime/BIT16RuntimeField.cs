using System;
using System.Linq;
using System.Windows.Controls;
using MiscUtil.Conversion;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class BIT16RuntimeField : BITRuntimeFieldBase
    {
        public BIT16RuntimeField(FieldModel fieldModel) 
            : base(fieldModel, 16)
        {
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToUInt16(data, 0);

            for (ushort index = 0; index < 16; index++)
            {
                var mask = (ushort)(1 << index);

                var isSet = (value & mask) > 0;

                AllFieldEditors[index].Visual.IsChecked = isSet;
            }
        }

        public override byte[] GetBytes()
        {
            ushort value = 0;

            for (ushort index = 0; index < 16; index++)
            {
                if (AllFieldEditors[index].Visual.IsChecked == true)
                {
                    var mask = (ushort)(1 << index);

                    value |= mask;
                }
            }

            return EndianBitConverter.Big.GetBytes(value);
        }
    }
}