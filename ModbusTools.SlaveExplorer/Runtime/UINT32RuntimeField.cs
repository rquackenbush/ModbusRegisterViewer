using System;
using System.Windows;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.SlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class UINT32RuntimeField : RuntimeFieldBase
    {
        private readonly LongUpDown _visual;

        public UINT32RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
        {
            _visual = new LongUpDown()
            {
                Minimum = UInt32.MinValue,
                Maximum = UInt32.MaxValue,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0),
                ClipValueToMinMax = true
            };
        }

        public override int Size
        {
            get { return 4; }
        }

        public override Visual Visual
        {
            get { return _visual; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToUInt32(data, 0);

            _visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (UInt32)(_visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }
    }
}
