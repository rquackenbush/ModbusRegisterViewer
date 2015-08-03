using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;
using ModbusTools.SlaveExplorer.View;
using ModbusTools.SlaveExplorer.ViewModel;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
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
