using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ModbusRegisterViewer.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        private readonly RangeViewModel _parent;

        public FieldViewModel(RangeViewModel parent)
        {
            _parent = parent;
        }

        private ushort _byteAddress;
        public ushort ByteAddress
        {
            get { return _byteAddress; }
            set
            {
                _byteAddress = value;
                RaisePropertyChanged(() => ByteAddress);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private ObservableCollection<ControlViewModel> _controls = new ObservableCollection<ControlViewModel>();
        public ObservableCollection<ControlViewModel> Controls
        {
            get { return _controls; }
            set
            {
                _controls = value;
                RaisePropertyChanged(() => Controls);
            }
        }
        
    }
}
