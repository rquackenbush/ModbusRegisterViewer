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
        private readonly long _time;
        private readonly MessageDirection _direction = MessageDirection.Unknown;
        private readonly Sample[] _samples;
        private readonly Lazy<byte[]> _message;

        private PacketViewModel(long time, Sample[] samples)
         {
             _time = time;
             _samples = samples; 

            _message = new Lazy<byte[]>(() =>
                {
                    if (_samples == null)
                        return null;

                    return _samples.Select(s => s.Value).ToArray();
                });
         }

        private PacketViewModel(long time, Sample[] samples, MessageDirection direction, PacketViewModel previousPacket)
            : this(time, samples)
        {
            
            _direction = direction;
            _previousPacket = previousPacket;
        }

        private PacketViewModel(long time, Sample[] samples, string invalidReason)
            : this(time, samples)
        {
            _invalidReason = invalidReason;
        }

        public static PacketViewModel CreateValidPacket(long time, Sample[] samples, MessageDirection direction,
                                                        PacketViewModel previousPacket)
        {
            return new PacketViewModel(time, samples, direction, previousPacket);
        }

        public static PacketViewModel CreateInvalidPacket(long time, Sample[] samples, string invalidReason)
        {
            return new PacketViewModel(time, samples, invalidReason);
        }

        public bool IsInvalid
        {
            get { return !string.IsNullOrEmpty(this.InvalidReason); }
        }

        public string InvalidReason
        {
            get { return _invalidReason; }
        }

        public long Time 
        {
            get { return _time; }
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

        //public string Payload
        //{
        //    get 
        //    { 
        //        if (this.IsInvalid)
        //        {
        //            if (_message != null)
        //            {
        //                //Render the entire raw message
        //                return BufferRender.GetDisplayString(_message);
        //            }
        //        }
        //        else if (_message != null && _message.Length > 4)
        //        {
        //            var payloadBytes = _message.Skip(2).Take(_message.Length - 4).ToArray();

        //            return  BufferRender.GetDisplayString(payloadBytes);     
        //        }

        //        return null;
        //    }
        //}


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

        //public byte[] Message
        //{
        //    get { return _message; }
        //}

        public long? ResponseTime
        {
            get
            {
                if (this.PreviousPacket == null)
                    return null;

                return this.Time - this.PreviousPacket.Time;
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
    }
}
