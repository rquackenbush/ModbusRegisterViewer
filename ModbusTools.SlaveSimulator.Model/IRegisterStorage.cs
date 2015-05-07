namespace ModbusTools.SlaveSimulator.Model
{
    public interface IRegisterStorage
    {
        ushort[] Read(ushort startingIndex, ushort numberOfRegisters);

        void Write(ushort startingIndex, ushort[] values);
    }
}