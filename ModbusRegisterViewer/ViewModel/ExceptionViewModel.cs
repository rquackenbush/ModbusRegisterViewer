using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ModbusRegisterViewer.ViewModel
{
    public class ExceptionViewModel : ViewModelBase
    {
        private string _title;
        private string _message;
        private string _details;
        private Visibility _visibility;
        private Exception _exception;

        public ExceptionViewModel()
        {
            _visibility = Visibility.Collapsed;
            
            this.ShowCommand = new RelayCommand<Exception>(Show);
            this.HideCommand = new RelayCommand(Hide);
            this.CopyToClipboardCommand = new RelayCommand(CopyToClipboard);
        }

        private void Show(Exception ex)
        {
            if (ex == null)
                return;

            _exception = ex;

            this.Title = ex.GetType().Name;
            this.Message = ex.Message;
            this.Details = ex.ToString();

            this.Visibility = Visibility.Visible;
        }

        private void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void CopyToClipboard()
        {
            if (_exception == null)
                return;

            var content = new StringBuilder();

            content.AppendLine(string.Format("Exception Type: {0}",  _exception.GetType().Name));
            content.AppendLine();
            content.AppendLine("Message:");
            content.AppendLine(_exception.Message);
            content.AppendLine();
            content.AppendLine("Details:");
            content.AppendLine(_exception.ToString());

            Clipboard.SetText(content.ToString());
        }

        public ICommand HideCommand { get; private set; }

        public ICommand ShowCommand { get; private set; }

        public ICommand CopyToClipboardCommand { get; private set; }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public string Details
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Details);
            }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            private set
            {
                _visibility = value;
                RaisePropertyChanged(() => Visibility);
            }
        }

       
    }
}
