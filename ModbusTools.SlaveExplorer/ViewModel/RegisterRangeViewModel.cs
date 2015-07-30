using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class RegisterRangeViewModel : RangeViewModelBase
    {
        private RangeModel _rangeModel;
        private bool _isZeroBased;
        private ushort _startingRegisterIndex;

        public RegisterRangeViewModel(RangeModel rangeModel)
        {
            _rangeModel = rangeModel;
            EditCommand = new RelayCommand(Edit, CanEdit);
            Name = rangeModel.Name;
        }

        public ICommand EditCommand { get; private set; }

        private void Edit()
        {
            var rangeEditorViewModel = new RegisterRangeEditorViewModel(_rangeModel.Clone());

            var view = new RegisterRangeEditorView()
            {
                DataContext = rangeEditorViewModel
            };

            var result = view.ShowDialog();

            if (result == true)
            {
                //TODO:

                var rangeModel = rangeEditorViewModel.GetModel();

                Name = rangeModel.Name;

                _rangeModel = rangeModel;
            }
        }

        protected internal override RangeModel GetModel()
        {
            return _rangeModel;
        }

        private bool CanEdit()
        {
            return true;
        }
        
        public ushort StartingRegisterIndex
        {
            get { return _startingRegisterIndex; }
            set
            {
                _startingRegisterIndex = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => StartingRegisterNumber);
            }
        }

        public ushort StartingRegisterNumber
        {
            get
            {
                if (IsZeroBased)
                    return StartingRegisterIndex;

                return (ushort)(StartingRegisterIndex + 1);
            }
            set
            {
                if (IsZeroBased)
                {
                    StartingRegisterIndex = value;
                }
                else
                {
                    StartingRegisterIndex = (ushort)(value - 1);
                }
            }
        }

        public ushort StartingRegisterNumberMin
        {
            get
            {
                if (IsZeroBased)
                    return 0;

                return 1;
            }
        }
        
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set
            {
                _isZeroBased = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => StartingRegisterNumber);
                RaisePropertyChanged(() => StartingRegisterNumberMin);
            }
        }

    }
}
