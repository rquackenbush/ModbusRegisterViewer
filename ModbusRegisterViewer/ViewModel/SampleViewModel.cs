using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    public class SampleViewModel
    {
        private readonly Sample _previousSample;
        private readonly Sample _sample;

        public SampleViewModel(Sample sample, Sample previousSample)
        {
            _sample = sample;
            _previousSample = previousSample;
        }

        public long Time
        {
            get { return _sample.Ticks; }
        }

        public byte Value
        {
            get { return _sample.Value; }
        }

        public string Hex
        {
            get { return string.Format("0x{0:x2}", this.Value); }
        }

        public string Binary
        {
            get { return Convert.ToString(this.Value, 2).PadLeft(8, '0'); }
        }

        public long? Interval
        {
            get
            {
                if (_previousSample == null)
                    return null;

                return _sample.Ticks - _previousSample.Ticks;
            }
        }
    }
}
