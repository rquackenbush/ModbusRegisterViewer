using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    public class PacketViewModel
    {
        private readonly PacketViewModel _previousPacket;
        private readonly string _invalidReason;
        
        private readonly MessageDirection _direction = MessageDirection.Unknown;
        private readonly Sample[] _samples;
        private readonly Lazy<byte[]> _message;

        private readonly long _offsetTicks;
        private readonly long _ticksPerMillisecond;

        private PacketViewModel(long offsetTicks, long ticksPerMillisecond, Sample[] samples)
        {
            _offsetTicks = offsetTicks;
            _ticksPerMillisecond = ticksPerMillisecond;
            _samples = samples; 

            _message = new Lazy<byte[]>(() =>
                {
                    if (_samples == null)
                        return null;

                    return _samples.Select(s => s.Value).ToArray();
                });
         }

        private PacketViewModel(long offsetTicks, long ticksPerMillisecond, Sample[] samples, MessageDirection direction, PacketViewModel previousPacket)
            : this(offsetTicks, ticksPerMillisecond, samples)
        {
            
            _direction = direction;
            _previousPacket = previousPacket;
        }

        private PacketViewModel(long offsetTicks, long ticksPerMillisecond, Sample[] samples, string invalidReason)
            : this(offsetTicks, ticksPerMillisecond, samples)
        {
            _invalidReason = invalidReason;
        }

        public static PacketViewModel CreateValidPacket(long offsetTicks, long ticksPerMillisecond, Sample[] samples, MessageDirection direction,
                                                        PacketViewModel previousPacket)
        {
            return new PacketViewModel(offsetTicks, ticksPerMillisecond, samples, direction, previousPacket);
        }

        public static PacketViewModel CreateInvalidPacket(long offsetTicks, long ticksPerMillisecond, Sample[] samples, string invalidReason)
        {
            return new PacketViewModel(offsetTicks, ticksPerMillisecond, samples, invalidReason);
        }

        public long OffsetTicks
        {
            get { return _offsetTicks; }
        }

        public long TicksPerMillisecond
        {
            get { return _ticksPerMillisecond; }
        }

        public bool IsInvalid
        {
            get { return !string.IsNullOrEmpty(this.InvalidReason); }
        }

        public string InvalidReason
        {
            get { return _invalidReason; }
        }

        internal double GetRelativeMilliseconds(long ticks)
        {
            return ((double)(ticks - _offsetTicks))/ _ticksPerMillisecond;
        }

        public double? Time 
        {
            get
            {
                if (_samples == null || _samples.Length == 0)
                    return null;

                return GetRelativeMilliseconds(_samples[0].Ticks);
            }
        }

        public PacketViewModel PreviousPacket
        {
            get { return _previousPacket; }
        }

        public byte? Function
        {
            get 
            {
                if (_samples == null || _samples.Length < 2)
                    return null;

                return _samples[1].Value; 
            }
        }

        public byte? Address
        {
            get
            {
                if (_samples == null || _samples.Length < 1)
                    return null;

                return _samples[0].Value; 
            }
        }

        public string Type
        {
            get 
            {
                if (this.IsInvalid)
                    return this.InvalidReason;

                var function = this.Function;

                if (function.HasValue)
                    return FunctionDescriptionFactory.GetFunctionDescription(function.Value);

                return "Unknown";
            }
        }

        public UInt16? CRC
        {
            get
            {
                if (this.Message == null)
                    return null;

                if (this.Message.Length < 4)
                    return null;

                return MessageUtilities.GetCRC(this.Message); 
            }
        }

        public int Bytes
        {
            get
            {
                if (_samples == null)
                    return 0;

                return _samples.Length; 
            }
        }

        public byte[] Message
        {
            get { return _message.Value; }

        }

        public double? ResponseTime
        {
            get
            {
                if (this.PreviousPacket == null)
                    return null;

                if (this.PreviousPacket.Samples == null)
                    return null;

                if (this.PreviousPacket.Samples.Length == 0)
                    return null;

                if (this.Samples == null)
                    return null;

                if (this.Samples.Length == 0)
                    return null;

                var lastRequestSample = this.PreviousPacket.Samples.Last();
                var firstResponseSample = this.Samples[0];

                return ((double)(firstResponseSample.Ticks - lastRequestSample.Ticks))/_ticksPerMillisecond;
            }
        }

        public Sample[] Samples
        {
            get { return _samples; }
        }

        public MessageDirection Direction 
        {
            get { return _direction;  }
        }

        public SampleViewModel[] SampleViewModels
        {
            get
            {
                if (this.Samples == null)
                    return null;

                var viewModels = new SampleViewModel[this.Samples.Length];

                for(int sampleIndex = 0; sampleIndex < this.Samples.Length; sampleIndex++)
                {
                    if (sampleIndex == 0)
                        viewModels[sampleIndex] = new SampleViewModel(this, this.Samples[sampleIndex], null);
                    else
                        viewModels[sampleIndex] = new SampleViewModel(this, this.Samples[sampleIndex], this.Samples[sampleIndex - 1]);
                }

                return viewModels;
            }
        }
    }
}
