using System;
using System.Windows;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.SlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class INT16RuntimeField : RuntimeFieldBase
    {
         private readonly IntegerUpDown _visual;

         public INT16RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
         {
            _visual = new IntegerUpDown()
            {
                Minimum = Int16.MinValue,
                Maximum = Int16.MaxValue,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0),
                ClipValueToMinMax = true
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
            var value = EndianBitConverter.Big.ToInt16(data, 0);

            _visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (Int16)(_visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }
    }
}
