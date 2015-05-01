using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Capture.Model;

namespace ModbusTools.Capture.ViewModel
{
    public class SimpleTextCaptureViewModel : ViewModelBase
    {
        private readonly string _captureFilePath;
        private string _output;
        private double _packetThreshold = 16.0;

        public SimpleTextCaptureViewModel(string captureFilePath)
        {
            _captureFilePath = captureFilePath;

            Refresh();

            RefreshCommand = new RelayCommand(Refresh);
        }

        public ICommand RefreshCommand { get; private set; }

        private void Refresh()
        {
            var output = new StringBuilder();

            //Create the reader
            using (var captureReader = new CaptureFileReader(_captureFilePath))
            {
                var sample = captureReader.Read();
                Sample lastSample = null;

                var captureTimerInfo = new CaptureTimerInfo(captureReader.StartTime, captureReader.TicksPerSecond);

                while (sample != null)
                {
                    if (lastSample != null)
                    {
                        //Get the elapsed Ms
                        var elapsedMs = captureTimerInfo.TicksToMilliseconds(sample.Ticks - lastSample.Ticks);

                        if (elapsedMs >= _packetThreshold)
                        {
                            output.AppendLine();
                            output.AppendLine();
                        }
                    }

                    output.AppendFormat("{0:X2} ", sample.Value);

                    //Save this sample for next time
                    lastSample = sample;

                    //Get the next sample
                    sample = captureReader.Read();
                }
            }

            Output = output.ToString();
        }

        public double PacketThreshold
        {
            get { return _packetThreshold; }
            set
            {
                _packetThreshold = value;
                RaisePropertyChanged();
            }
        }

        public string Output
        {
            get { return _output; }
            set
            {
                _output = value; 
                RaisePropertyChanged();
            }
        }

        public string Title
        {
            get { return "Simple Capture Viewer - " + _captureFilePath; }
        }
    }
}
