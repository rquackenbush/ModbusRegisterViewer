using GalaSoft.MvvmLight;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public abstract class RangeViewModelBase : ViewModelBase
    {
        private string _name;
        private bool _isExpanded = true;

        protected RangeViewModelBase()
        {
            
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; 
                RaisePropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value; 
                RaisePropertyChanged();
            }
        }
    }
}
