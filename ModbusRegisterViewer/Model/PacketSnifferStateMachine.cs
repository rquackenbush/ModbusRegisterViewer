using Modbus.Utility;
using ModbusRegisterViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public class PacketSnifferStateMachine
    {
        private readonly Dictionary<byte, SlaveStateMachine> _slaves = new Dictionary<byte, SlaveStateMachine>();

        public PacketViewModel ProcessMessage(byte[] message, DateTime timestamp)
        {
            if (message == null || message.Length == 0)
                return null;

            SlaveStateMachine slave = null;

            var slaveId = MessageUtilities.GetSlaveId(message);

            if (!_slaves.TryGetValue(slaveId, out slave))
            {
                slave = new SlaveStateMachine(slaveId);

                _slaves.Add(slaveId, slave);
            }

            return slave.ProcessMessage(message, timestamp);
        }
    }

    public class SlaveStateMachine
    {
        private readonly byte _slaveId;
        private SlaveState _state = SlaveState.Idle;

        private PacketViewModel _previousPacket;

        private enum SlaveState
        {
            /// <summary>
            /// Nothing - we're just waiting for some packets.
            /// </summary>
            Idle,

            /// <summary>
            /// We've recieved a request and we're waiting for a response
            /// </summary>
            AwaitingResponse,
        }

        public SlaveStateMachine(byte slaveId)
        {
            _slaveId = slaveId;
        }

        public PacketViewModel ProcessMessage(byte[] message, DateTime timestamp)
        {
            //Get the crc of this message
            ushort currentCrc = MessageUtilities.GetCRC(message);
            ushort currentFunction = MessageUtilities.GetFunction(message);

            var direction = MessageDirection.Unknown;

            if (_state == SlaveState.Idle || ( _previousPacket != null && (currentFunction != _previousPacket.Function || currentCrc == _previousPacket.CRC)))
            {
                //We've gotten the request - now wait for the response
                _state = SlaveState.AwaitingResponse;
            
                //This is a request
                direction = MessageDirection.Request;
            }
            else
            {
                //This appears to be a valid response
                direction = MessageDirection.Response;

                //Let's away another request
                _state = SlaveState.Idle;
            }

            //Create the packet
            var packet = PacketViewModel.CreateValidPacket(timestamp, message, direction, direction == MessageDirection.Response ? _previousPacket : null);

            //Create the packet
            _previousPacket = packet;

            //Return a new view model
            return _previousPacket;
        }

        public byte SlaveId
        {
            get { return _slaveId; }
        }
    }
}
