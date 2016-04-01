using System;
using System.Linq;
using System.Windows.Controls;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public abstract class BITRuntimeFieldBase : RuntimeFieldBase
    {
        private readonly RuntimeFieldEditor<CheckBox>[] _allFieldEditors;
        private readonly RuntimeFieldEditor<CheckBox>[] _displayedFieldEditors;

        protected BITRuntimeFieldBase(FieldModel fieldModel, int numberOfBits) : base(fieldModel)
        {
            var optionWrapper = new BITOptionWrapper(fieldModel.Options, numberOfBits);

            string[] names = optionWrapper.GetNames();

            //Create tuples that contain the display name 
            var tuples = names
                .Select(n => new Tuple<string, RuntimeFieldEditor<CheckBox>>(
                    n,
                    new RuntimeFieldEditor<CheckBox>(fieldModel.Name + " - " + n, new CheckBox())))
                .ToArray();

            _allFieldEditors = tuples
                .Select(t => t.Item2)
                .ToArray();

            _displayedFieldEditors = tuples
                .Where(t => !string.IsNullOrWhiteSpace(t.Item1))
                .Select(t => t.Item2)
                .ToArray();
        }

        protected RuntimeFieldEditor<CheckBox>[] AllFieldEditors
        {
            get { return _allFieldEditors; }
        }

        public sealed override int Size
        {
            get { return _allFieldEditors.Length/8; }
        }

        public abstract override void SetBytes(byte[] data);

        public abstract override byte[] GetBytes();

        public sealed override IRuntimeFieldEditor[] FieldEditors
        {
            get { return _displayedFieldEditors.Cast<IRuntimeFieldEditor>().ToArray(); }
        }
    }
}