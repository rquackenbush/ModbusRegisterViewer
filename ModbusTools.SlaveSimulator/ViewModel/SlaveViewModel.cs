using GalaSoft.MvvmLight;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        public byte SlaveIdMin
        {
            get { return 1; }
        }

        public byte SlaveIdMax
        {
            get { return 247; }
        }

        private byte _slaveId;
        public byte SlaveId
        {
            get { return _slaveId; }
            set
            {
                _slaveId = value; 
                RaisePropertyChanged();
            }
        }
    }
}
