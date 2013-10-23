using System;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel.Sniffer
{
    public class SampleViewModel
    {
        private readonly PacketViewModel _packet;
        private readonly Sample _previousSample;
        private readonly Sample _sample;

        public SampleViewModel(PacketViewModel packet, Sample sample, Sample previousSample)
        {
            _packet = packet;
            _sample = sample;
            _previousSample = previousSample;
        }

        public double Time
        {
            get { return _packet.GetRelativeMilliseconds(_sample.Ticks); }
        }

        public byte Value
        {
            get { return _sample.Value; }
        }

        public long Ticks
        {
            get { return _sample.Ticks; }
        }

        public string Hex
        {
            get { return string.Format("0x{0:x2}", this.Value); }
        }

        public string Binary
        {
            get { return Convert.ToString(this.Value, 2).PadLeft(8, '0'); }
        }

        public double? Interval
        {
            get
            {
                if (_previousSample == null)
                    return null;

                return ((double)(_sample.Ticks - _previousSample.Ticks)) / _packet.TicksPerMillisecond;
            }
        }
    }
}
