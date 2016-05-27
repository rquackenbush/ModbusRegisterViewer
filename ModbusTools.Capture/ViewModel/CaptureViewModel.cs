using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using ModbusTools.Capture.Common;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.CaptureViewer.Simple;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.Capture.ViewModel
{
    public class CaptureViewModel : ViewModelBase
    {
        private readonly ICaptureViewerFactory[] _captureViewerFactories;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();

        private string _capturePath;
        private CaptureHost _captureHost;
        
        public CaptureViewModel()
        {
            // These are the different ways to view the capture
            _captureViewerFactories = new ICaptureViewerFactory[]
            {
                new InterpretedCaptureViewerFactory(), 
                new SimpleCaptureViewerFactory(),
            };

            _captureViewerFactory = _captureViewerFactories[0];

            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            OpenCommand = new RelayCommand(Open, CanOpen);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public ICaptureViewerFactory[] CaptureViewerFactories
        {
            get { return _captureViewerFactories; }
        }

        private ICaptureViewerFactory _captureViewerFactory;
        public ICaptureViewerFactory CaptureViewerFactory
        {
            get { return _captureViewerFactory; }
            set
            {
                _captureViewerFactory = value;
                RaisePropertyChanged();
                Refresh();
            }
        }

        private string _status = "Idle";
        public string Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        private void Refresh()
        {
            CaptureViewer = null;

            if (string.IsNullOrWhiteSpace(_capturePath))
                return;

            if (CaptureViewerFactory == null)
                return;

            CaptureViewer = CaptureViewerFactory.Open(_capturePath);
        }

        private void Open()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = CaptureConstants.CaptureFilter
            };

            if (dialog.ShowDialog() != true)
                return;

            _capturePath = dialog.FileName;

            Refresh();
        }

        private bool CanOpen()
        {
            return _captureHost == null;
        }

        private void Close()
        {

        }

        private bool CanClose()
        {
            return _captureHost == null;
        }

        private bool CanStop()
        {
            return _captureHost != null;
        }

        private void Start()
        {
            if (!ModbusAdapters.IsItemSelected)
                return;

            var dialog = new SaveFileDialog()
            {
                Filter = CaptureConstants.CaptureFilter
            };

            if (dialog.ShowDialog() != true)
                return;

            CaptureViewer = null;

            BytesReceived = 0;

            //Save the filename
            _capturePath = dialog.FileName;

            _captureHost = new CaptureHost(dialog.FileName, ModbusAdapters.GetFactory().Create());

            _captureHost.SampleReceived += OnSampleReceived;

            Status = "Capturing...";
        }

        private bool CanStart()
        {
            return _captureHost == null && ModbusAdapters.IsItemSelected;
        }

        private void Stop()
        {
            try
            {
                Status = "Idle";

                if (_captureHost != null)
                {
                    _captureHost.SampleReceived -= OnSampleReceived;
                    _captureHost.Dispose();
                    _captureHost = null;
                }

                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Source);
            }
        }

        void OnSampleReceived(object sender, EventArgs e)
        {
            BytesReceived++;
        }

        private int _bytesReceived;
        private Visual _captureViewer;

        public int BytesReceived
        {
            get { return _bytesReceived; }
            private set
            {
                _bytesReceived = value; 
                RaisePropertyChanged();
            }
        }

        public Visual CaptureViewer
        {
            get { return _captureViewer; }
            private set
            {
                _captureViewer = value; 
                RaisePropertyChanged();
            }
        }
    }
}
