using ModbusTools.Common;

namespace ModbusTools.SlaveSimulator.Model
{
    public interface IModbusFunctionHandler
    {
        FunctionCode FunctionCode { get; }

        byte[] ProcessRequest(byte slaveId, byte[] request);

        bool SupportsBroadcast { get; }
    }
}
