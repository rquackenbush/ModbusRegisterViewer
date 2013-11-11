//using Modbus.Utility;
//using ModbusRegisterViewer.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ModbusRegisterViewer.ViewModel.Sniffer;

//namespace ModbusRegisterViewer.Model
//{
//    public class PacketSnifferStateMachine
//    {
//        private const int BufferSize = 256;

//        private readonly CaptureTimerInfo _captureTimerInfo;
//        private readonly double _interMessageTimeout;

//        private PacketViewModel _previousPacket;
//        private int _bufferPosition;
//        private byte? _previousSlaveId;

//        /// <summary>
//        /// This is the buffer the samples get stored in until a message is produced
//        /// </summary>
//        private readonly Sample[] _buffer = new Sample[BufferSize];

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="interMessageTimeout">The time in milliseconds that it takes for a message to timeout.</param>
//        /// <param name="ticksPerSecond"></param>
//        /// <param name="startTime"></param>
//        public PacketSnifferStateMachine(double interMessageTimeout, long ticksPerSecond, DateTime startTime)
//        {
//            _interMessageTimeout = interMessageTimeout;

//            _captureTimerInfo = new CaptureTimerInfo(startTime, ticksPerSecond);
//        }
        
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sample"></param>
//        /// <param name="intermessageTimeout">The max amount of time inbetween messages.</param>
//        /// <returns>A packet view model if a packet was created, null otherwise.</returns>
//        public PacketViewModel ProcessSample(Sample sample)
//        {
//            if (sample == null)
//                return null;

//            PacketViewModel packet = null;

//            //Check to see if we've gone beyond the end of the buffer
//            if (_bufferPosition >= BufferSize - 2)
//            {
                
//                //Go back to the beginning
//                _bufferPosition = 0;

//                //Return an invalid packet
//                packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo,
//                                                             _buffer.ToArray(),
//                                                             "Maximum buffer exceeded.");
//            }
//            else
//            {
//                //Check to see if we have a previous sample available
//                Sample previousSample = null;

//                if (_bufferPosition > 0)
//                {
//                    previousSample = _buffer[_bufferPosition - 1];
//                }

//                //Calculate the interval between this packet and the previous one
//                double? interval = null;

//                if (previousSample != null)
//                    interval = _captureTimerInfo.TicksToMilliseconds(sample.Ticks - previousSample.Ticks);

//                //Has the previous message timed out?
//                if (interval > _interMessageTimeout)
//                {
//                    packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo,
//                                                                 _buffer.Take(_bufferPosition).ToArray(),
//                                                                 "Timed out");
//                    _bufferPosition = 0;
//                }

//                //Save the sample
//                _buffer[_bufferPosition] = sample;

//                //Increase the position
//                _bufferPosition++;

//                if (_bufferPosition > 6)
//                {
//                    //This is the entire message
//                    var message = _buffer.Take(_bufferPosition).Select(s => s.Value).ToArray();

//                    var currentSlaveId = MessageUtilities.GetSlaveId(message);

//                    int messageSize;

//                    try
//                    {
//                        //Check to see if we're already part of a communication
//                        if (_previousSlaveId.HasValue && _previousSlaveId.Value == currentSlaveId)
//                        {
//                            //Try to get the response size
//                            messageSize = MessageSizeCalculator.GetResponseMessageLength(message);
//                        }
//                        else
//                        {
//                            //TODO: Detect if there is a missing response????

//                            //This is a new bloody packet
//                            _previousPacket = null;
//                            _previousSlaveId = null;

//                            //Assume that this is a new request
//                            messageSize = MessageSizeCalculator.GetMessageLength(message, MessageDirection.Request);

//                        }
//                    }
//                    catch(NotImplementedException)
//                    {
//                        _previousPacket = null;
//                        _previousSlaveId = null;

//                        //This doesn't make any sense
//                        packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo,
//                                                                     _buffer.Take(_bufferPosition).ToArray(),
//                                                                     "Unable to determine packet length");

//                        _bufferPosition = 0;

//                        return packet;
//                    }

//                    //Check to see if this is supposed to be completed
//                    if (_bufferPosition >= messageSize)
//                    {
//                        //It has been too long in between messages - be done with the previous message
//                        var messageFrame = _buffer.Take(_bufferPosition - 2).Select(s => s.Value).ToArray();

//                        //Calculate the CRC with the given set of bytes
//                        var calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(messageFrame), 0);

//                        //Get the crc that is stored in the message
//                        var messageCrc = MessageUtilities.GetCRC(message);

//                        //Check to see if the crc's match
//                        if (calculatedCrc == messageCrc)
//                        {
//                            //Determine the diection
//                            var direction = _previousSlaveId.HasValue
//                                                ? MessageDirection.Response
//                                                : MessageDirection.Request;

//                            //We have a good messsage
//                            packet = PacketViewModel.CreateValidPacket(_captureTimerInfo,
//                                                                       _buffer.Take(_bufferPosition).ToArray(),
//                                                                       direction,
//                                                                       _previousPacket);


//                            if (direction == MessageDirection.Request)
//                            {
//                                //Save this address 
//                                _previousSlaveId = MessageUtilities.GetSlaveId(message);

//                                _previousPacket = packet;
//                            }
//                            else
//                            {
//                                _previousSlaveId = null;
//                                _previousPacket = null;
//                            }
//                        }
//                        else
//                        {
//                            //The CRC did not match.  Start over.
//                            packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo,
//                                                                         _buffer.Take(_bufferPosition).ToArray(),
//                                                                         "Invalid CRC");

//                            _previousSlaveId = null;
//                            _previousPacket = null;
//                        }

//                        _bufferPosition = 0;
//                    }
//                }
//            }


//            return packet;

            
//        }

//    }
//}
