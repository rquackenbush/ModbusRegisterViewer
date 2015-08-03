using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class BIT8FieldOptionsViewModel : ViewModelBase, ICloseableViewModel
    {
        private readonly BIT8OptionWrapper _options;

        internal BIT8FieldOptionsViewModel(BIT8OptionWrapper options)
        {
            _options = options;
            OkCommand = new RelayCommand(Ok, CanOk);
            CancelCommand = new RelayCommand(Cancel);
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public string Bit0Name
        {
            get { return _options.Bit0Name; }
            set
            {
                _options.Bit0Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit1Name
        {
            get { return _options.Bit1Name; }
            set
            {
                _options.Bit1Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit2Name
        {
            get { return _options.Bit2Name; }
            set
            {
                _options.Bit2Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit3Name
        {
            get { return _options.Bit3Name; }
            set
            {
                _options.Bit3Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit4Name
        {
            get { return _options.Bit4Name; }
            set
            {
                _options.Bit4Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit5Name
        {
            get { return _options.Bit5Name; }
            set
            {
                _options.Bit5Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit6Name
        {
            get { return _options.Bit6Name; }
            set
            {
                _options.Bit6Name = value;
                RaisePropertyChanged();
            }
        }

        public string Bit7Name
        {
            get { return _options.Bit7Name; }
            set
            {
                _options.Bit7Name = value;
                RaisePropertyChanged();
            }
        } 

        private void Ok()
        {
            Close.RaiseEvent(new CloseEventArgs(true));
        }

        private bool CanOk()
        {
            return true;
        }

        private void Cancel()
        {
            Close.RaiseEvent(new CloseEventArgs(false));
        }

        #region ICloseableViewModel

        public event EventHandler<CloseEventArgs> Close;

        public bool CanClose()
        {
            return true;
        }

        public void Closed()
        {
            
        }

        #endregion
    }
}
