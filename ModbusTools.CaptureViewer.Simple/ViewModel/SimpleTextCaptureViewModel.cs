using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Capture.Model;

namespace ModbusTools.CaptureViewer.Simple.ViewModel
{
    public class SimpleTextCaptureViewModel : ViewModelBase
    {
        private readonly string _captureFilePath;
        private double _packetThreshold = 2.5;
        private readonly ObservableCollection<PacketViewModel> _packets = new ObservableCollection<PacketViewModel>();

        /// <summary>
        /// For designer only
        /// </summary>
        public SimpleTextCaptureViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);

            Packets.Add(new PacketViewModel(new byte[]{ 1, 2, 3, 4, 5 }));
            Packets.Add(new PacketViewModel(new byte[] { 1, 2, 3, 4, 5 }));
        }

        public SimpleTextCaptureViewModel(string captureFilePath) 
            : this()
        {
            _captureFilePath = captureFilePath;

            Refresh();           
        }

        public ICommand RefreshCommand { get; private set; }

        private void Refresh()
        {
            //var output = new StringBuilder();
            Packets.Clear();

            //Create the reader
            using (var captureReader = new CaptureFileReader(_captureFilePath))
            {
                var sample = captureReader.Read();

                //Keep track of the last sample so we now how far apart they are
                Sample lastSample = null;

                var buffer = new List<byte>();

                var captureTimerInfo = new CaptureTimerInfo(captureReader.StartTime, captureReader.TicksPerSecond);

                while (sample != null)
                {
                    if (lastSample != null)
                    {
                        //Get the elapsed Ms
                        var elapsedMs = captureTimerInfo.TicksToMilliseconds(sample.Ticks - lastSample.Ticks);

                        if (elapsedMs >= _packetThreshold)
                        {
                            Packets.Add(new PacketViewModel(buffer.ToArray()));
                            buffer.Clear();
                        }
                    }

                    //Add an item to the buffer
                    buffer.Add(sample.Value);

                    //Save this sample for next time
                    lastSample = sample;

                    //Get the next sample
                    sample = captureReader.Read();

                    //See if we have any last bits... (might take this out)
                    if (sample == null && buffer.Any())
                    {
                        Packets.Add(new PacketViewModel(buffer.ToArray()));
                        buffer.Clear();
                    }
                }
            }           
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

        public string Title
        {
            get { return "Simple Capture Viewer - " + _captureFilePath; }
        }

        public ObservableCollection<PacketViewModel> Packets
        {
            get { return _packets; }
        }
    }
}
