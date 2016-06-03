using System.Windows;
using ModbusTools.Common;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public class INT8RuntimeField : RuntimeFieldBase
    {
        private readonly RuntimeFieldEditor<IntegerUpDown> _editor;

         public INT8RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
        {
            _editor = new RuntimeFieldEditor<IntegerUpDown>(
               fieldModel.Name,
               new IntegerUpDown()
               {
                   Minimum = sbyte.MinValue,
                   Maximum = sbyte.MaxValue,
                   VerticalAlignment = VerticalAlignment.Stretch,
                   HorizontalAlignment = HorizontalAlignment.Stretch,
                   Margin = new Thickness(0),
                   BorderThickness = new Thickness(0),
                   ClipValueToMinMax = true
               });
        }

        public override int Size
        {
            get { return 1; }
        }

        public override void SetBytes(byte[] data)
        {
            _editor.Visual.Value = data[0];
        }

        public override byte[] GetBytes()
        {
            var value = (byte)(_editor.Visual.Value ?? 0);

            return new [] { value };
        }

        public override IRuntimeFieldEditor[] FieldEditors
        {
            get { return _editor.ToSingletonArray<IRuntimeFieldEditor>(); }
        }
    }
}
