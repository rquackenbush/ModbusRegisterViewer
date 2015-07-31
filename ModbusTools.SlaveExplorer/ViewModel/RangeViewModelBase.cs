using GalaSoft.MvvmLight;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public abstract class RangeViewModelBase : ViewModelBase
    {
        private readonly IDirty _dirty;
        private string _name;
        private bool _isExpanded = true;

        protected RangeViewModelBase(IDirty dirty)
        {
            _dirty = dirty;
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
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged();
                    _dirty.MarkDirtySafe();
                }
            }
        }

        protected internal abstract RangeModel GetModel();
    }
}
