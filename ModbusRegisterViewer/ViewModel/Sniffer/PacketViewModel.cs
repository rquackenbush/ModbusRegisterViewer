using System;
using System.Linq;
using ModbusRegisterViewer.Model;
using ModbusTools.Common;

namespace ModbusRegisterViewer.ViewModel.Sniffer
{
    public class PacketViewModel
    {
        
        private readonly string _invalidReason;
        
        private readonly MessageDirection _direction = MessageDirection.Unknown;
        private readonly Sample[] _samples;
        private readonly Lazy<byte[]> _message;

        private readonly CaptureTimerInfo _captureTimerInfo;

        private PacketViewModel(CaptureTimerInfo captureTimerInfo, Sample[] samples)
        {
            _captureTimerInfo = captureTimerInfo;
            _samples = samples;
            
            _message = new Lazy<byte[]>(() =>
                {
                    if (_samples == null)
                        return null;

                    return _samples.Select(s => s.Value).ToArray();
                });
         }

        private PacketViewModel(CaptureTimerInfo captureTimerInfo, Sample[] samples, MessageDirection direction)
            : this(captureTimerInfo, samples)
        {
            
            _direction = direction;
        }

        private PacketViewModel(CaptureTimerInfo captureTimerInfo, Sample[] samples, string invalidReason)
            : this(captureTimerInfo, samples)
        {
            _invalidReason = invalidReason;
        }

        public static PacketViewModel CreateValidPacket(CaptureTimerInfo captureTimerInfo, Sample[] samples, MessageDirection direction)
        {
            return new PacketViewModel(captureTimerInfo, samples, direction);
        }

        public static PacketViewModel CreateInvalidPacket(CaptureTimerInfo captureTimerInfo, Sample[] samples, string invalidReason)
        {
            return new PacketViewModel(captureTimerInfo, samples, invalidReason);
        }

        public bool IsInvalid
        {
            get { return !string.IsNullOrEmpty(this.InvalidReason); }
        }

        public string InvalidReason
        {
            get { return _invalidReason; }
        }

        public CaptureTimerInfo CaptureTimerInfo
        {
            get { return _captureTimerInfo; }
        }

        public DateTime? Time
        {
            get
            {
                //Check top see if we ahve any data
                if (_samples == null || _samples.Length == 0)
                    return null;

                //Get the ticks
                long ticks = _samples[0].Ticks;

                //Get the offset time
                return _captureTimerInfo.GetOffsetTime(ticks);
            }
        }

        public FunctionCode? Function
        {
            get 
            {
                if (_samples == null || _samples.Length < 2)
                    return null;

                return (FunctionCode )_samples[1].Value; 
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
                if (this.AssociatedRequestPacket == null)
                    return null;

                if (this.AssociatedRequestPacket.Samples == null)
                    return null;

                if (this.AssociatedRequestPacket.Samples.Length == 0)
                    return null;

                if (this.Samples == null)
                    return null;

                if (this.Samples.Length == 0)
                    return null;

                var lastRequestSample = this.AssociatedRequestPacket.Samples.Last();
                var firstResponseSample = this.Samples[0];

                var ticks = firstResponseSample.Ticks - lastRequestSample.Ticks;

                return _captureTimerInfo.TicksToMilliseconds(ticks);
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

        public PacketErrorLevel ErrorLevel
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.InvalidReason))
                    return PacketErrorLevel.Error;

                if (this.Direction == MessageDirection.Request && this.AssociatedResponsePacket == null)
                    return PacketErrorLevel.Warning;

                if (this.Direction == MessageDirection.Response && this.AssociatedRequestPacket == null)
                    return PacketErrorLevel.Error;

                return PacketErrorLevel.None;
            }
        }

        public string HoverText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.InvalidReason))
                    return this.InvalidReason;

                if (this.Direction == MessageDirection.Request && this.AssociatedResponsePacket == null)
                    return "This request has no response. This might mean that the device is not connected or responding.";

                if (this.Direction == MessageDirection.Response && this.AssociatedRequestPacket == null)
                    return "This response has no request";

                return null;
            }
        }

        public PacketViewModel AssociatedRequestPacket { get; internal set; }

        public PacketViewModel AssociatedResponsePacket { get; internal set; }
    }

    public enum PacketErrorLevel
    {
        None,

        Warning,
        
        Error
    }
}
