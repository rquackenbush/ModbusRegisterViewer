using System;
using System.Collections.Generic;
using System.Linq;
using ModbusRegisterViewer.ViewModel.Sniffer;

namespace ModbusRegisterViewer.Model.Sniffer
{
    public class ParallelPacketHandler : IPacketSnifferStateMachine
    {
        private const int MaxBufferSize = 256;

        private readonly CaptureTimerInfo _captureTimerInfo;
        private readonly double _interMessageTimeout;

        private PacketViewModel _previousPacket;

        private readonly PacketHandlerStrategy[] _packetHandlerStrategies;

        private readonly List<Sample> _buffer = new List<Sample>(MaxBufferSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interMessageTimeout">The time in milliseconds that it takes for a message to timeout.</param>
        /// <param name="ticksPerSecond"></param>
        /// <param name="startTime"></param>
        public ParallelPacketHandler(double interMessageTimeout, long ticksPerSecond, DateTime startTime)
        {
            _interMessageTimeout = interMessageTimeout;

            _captureTimerInfo = new CaptureTimerInfo(startTime, ticksPerSecond);

            PacketHandlerSharedState sharedState = new PacketHandlerSharedState(_captureTimerInfo);

            _packetHandlerStrategies = new PacketHandlerStrategy[] 
            {
                new RequestPacketHandlerStrategy(sharedState),
                new ResponsePacketHandlerStrategy(sharedState)
            };
        }

        private void Reset()
        {
            _previousPacket = null;

            _buffer.Clear();

            foreach(var s in _packetHandlerStrategies)
            {
                s.Reset();
            }
        }

        public PacketViewModel ProcessSample(Sample sample)
        {
            PacketViewModel packet = null;

            if (sample == null)
                return null;

            //Check to see if we're too large here
            if (_buffer.Count >= MaxBufferSize)
            {
                //Return an invalid packet
                packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo,
                                                             _buffer.ToArray(),
                                                             "Maximum buffer exceeded.");
                //Reset it!
                Reset();
            }

            //Check to see if we have a previous sample available
            Sample previousSample = null;

            if (_buffer.Count > 0)
            {
                previousSample = _buffer[_buffer.Count - 1];
            }

            //Calculate the interval between this packet and the previous one
            double? interval = null;

            //Calculate the interval
            if (previousSample != null)
                interval = _captureTimerInfo.TicksToMilliseconds(sample.Ticks - previousSample.Ticks);

            //Has the previous message timed out?
            if (interval > _interMessageTimeout)
            {
                packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo,
                                                             _buffer.ToArray(),
                                                             "Timed out");
                Reset();
            }

            //Add the sample
            _buffer.Add(sample);

            //Check to see if we already have a packet
            if (packet == null)
            {
                //Get a copy of the buffer
                var buffer = _buffer.ToArray();

                //Execute all of the strategies
                foreach (var strategy in _packetHandlerStrategies.Where(s => !s.IsFaulted))
                {
                    var packetFromStrategy = strategy.Process(buffer);

                    if (packetFromStrategy != null)
                    {
                        //Check to see if these packets are related
                        if (packetFromStrategy.Direction == MessageDirection.Response 
                            && _previousPacket != null
                            && _previousPacket.Address == packetFromStrategy.Address
                            && _previousPacket.Direction == MessageDirection.Request
                            && _previousPacket.Function == packetFromStrategy.Function)
                        {
                            _previousPacket.AssociatedResponsePacket = packetFromStrategy;
                            packetFromStrategy.AssociatedRequestPacket = _previousPacket;
                        }

                        //Reset it!
                        Reset();

                        //Save this packet
                        _previousPacket = packetFromStrategy;

                        //Yay!
                        return packetFromStrategy;
                    }
                }

                //Check to see if all of the strategies are faulted
                if (_packetHandlerStrategies.All(s => s.IsFaulted))
                {
                    //No good - start over
                    Reset();

                    //Create an invalid packet
                    packet = PacketViewModel.CreateInvalidPacket(_captureTimerInfo, buffer, "Unable to parse packet");
                }
            }
 
            return packet;
        }
    }

    internal class PacketHandlerSharedState
    {
        private readonly CaptureTimerInfo _captureTimerInfo;

        public PacketHandlerSharedState(CaptureTimerInfo captureTimerInfo)
        {
            _captureTimerInfo = captureTimerInfo;
        }

        public CaptureTimerInfo CaptureTimerInfo
        {
            get { return _captureTimerInfo; }
        }
    }

    internal class RequestPacketHandlerStrategy : PacketHandlerStrategy
    {
        public RequestPacketHandlerStrategy(PacketHandlerSharedState sharedState)
            : base(sharedState)
        {
        }
       
        protected override int? GetMessageLength(byte[] message)
        {
            return MessageSizeCalculator.GetRequestMessageLength(message);
        }

        protected override MessageDirection MessageDirection
        {
            get { return MessageDirection.Request; }
        }
    }

    internal class ResponsePacketHandlerStrategy : PacketHandlerStrategy
    {
        public ResponsePacketHandlerStrategy(PacketHandlerSharedState sharedState)
            : base(sharedState)
        {
        }
       
        protected override int? GetMessageLength(byte[] message)
        {
            return MessageSizeCalculator.GetResponseMessageLength(message);
        }

        protected override MessageDirection MessageDirection
        {
            get { return MessageDirection.Response; }
        }
    }

    internal abstract class PacketHandlerStrategy
    {
        private readonly PacketHandlerSharedState _sharedState;
        
        internal PacketHandlerStrategy(PacketHandlerSharedState sharedState)
        {
            _sharedState = sharedState;
        }

        protected abstract MessageDirection MessageDirection { get; }

        protected abstract int? GetMessageLength(byte[] message);

        public PacketViewModel Process(Sample[] buffer)
        {
            //Check to see if we have enough data to calculate the size
            if (buffer.Length > 6)
            {
                //Get the raw bytes
                var message = buffer.Select(s => s.Value).ToArray();

                //Get the message length
                var messageLength = this.GetMessageLength(message);

                if (messageLength == null)
                {
                    this.Faulted();
                }
                else if (message.Length >= messageLength)
                {
                    ///Check to see if the crc's match
                    if (CrcComparer.DoesCrcMatch(message))
                    {
                        //They did, so we're good to go.
                        return PacketViewModel.CreateValidPacket(this.SharedState.CaptureTimerInfo, buffer, this.MessageDirection);
                    }
                    else
                    {
                        //TODO: Save some sort of error message for later
                        this.Faulted();                            
                    }
                }
            }

            return null;
        }
        public bool IsFaulted { get; private set; }

        protected PacketHandlerSharedState SharedState
        {
            get { return _sharedState; }
        }

        //Marks this item as faulted
        protected void Faulted()
        {
            this.IsFaulted = true;
        }

        internal virtual void Reset()
        {
            IsFaulted = false;
        }
    }

   
}
