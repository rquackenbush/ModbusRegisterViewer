using System;
using MiscUtil.Conversion;
using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public class BIT32RuntimeField : BITRuntimeFieldBase
    {
        public BIT32RuntimeField(FieldModel fieldModel) 
            : base(fieldModel, 32)
        {
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToUInt32(data, 0);

            for (ushort index = 0; index < 32; index++)
            {
                var mask = (ushort)(1 << index);

                var isSet = (value & mask) > 0;

                AllFieldEditors[index].Visual.IsChecked = isSet;
            }
        }

        public override byte[] GetBytes()
        {
            UInt32 value = 0;

            for (ushort index = 0; index < 32; index++)
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