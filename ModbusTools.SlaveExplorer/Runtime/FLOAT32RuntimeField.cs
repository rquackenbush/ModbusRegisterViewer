using System;
using System.Windows;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class FLOAT32RuntimeField : RuntimeFieldBase
    {
         private readonly RuntimeFieldEditor<DoubleUpDown> _editor;

         public FLOAT32RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
         {
             _editor = new RuntimeFieldEditor<DoubleUpDown>(
               fieldModel.Name,
               new DoubleUpDown()
                {
                    Minimum = Single.MinValue,
                    Maximum = Single.MaxValue,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    BorderThickness = new Thickness(0)
                });
        }

        public override int Size
        {
            get { return 4; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToSingle(data, 0);

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
