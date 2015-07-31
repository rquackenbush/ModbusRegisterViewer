using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
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
                throw new ArgumentNullException("fieldModel");

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

        private void EditBit8Options()
        {
            var options = new BIT8OptionWrapper(_fieldModel.Options);

            var viewModel = new BIT8FieldOptionsViewModel(options);

            var view = new BIT8FieldOptionsView()
            {
                DataContext = viewModel
            };

            if (view.ShowDialog() == true)
            {
                _fieldModel.Options = options.GetOptions();
            }
        }

        private void EditFixedPointOptions()
        {
            var options = new FixedPointOptionWrapper(_fieldModel.Options);

            var viewModel = new FixedPointOptionsViewModel(options);

            var view = new FixedPointOptionsView()
            {
                DataContext = viewModel
            };

            if (view.ShowDialog() == true)
            {
                _fieldModel.Options = options.GetOptions();
            }
        }

        private void EditOptions()
        {
            switch (FieldType)
            {
                case FieldType.BIT8:
                    EditBit8Options();
                    break;

                case FieldType.FIXED16:
                case FieldType.UFIXED16:
                    EditFixedPointOptions();
                    break;
            }
        }

        public bool CanEditOptions
        {
            get
            {
                switch (FieldType)
                {
                    case FieldType.BIT8:
                    case FieldType.FIXED16:
                    case FieldType.UFIXED16:
                        return true;

                    default:
                        return false;
                }
            }
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
            get
            {
                switch (FieldType)
                {
                    case FieldType.UINT32:
                    case FieldType.INT32:
                    case FieldType.FLOAT32:
                        return 4;

                    case FieldType.UINT16:
                    case FieldType.INT16:
                    case FieldType.FIXED16:
                    case FieldType.UFIXED16:
                        return 2;

                    case FieldType.UINT8:
                    case FieldType.INT8:
                    case FieldType.BIT8:
                        return 1;

                    default:
                        throw new Exception(string.Format("Unexpected value {0}", FieldType));
                }
            }
        }

        public FieldModel GetModel()
        {
            return _fieldModel.Clone();
        }

     

        
    }
}
