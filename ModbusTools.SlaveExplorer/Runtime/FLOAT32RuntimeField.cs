using System;
using System.Windows;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.SlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class FLOAT32RuntimeField : RuntimeFieldBase
    {
         private readonly DoubleUpDown _visual;

         public FLOAT32RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
         {
            _visual = new DoubleUpDown()
            {
                Minimum = Single.MinValue,
                Maximum = Single.MaxValue,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0)
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
            var value = EndianBitConverter.Big.ToSingle(data, 0);

            _visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (Int16)(_visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }
    }
}
