using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.Runtime;
using ModbusTools.StructuredSlaveExplorer.View;
using ModbusTools.StructuredSlaveExplorer.ViewModel;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    internal  class UFIXED16FieldTypeService : FieldTypeServiceBase
    {
        internal UFIXED16FieldTypeService() 
            : base(FieldType.UFIXED16)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new UFIXED16RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 2;
        }

        public override bool SupportsOptions
        {
            get { return true; }
        }

        public override FieldOptionModel[] EditOptions(FieldOptionModel[] options)
        {
            var optionsWrapper = new FixedPointOptionWrapper(options);

            var viewModel = new FixedPointOptionsViewModel(optionsWrapper);

            var view = new FixedPointOptionsView()
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
