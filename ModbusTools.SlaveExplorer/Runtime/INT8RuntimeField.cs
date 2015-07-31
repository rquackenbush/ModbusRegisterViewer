using System.Windows;
using System.Windows.Media;
using ModbusTools.SlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class INT8RuntimeField : RuntimeFieldBase
    {
         private readonly IntegerUpDown _visual;

         public INT8RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
        {
            _visual = new IntegerUpDown()
            {
                Minimum = sbyte.MinValue,
                Maximum = sbyte.MaxValue,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0),
                BorderThickness = new Thickness(0),
                ClipValueToMinMax = true
            };
        }

        public override int Size
        {
            get { return 1; }
        }

        public override Visual Visual
        {
            get { return _visual; }
        }

        public override void SetBytes(byte[] data)
        {
            _visual.Value = data[0];
        }

        public override byte[] GetBytes()
        {
            var value = (byte)(_visual.Value ?? 0);

            return new [] { value };
        }
    }
}
