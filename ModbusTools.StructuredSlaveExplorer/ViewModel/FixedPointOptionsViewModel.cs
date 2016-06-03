using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.ViewModel
{
    public class FixedPointOptionsViewModel : ViewModelBase, ICloseableViewModel
    {
        private readonly FixedPointOptionWrapper _options;

        internal FixedPointOptionsViewModel(FixedPointOptionWrapper options)
        {
            _options = options;
            OkCommand = new RelayCommand(Ok, CanOk);
            CancelCommand = new RelayCommand(Cancel);
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public double Scale
        {
            get { return _options.Scale; }
            set { _options.Scale = value; }
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
