using NModbus;
using NModbus.IO;

namespace ModbusTools.Common
{
    public interface IMasterContextFactory
    {
        IMasterContext Create();

        IStreamResource CreateStreamResource();

        IModbusSlaveNetwork CreateSlaveNetwork();

        IModbusMaster CreateMaster();
    }
}
