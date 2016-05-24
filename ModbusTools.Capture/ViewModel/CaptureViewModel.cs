using System;
using System.Linq;
using System.Windows.Input;
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
        private readonly OpenActionViewModel[] _openActions;
        private readonly CaptureCompletedActionViewModel[] _captureCompletedActions;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();

        private string _capturePath;
        private CaptureHost _captureHost;
        
        public CaptureViewModel()
        {
            // These are the different ways to view the capture
            var captureViewerFactories = new ICaptureViewerFactory[]
            {
                new InterpretedCaptureViewerFactory(), 
                new SimpleCaptureViewerFactory(),
            };

            //These are the ways a user can explicitly open a capture file
            _openActions = captureViewerFactories.Select(f => new OpenActionViewModel(f)).ToArray();

            //These are the actions that can take place after a capture is complete.
            _captureCompletedActions = new []
            {
                new CaptureCompletedActionViewModel("<None>", null), 
            }.Concat(captureViewerFactories.Select(f => new CaptureCompletedActionViewModel(f.Name, f))).ToArray();

            CaptureCompletedAction = _captureCompletedActions[1];

            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public OpenActionViewModel[] OpenActions
        {
            get { return _openActions; }
        }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public CaptureCompletedActionViewModel[] CaptureCompletedActions
        {
            get { return _captureCompletedActions; }
        }

        private CaptureCompletedActionViewModel _captureCompleted;
        public CaptureCompletedActionViewModel CaptureCompletedAction
        {
            get { return _captureCompleted; }
            set
            {
                _captureCompleted = value;
                RaisePropertyChanged();
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
                }

                if (!string.IsNullOrWhiteSpace(_capturePath))
                {
                    if (CaptureCompletedAction != null && CaptureCompletedAction.Factory != null)
                    {
                        CaptureCompletedAction.Factory.ShowCapture(_capturePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void OnSampleReceived(object sender, EventArgs e)
        {
            BytesReceived++;
        }

        private int _bytesReceived;
        public int BytesReceived
        {
            get { return _bytesReceived; }
            private set
            {
                _bytesReceived = value; 
                RaisePropertyChanged();
            }
        }

    }
}
