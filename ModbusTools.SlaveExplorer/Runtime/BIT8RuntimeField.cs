using System.Linq;
using System.Windows.Controls;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class BIT8RuntimeField : RuntimeFieldBase
    {
        private readonly RuntimeFieldEditor<CheckBox>[] _fieldEditors;

        public BIT8RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
        {
            var optionWrapper = new BIT8OptionWrapper(fieldModel.Options);

            _fieldEditors = new []
            {
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit0Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit1Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit2Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit3Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit4Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit5Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit6Name, new CheckBox()),
                new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + optionWrapper.Bit7Name, new CheckBox()),
            };
        }

        public override int Size
        {
            get { return 1; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = data[0];

            for (byte index = 0; index < 8; index++)
            {
                var mask = (byte) (1 << index);

                var isSet = (value & mask) > 0;

                _fieldEditors[index].Visual.IsChecked = isSet;
            }
        }

        public override byte[] GetBytes()
        {
            byte value = 0;

            for (byte index = 0; index < 8; index++)
            {
                if (_fieldEditors[index].Visual.IsChecked == true)
                {
                    var mask = (byte)(1 << index);

                    value |= mask;
                }
            }

            return value.ToSingletonArray();
        }

        public override IRuntimeFieldEditor[] FieldEditors
        {
            get { return _fieldEditors.Cast<IRuntimeFieldEditor>().ToArray(); }
        }
    }
}
