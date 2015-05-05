using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Modbus.IO;
using ModbusTools.Capture.Model;
using ModbusTools.Capture.View;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.Capture.ViewModel
{
    public class CaptureViewModel : ViewModelBase
    {
        private readonly OpenActionViewModel[] _openActions;
        private readonly CaptureCompletedActionViewModel[] _captureCompletedActions;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();

        private PromiscuousListener _listener;
        private string _capturePath;
        private CaptureFileWriter _writer;
        private IStreamResource _port;

        private Task _task;

        private const string CaptureFilter = "Modbus Capture Files(*.mbcap)|*.mbcap";

        public CaptureViewModel()
        {
            // These are the different ways to view the capture
            var captureViewerFactories = new []
            {
                new CaptureViewerFactory("Simple Text", path =>
                {
                    var viewModel = new SimpleTextCaptureViewModel(path);

                    return new SimpleTextCaptureView()
                    {
                        DataContext = viewModel
                    };

                }), 
            };

            //These are the ways a user can explicitly open a capture file
            _openActions = captureViewerFactories.Select(f => new OpenActionViewModel(f)).ToArray();

            //These are the actions that can take place after a capture is complete.
            _captureCompletedActions = new []
            {
                new CaptureCompletedActionViewModel("<None>", null), 
            }.Concat(captureViewerFactories.Select(f => new CaptureCompletedActionViewModel(f.Name, f))).ToArray();

            CaptureCompletedAction = _captureCompletedActions.FirstOrDefault();

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
            return _task == null;
        }

        private void Close()
        {

        }

        private bool CanClose()
        {
            return _task == null;
        }

        private bool CanStop()
        {
            return _task != null;
        }

        private void Start()
        {
            if (!ModbusAdapters.IsItemSelected)
                return;

            var dialog = new SaveFileDialog()
            {
                Filter = CaptureFilter
            };

            if (dialog.ShowDialog() != true)
                return;

            BytesReceived = 0;

            //Save the filename
            _capturePath = dialog.FileName;

            //Try to create the file first
            _writer = new CaptureFileWriter(dialog.FileName);

            Status = "Capturing...";

            //Spin up the listener in its own thread
            _task = new Task(() =>
            {
                try
                {
                    using (var master = ModbusAdapters.GetFactory().Create())
                    {
                        _port = master.Master.Transport.GetStreamResource();

                        _listener = new PromiscuousListener(_port);

                        _listener.Sample += OnSample;

                        _listener.Listen();
                    }
                }
                catch (Exception ex)
                {
                    //This will exception out when the port is killed
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    _task = null;

                    _listener.Sample -= OnSample;

                    _listener = null;
                }
            });

            _task.Start();
        }

        private bool CanStart()
        {
            return _task == null && ModbusAdapters.IsItemSelected;
        }

        private void Stop()
        {
            try
            {
                Status = "Idle";

                if (_listener != null)
                {
                    //When a message is received
                    _listener.Sample -= OnSample;
                    _listener.Dispose();
                }

                if (_port != null)
                {
                    //if (_port.IsOpen)
                    //{
                    //_port.DiscardInBuffer();
                    //}

                    _port.Dispose();
                }

                if (_writer != null)
                {
                    _writer.Dispose();
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

        void OnSample(object sender, SampleEventArgs e)
        {
            if (_writer != null)
            {
                _writer.WriteSample(e.Sample);
            }

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
