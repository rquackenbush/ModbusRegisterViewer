namespace ModbusTools.SlaveSimulator.Model
{
    public interface IRegisterStorage
    {
        ushort this[ushort registerIndex] { get; set; }
    }
}