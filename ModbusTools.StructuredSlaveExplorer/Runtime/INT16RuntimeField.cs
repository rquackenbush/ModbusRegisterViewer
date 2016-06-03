using System;
using System.Windows;
using MiscUtil.Conversion;
using ModbusTools.Common;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public class INT16RuntimeField : RuntimeFieldBase
    {
         private readonly RuntimeFieldEditor<IntegerUpDown> _editor;

         public INT16RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
         {
            _editor = new RuntimeFieldEditor<IntegerUpDown>(
                fieldModel.Name, 
                new IntegerUpDown()
                {
                    Minimum = Int16.MinValue,
                    Maximum = Int16.MaxValue,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    BorderThickness = new Thickness(0),
                    ClipValueToMinMax = true
                });
        }

        public override int Size
        {
            get { return 2; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToInt16(data, 0);

            _editor.Visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (Int16)(_editor.Visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }

        public override IRuntimeFieldEditor[] FieldEditors
        {
            get { return _editor.ToSingletonArray<IRuntimeFieldEditor>(); }
        }
    }
}
