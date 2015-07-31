using System.Windows.Media;
using GalaSoft.MvvmLight;
using ModbusTools.SlaveExplorer.Interfaces;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        private readonly IRuntimeField _runtimeField;
        
        public FieldViewModel(IRuntimeField runtimeField)
        {
            _runtimeField = runtimeField;
        }

        public string Name
        {
            get { return _runtimeField.Name; }
        }

        public int Offset
        {
            get { return _runtimeField.Offset; }
        }

        public Visual Visual
        {
            get { return _runtimeField.Visual; }
        }

        public IRuntimeField RuntimeField
        {
            get { return _runtimeField; }
        }
    }
}
