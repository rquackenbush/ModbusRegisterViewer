using System;

namespace ModbusTools.Capture.Model
{
    /// <summary>
    /// Represents a single sample
    /// </summary>
    public class Sample
    {
        public Sample(long ticks, Byte value)
        {
            this.Ticks = ticks;
            this.Value = value;
        }

        public long Ticks { get; private set; }
        public Byte Value { get; private set; }
    }
}