using System;
using System.Windows;
using MiscUtil.Conversion;
using ModbusTools.Common;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public class FIXED16RuntimeField : RuntimeFieldBase
    {
        private readonly RuntimeFieldEditor<DoubleUpDown> _editor;

        private readonly FixedPointOptionWrapper _options;

        public FIXED16RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
         {
             _options = new FixedPointOptionWrapper(fieldModel.Options);

             _editor = new RuntimeFieldEditor<DoubleUpDown>(
                fieldModel.Name,
                new DoubleUpDown()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    BorderThickness = new Thickness(0),
                    ClipValueToMinMax = true,
                    Minimum = Int16.MinValue / _options.Scale,
                    Maximum = Int16.MaxValue / _options.Scale
                });
        }

        public override int Size
        {
            get { return 2; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToInt16(data, 0);

            _editor.Visual.Value = value / _options.Scale;
        }

        public override byte[] GetBytes()
        {
            var value = (Int16) ((_editor.Visual.Value ?? 0)*_options.Scale);

            return EndianBitConverter.Big.GetBytes(value);
        }

        public override IRuntimeFieldEditor[] FieldEditors
        {
            get { return _editor.ToSingletonArray<IRuntimeFieldEditor>(); }
        }
    }
}
