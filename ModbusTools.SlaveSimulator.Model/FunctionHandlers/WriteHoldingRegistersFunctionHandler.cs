using System;
using ModbusTools.Common;
using ModbusTools.SlaveSimulator.Model.MessageFactories;

namespace ModbusTools.SlaveSimulator.Model.FunctionHandlers
{
    public class WriteHoldingRegistersFunctionHandler : IModbusFunctionHandler
    {
        private readonly IRegisterStorage _registerStorage;

        private static class RequestOffsets
        {
            public const int StartingAddress = 2;
            public const int NumberOfRegisters = 4;
            public const int Registers = 7;
        }

        public WriteHoldingRegistersFunctionHandler(IRegisterStorage registerStorage)
        {
            if (registerStorage == null) throw new ArgumentNullException("registerStorage");

            _registerStorage = registerStorage;
        }

        public FunctionCode FunctionCode
        {
            get { return FunctionCode.WriteMultipleRegisters; }
        }

        public byte[] ProcessRequest(byte slaveId, byte[] request)
        {
            var startingAddress = request.ReadUInt16(RequestOffsets.StartingAddress);
            var numberOfRegisters = request.ReadUInt16(RequestOffsets.NumberOfRegisters);

            //Create a place to put the registers
            var registers = new ushort[numberOfRegisters];

            for (int index = 0; index < numberOfRegisters; index++)
            {
                var registerOffsetInRequest = RequestOffsets.Registers + (2*index);

                registers[index] = request.ReadUInt16(registerOffsetInRequest, false);
            }

            //Write to storage
            _registerStorage.Write(startingAddress, registers);

            return WriteHoldingRegistersResponseFactory.Create(slaveId, startingAddress, numberOfRegisters);
        }

        public bool SupportsBroadcast
        {
            get { return true; }
        }
    }
}
