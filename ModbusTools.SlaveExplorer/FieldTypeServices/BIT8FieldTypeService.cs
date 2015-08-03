using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;
using ModbusTools.SlaveExplorer.View;
using ModbusTools.SlaveExplorer.ViewModel;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal class BIT8FieldTypeService : FieldTypeServiceBase
    {
        public BIT8FieldTypeService() 
            : base(FieldType.BIT8)
        {
        }

        public override IRuntimeField CreateRuntimeField(FieldModel fieldModel)
        {
            return new BIT8RuntimeField(fieldModel);
        }

        public override int GetSize(FieldModel fieldModel)
        {
            return 1;
        }

        public override bool SupportsOptions
        {
            get { return true; }
        }

        public override FieldOptionModel[] EditOptions(FieldOptionModel[] options)
        {
            var optionsWrapper = new BIT8OptionWrapper(options);

            var viewModel = new BIT8FieldOptionsViewModel(optionsWrapper);

            var view = new BIT8FieldOptionsView()
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
