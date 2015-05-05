namespace ModbusTools.SlaveSimulator.Model
{
    public interface ISlave
    {
        byte SlaveId { get; }

        byte[] ProcessRequest(byte[] request);
    }
}
