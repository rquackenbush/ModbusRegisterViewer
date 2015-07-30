using GalaSoft.MvvmLight;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        private string _name;
        private int _offset;

        public FieldViewModel()
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

        public int Offset
        {
            get { return _offset; }
            internal set
            {
                _offset = value; 
                RaisePropertyChanged();
            }
        }
    }
}
