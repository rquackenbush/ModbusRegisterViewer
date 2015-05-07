using System;
using ModbusTools.Common;
using ModbusTools.SlaveSimulator.Model.MessageFactories;

namespace ModbusTools.SlaveSimulator.Model.FunctionHandlers
{
    public class ReadInputRegistersFunctionHandler : IModbusFunctionHandler
    {
        private readonly IRegisterStorage _registerStorage;

       

        public ReadInputRegistersFunctionHandler(IRegisterStorage registerStorage)
        {
            if (registerStorage == null) throw new ArgumentNullException("registerStorage");

            _registerStorage = registerStorage;
        }

        public FunctionCode FunctionCode
        {
            get { return FunctionCode.ReadInputRegisters; }
        }

        public byte[] ProcessRequest(byte slaveId, byte[] request)
        {
            var dataAddress = request.ReadUInt16(2);
            var numberOfRegisters = request.ReadUInt16(4);

            var registers = _registerStorage.Read(dataAddress, numberOfRegisters);

            return ReadInputRegistersResponseFactory.Create(slaveId, registers);
        }
        public bool SupportsBroadcast
        {
            get { return false; }
        }
    }
}
