﻿using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.View;
using ModbusTools.StructuredSlaveExplorer.ViewModel;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    public abstract class BitFieldTypeServiceBase : FieldTypeServiceBase
    {
        private readonly int _numberOfBits;

        protected BitFieldTypeServiceBase(FieldType fieldType, int numberOfBits) : base(fieldType)
        {
            _numberOfBits = numberOfBits;
        }

        public abstract override IRuntimeField CreateRuntimeField(FieldModel fieldModel);

        public sealed override int GetSize(FieldModel fieldModel)
        {
            return _numberOfBits / 8;
        }

        public sealed override bool SupportsOptions
        {
            get { return true; }
        }

        public sealed override FieldOptionModel[] EditOptions(FieldOptionModel[] options)
        {
            var optionsWrapper = new BITOptionWrapper(options, _numberOfBits);

            var viewModel = new BitFieldOptionsViewModel(optionsWrapper);

            var view = new BITFieldOptionsView()
            {
                DataContext = viewModel
            };

            if (view.ShowDialog() == true)
            {
                return optionsWrapper.GetOptions();
            }

            return null;
        }
    }
}