using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class RegisterRangeViewModel : RangeViewModelBase
    {
        private readonly RangeModel _rangeModel;
        private bool _isZeroBased;
        private ushort _startingRegisterIndex;

        public RegisterRangeViewModel(RangeModel rangeModel)
        {
            _rangeModel = rangeModel;
            EditCommand = new RelayCommand(Edit, CanEdit);
        }

        public ICommand EditCommand { get; private set; }

        private void Edit()
        {
            var rangeModel = _rangeModel.Clone();

            var rangeEditorViewModel = new RegisterRangeEditorViewModel(rangeModel);

            var view = new RegisterRangeEditorView()
            {
                DataContext = rangeEditorViewModel
            };

            var result = view.ShowDialog();

            if (result == true)
            {
                //TODO:
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
