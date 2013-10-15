using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ModbusRegisterViewer.ViewModel
{
    public class ControlPanelViewModel : ViewModelBase
    {

        private ObservableCollection<RangeViewModel> _ranges = new ObservableCollection<RangeViewModel>();
        public ObservableCollection<RangeViewModel> Ranges
        {
            get { return _ranges; }
            set
            {
                _ranges = value;
                RaisePropertyChanged(() => Ranges);
            }
        }

    }
}
