using Modbus.Utility;
using ModbusRegisterViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusRegisterViewer.ViewModel.Sniffer;

namespace ModbusRegisterViewer.Model
{
    public class PacketSnifferStateMachine
    {
        private const int BufferSize = 256;

        private long? _firstTime;
        private readonly long _interMessageTimeout;
        private readonly long _ticksPerMillisecond;

        private PacketViewModel _previousPacket;
        private int _bufferPosition;
        private byte? _previousSlaveId;

        /// <summary>
        /// This is the buffer the samples get stored in until a message is produced
        /// </summary>
        private readonly Sample[] _buffer = new Sample[BufferSize];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interMessageTimeout">The timeout in between messages (milliseconds)</param>
        /// <param name="ticksPerMillisecond">Ticks per millisecond.</param>
        public PacketSnifferStateMachine(long interMessageTimeout, long ticksPerMillisecond)
        {
            _interMessageTimeout = interMessageTimeout * ticksPerMillisecond;
            _ticksPerMillisecond = ticksPerMillisecond;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="intermessageTimeout">The max amount of time inbetween messages.</param>
        /// <returns>A packet view model if a packet was created, null otherwise.</returns>
        public PacketViewModel ProcessSample(Sample sample)
        {

            if (sample == null)
                return null;

            if (_firstTime == null)
                _firstTime = sample.Ticks;

            long sampleTime = (sample.Ticks - _firstTime.Value) /_ticksPerMillisecond;

            PacketViewModel packet = null;

            //Check to see if we've gone beyond the end of the buffer
            if (_bufferPosition >= BufferSize - 2)
            {
                
                //Go back to the beginning
                _bufferPosition = 0;

                //Return an invalid packet
                packet = PacketViewModel.CreateInvalidPacket(_firstTime.Value,
                                                             _ticksPerMillisecond,
                                                             _buffer.ToArray(),
                                                             "Maximum buffer exceeded.");
            }
            else
            {
                //Check to see if we have a previous sample available
                Sample previousSample = null;

                if (_bufferPosition > 0)
                {
                    previousSample = _buffer[_bufferPosition - 1];
                }

                //Calculate the interval between this packet and the previous one
                long? interval = null;

                if (previousSample != null)
                    interval = sample.Ticks - previousSample.Ticks;

                //Has the previous message timed out?
                if (interval > _interMessageTimeout)
                {
                    packet = PacketViewModel.CreateInvalidPacket(_firstTime.Value,
                                                                 _ticksPerMillisecond,
                                                                 _buffer.Take(_bufferPosition).ToArray(),
                                                                 "Timed out");
                    
                    _bufferPosition = 0;

                }

                //Save the sample
                _buffer[_bufferPosition] = sample;

                //Increase the position
                _bufferPosition++;

                if (_bufferPosition > 6)
                {
                    //This is the entire message
                    var message = _buffer.Take(_bufferPosition).Select(s => s.Value).ToArray();

                    var currentSlaveId = MessageUtilities.GetSlaveId(message);

                    int messageSize;

                    if (_previousSlaveId.HasValue && _previousSlaveId.Value == currentSlaveId)
                    {
                        messageSize = RequestSizeCalculator.GetResponseMessageLength(message);
                    }
                    else
                    {
                        //In case the message was lost - just start over
                        _previousSlaveId = null;
                        _previousPacket = null;

                        messageSize = RequestSizeCalculator.GetRequestMessageLength(message);
                    }

                    //Check to see if this is supposed to be completed
                    if (_bufferPosition >= messageSize)
                    {
                        //It has been too long in between messages - be done with the previous message
                        var messageFrame = _buffer.Take(_bufferPosition - 2).Select(s => s.Value).ToArray();

                        //Calculate the CRC with the given set of bytes
                        var calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(messageFrame), 0);

                        //Get the crc that is stored in the message
                        var messageCrc = MessageUtilities.GetCRC(message);

                        //Check to see if the crc's match
                        if (calculatedCrc == messageCrc)
                        {
                            //Determine the diection
                            var direction = _previousSlaveId.HasValue
                                                ? MessageDirection.Response
                                                : MessageDirection.Request;

                            //We have a good messsage
                            packet = PacketViewModel.CreateValidPacket(_firstTime.Value,
                                                                       _ticksPerMillisecond,
                                                                       _buffer.Take(_bufferPosition).ToArray(),
                                                                       direction,
                                                                       _previousPacket);


                            if (direction == MessageDirection.Request)
                            {
                                //Save this address 
                                _previousSlaveId = MessageUtilities.GetSlaveId(message);

                                _previousPacket = packet;
                            }
                            else
                            {
                                _previousSlaveId = null;
                                _previousPacket = null;
                            }
                        }
                        else
                        {
                            packet = PacketViewModel.CreateInvalidPacket(_firstTime.Value,
                                                                         _ticksPerMillisecond,
                                                                         _buffer.Take(_bufferPosition).ToArray(),
                                                                         "Invalid CRC");

                            _previousSlaveId = null;
                            _previousPacket = null;
                        }

                        _bufferPosition = 0;
                    }
                }
            }


            return packet;

            
        }

    }
}
