using NModbus;

namespace ModbusTools.Common
{
    public interface IMasterContextFactory
    {
        IMasterContext Create();

        IModbusSlaveNetwork CreateSlaveNetwork();

        IModbusMaster CreateMaster();
    }
}
