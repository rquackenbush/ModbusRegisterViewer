using GalaSoft.MvvmLight;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    public class CoilViewModel : ViewModelBase, IPointViewModel<bool>
    {
        private ushort _address;
        private bool _value;
        private bool _isDirty;

        public ushort Address
        {
            get { return _address; }
            private set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value; 
                RaisePropertyChanged();
            }
        }

        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value; 
                RaisePropertyChanged();
                IsDirty = true;
            }
        }

        public void SetValue(bool value)
        {
            _value = value;
            RaisePropertyChanged(nameof(Value));
            IsDirty = false;
        }

        public void Initialize(ushort address, bool value)
        {
            Address = address;
            _value = value;
            RaisePropertyChanged(nameof(Value));
        }
    }
}