using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using ModbusTools.Capture.Model;

namespace ModbusTools.CaptureViewer.Interpreted.ViewModel
{
    public class InterpretedCaptureViewModel : ViewModelBase
    {
        private readonly string _captureFilePath;
        private double _packetThreshold = 2.5;
        private PacketViewModel[] _packets;
        private PacketViewModel _selectedPacket;

        /// <summary>
        /// For designer
        /// </summary>
        public InterpretedCaptureViewModel()
        {
            _packets = new PacketViewModel[] 
                { new PacketViewModel(new Sample[]
                    {
                        new Sample(40000, 60), 
                        new Sample(40010, 5), 
                        new Sample(40010, 20), 
                    })
                };
        }

        public InterpretedCaptureViewModel(string captureFilePath)
        {
            _captureFilePath = captureFilePath;

            Refresh();
        }

        private void Refresh()
        {
            //var output = new StringBuilder();
            var packets = new List<PacketViewModel>();

            //Create the reader
            using (var captureReader = new CaptureFileReader(_captureFilePath))
            {
                var sample = captureReader.Read();

                //Keep track of the last sample so we now how far apart they are
                Sample lastSample = null;

                var buffer = new List<Sample>();

                var captureTimerInfo = new CaptureTimerInfo(captureReader.StartTime, captureReader.TicksPerSecond);

                while (sample != null)
                {
                    if (lastSample != null)
                    {
                        //Get the elapsed Ms
                        var elapsedMs = captureTimerInfo.TicksToMilliseconds(sample.Ticks - lastSample.Ticks);

                        if (elapsedMs >= _packetThreshold)
                        {
                            packets.Add(new PacketViewModel(buffer.ToArray()));
                            buffer.Clear();
                        }
                    }

                    //Add an item to the buffer
                    buffer.Add(sample);

                    //Save this sample for next time
                    lastSample = sample;

                    //Get the next sample
                    sample = captureReader.Read();

                    //See if we have any last bits... (might take this out)
                    if (sample == null && buffer.Any())
                    {
                        packets.Add(new PacketViewModel(buffer.ToArray()));
                        buffer.Clear();
                    }
                }

            }

            //Ditch the first one if it has an error
            if (packets[0].HasError)
            {
                packets.RemoveAt(0);
            }

            Packets = packets.ToArray();
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
            get { return "Interpreted Capture Viewer - " + _captureFilePath; }
        }

        public PacketViewModel[] Packets
        {
            get { return _packets; }
            private set
            {
                _packets = value; 
                RaisePropertyChanged();
            }
        }

        public PacketViewModel SelectedPacket
        {
            get { return _selectedPacket; }
            set
            {
                _selectedPacket = value; 
                RaisePropertyChanged();
            }
        }
    }
}