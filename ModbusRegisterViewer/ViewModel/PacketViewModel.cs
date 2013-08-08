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
        private readonly byte[] _message;
        private readonly PacketViewModel _previousPacket;
        private readonly string _invalidReason;
        private readonly DateTime _timestamp;
        private readonly MessageDirection _direction = MessageDirection.Unknown;

         private PacketViewModel(DateTime timetamp, byte[] message)
         {
             _timestamp = timetamp;
             _message = message; 
         }

         private PacketViewModel(DateTime timestamp, byte[] message, MessageDirection direction, PacketViewModel previousPacket)
             : this(timestamp, message)
        {
            
            _direction = direction;
            _previousPacket = previousPacket;
        }

        private PacketViewModel (DateTime timestamp, byte[] message, string invalidReason)
            : this(timestamp, message)
        {
            _invalidReason = invalidReason;
        }

        public static PacketViewModel CreateValidPacket(DateTime timestamp, byte[] message, MessageDirection direction,
                                                        PacketViewModel previousPacket)
        {
            return new PacketViewModel(timestamp, message, direction, previousPacket);
        }

        public static PacketViewModel CreateInvalidPacket(DateTime timestamp, byte[] message, string invalidReason)
        {
            return new PacketViewModel(timestamp, message, invalidReason);
        }

        public bool IsInvalid
        {
            get { return !string.IsNullOrEmpty(this.InvalidReason); }
        }

        public string InvalidReason
        {
            get { return _invalidReason; }
        }

        public DateTime Timestamp 
        {
            get { return _timestamp; }
        }

        public PacketViewModel PreviousPacket
        {
            get { return _previousPacket; }
        }

        public byte Function
        {
            get 
            {
                if (_message == null || _message.Length < 2)
                    return 0;

                return _message[1]; 
            }
        }

        public byte Address
        {
            get
            {
                if (_message == null || _message.Length < 1)
                    return 0;

                return _message[0]; 
            }
        }

        public string Type
        {
            get 
            {
                if (this.IsInvalid)
                    return this.InvalidReason;

                if (_message != null && _message.Length >= 2)
                {
                    byte functionCode = _message[1];

                    return FunctionDescriptionFactory.GetFunctionDescription(functionCode);
                }

                return "Unknown";
            }
        }

        public string Payload
        {
            get 
            { 
                if (this.IsInvalid)
                {
                    if (_message != null)
                    {
                        //Render the entire raw message
                        return BufferRender.GetDisplayString(_message);
                    }
                }
                else if (_message != null && _message.Length > 4)
                {
                    var payloadBytes = _message.Skip(2).Take(_message.Length - 4).ToArray();

                    return  BufferRender.GetDisplayString(payloadBytes);     
                }

                return null;
            }
        }

        public UInt16 CRC
        {
            get
            {
                if (_message == null)
                    return 0;

                if (_message.Length < 4)
                    return 0;

                return MessageUtilities.GetCRC(_message); 
            }
        }

        public int Bytes
        {
            get
            {
                if (_message == null)
                    return 0;

                return _message.Length; 
            }
        }

        public byte[] Message
        {
            get { return _message; }
        }

        public int? ResponseTime
        {
            get
            {
                if (this.PreviousPacket == null)
                    return null;

                return (int)this.Timestamp.Subtract(this.PreviousPacket.Timestamp).TotalMilliseconds;
            }
        }

        public MessageDirection Direction 
        {
            get { return _direction;  }
        }
    }
}
