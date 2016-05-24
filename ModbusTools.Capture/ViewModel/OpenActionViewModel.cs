using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using ModbusTools.Capture.Common;
using ModbusTools.Capture.Model;

namespace ModbusTools.Capture.ViewModel
{
    public class OpenActionViewModel : ViewModelBase
    {
        private readonly ICaptureViewerFactory _factory;

        public OpenActionViewModel(ICaptureViewerFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            _factory = factory;

            OpenCommand = new RelayCommand(Open);
        }

        public string Name
        {
            get { return _factory.Name; }
        }

        public ICommand OpenCommand { get; private set; }

        private void Open()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = CaptureConstants.CaptureFilter
            };

            if (dialog.ShowDialog() == true)
            {
                _factory.ShowCapture(dialog.FileName);
            }
        }
    }
}
