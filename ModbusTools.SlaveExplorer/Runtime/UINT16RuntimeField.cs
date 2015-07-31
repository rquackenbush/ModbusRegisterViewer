using System;
using System.Windows;
using System.Windows.Media;
using MiscUtil.Conversion;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class UINT16RuntimeField : RuntimeFieldBase
    {
        private readonly IntegerUpDown _visual;

        public UINT16RuntimeField(string name, int offset) 
            : base(name, offset)
        {
            _visual = new IntegerUpDown()
            {
                Minimum = ushort.MinValue,
                Maximum = ushort.MaxValue,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0)
            };
        }

        public override int Size
        {
            get { return 2; }
        }

        public override Visual Visual
        {
            get { return _visual; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToUInt16(data, 0);

            _visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (ushort)(_visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }
    }
}
