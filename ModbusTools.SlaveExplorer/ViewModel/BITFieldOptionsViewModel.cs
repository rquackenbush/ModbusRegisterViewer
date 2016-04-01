using System;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class BitFieldOptionsViewModel : ViewModelBase, ICloseableViewModel
    {
        private readonly BITOptionWrapper _options;

        private readonly BitViewModel[] _bitNameViewModels;

        internal BitFieldOptionsViewModel(BITOptionWrapper options)
        {
            _options = options;

            OkCommand = new RelayCommand(Ok, CanOk);
            CancelCommand = new RelayCommand(Cancel);

            var bitIndexRange = Enumerable.Range(0, options.NumberOfBits);

            _bitNameViewModels = bitIndexRange
                .Select(bitIndex => new BitViewModel(options, bitIndex))
                .ToArray();
        }

        public string Title
        {
            get { return $"BIT{_options.NumberOfBits} Options"; }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public BitViewModel[] Names
        {
            get {  return _bitNameViewModels;}
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
