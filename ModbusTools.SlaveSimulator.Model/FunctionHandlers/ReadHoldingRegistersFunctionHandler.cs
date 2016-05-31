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
            if (registerStorage == null) throw new ArgumentNullException(nameof(registerStorage));

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

            var registers = _registerStorage.Read(dataAddress, numberOfRegisters);

            return ReadHoldingRegistersResponseFactory.Create(slaveId, registers);
        }

        public bool SupportsBroadcast
        {
            get { return false; }
        }
    }
}
