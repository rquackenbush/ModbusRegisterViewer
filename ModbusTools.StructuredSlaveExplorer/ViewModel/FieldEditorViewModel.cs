using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.StructuredSlaveExplorer.FieldTypeServices;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.ViewModel
{
    public class FieldEditorViewModel : ViewModelBase
    {
        private readonly FieldModel _fieldModel;

        internal RegisterRangeEditorViewModel Parent { get; set; }

        public FieldEditorViewModel() 
            : this(new FieldModel()
            {
                FieldType = FieldType.UINT16
            })
        {
        }

        public FieldEditorViewModel(FieldModel fieldModel)
        {
            if (fieldModel == null) 
                throw new ArgumentNullException(nameof(fieldModel));

            _fieldModel = fieldModel;

            EditOptionsCommand = new RelayCommand(EditOptions, () => CanEditOptions);
        }

        private void PerformParentAction(Action<RegisterRangeEditorViewModel> action)
        {
            var parent = Parent;

            if (parent == null)
                return;

            action(parent);
        }

        public ICommand EditOptionsCommand { get; private set; }

        private IFieldTypeService GetFieldTypeService()
        {
            return FieldTypeServiceFactory.GetFieldTypeService(FieldType);
        }

        private void EditOptions()
        {
            var fieldTypeService = GetFieldTypeService();

            var options = fieldTypeService.EditOptions(_fieldModel.Options);

            if (options != null)
            {
                _fieldModel.Options = options;
            }
        }

        public bool CanEditOptions
        {
            get { return GetFieldTypeService().SupportsOptions; }
        }

        public int Offset
        {
            get { return _fieldModel.Offset; }
            set
            {
                _fieldModel.Offset = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _fieldModel.Name; }
            set
            {
                _fieldModel.Name = value; 
                RaisePropertyChanged();
            }
        }

        public FieldType FieldType
        {
            get { return _fieldModel.FieldType; }
            set
            {
                _fieldModel.FieldType = value; 
                RaisePropertyChanged();
                RaisePropertyChanged(() => Size);
                PerformParentAction(p => p.CalculateOffsets());
                RaisePropertyChanged(() => CanEditOptions);

                //The old options are no longer valid
                _fieldModel.Options = new FieldOptionModel[] {};
            }
        }

        public int Size
        {
            get { return GetFieldTypeService().GetSize(_fieldModel); }
        }

        public FieldModel GetModel()
        {
            return _fieldModel.Clone();
        }

     

        
    }
}
