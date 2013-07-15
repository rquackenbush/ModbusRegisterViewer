using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace ModbusRegisterViewer.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private Visibility _visibility;

        public AboutViewModel()
        {
            this.ShowCommand = new RelayCommand(Show);
            this.HideCommand = new RelayCommand(Hide);

            _visibility = Visibility.Collapsed;
        }

        private void Show()
        {
            this.Visibility = Visibility.Visible;            
        }

        private void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public ICommand ShowCommand { get; private set; }
        public ICommand HideCommand { get; private set; }

        public Visibility Visibility
        {
            get { return _visibility; }
            private set
            {
                _visibility = value;
                RaisePropertyChanged(() => Visibility);
            }
        }

        public string Version
        {
            get { return this.GetType().Assembly.GetName().Version.ToString(); }
        }
    }
}
