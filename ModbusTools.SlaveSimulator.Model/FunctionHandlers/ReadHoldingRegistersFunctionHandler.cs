using System;
using ModbusTools.Common;
using ModbusTools.SlaveSimulator.Model.MessageFactories;

namespace ModbusTools.SlaveSimulator.Model.FunctionHandlers
{
    public class ReadHoldingRegistersFunctionHandler : IModbusFunctionHandler
    {
        private readonly IRegisterStorage _registerStorage;

        public ReadHoldingRegistersFunctionHandler(IRegisterStorage registerStorage)
        {
            if (registerStorage == null) throw new ArgumentNullException("registerStorage");

            _registerStorage = registerStorage;
        }

        public FunctionCode FunctionCode
        {
            get { return FunctionCode.ReadHoldingRegisters; }
        }

        public byte[] ProcessRequest(byte slaveId, byte[] request)
        {
            var dataAddress = request.ReadUInt16(2);
            var numberOfRegisters = request.ReadUInt16(4);

            //Create the registers
            var registers = new ushort[numberOfRegisters];

            for (int index = 0; index < registers.Length; index++)
            {
                registers[index] = _registerStorage[(ushort)(dataAddress + index)];
            }

            if (slaveId != 0)
                return ReadHoldingRegistersResponseFactory.Create(slaveId, registers);

            return null;
        }

        public bool SupportsBroadcast
        {
            get { return false; }
        }
    }
}
