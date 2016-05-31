namespace ModbusTools.StructuredSlaveExplorer.Interfaces
{
    public interface IRuntimeField
    {
        int Offset { get; }

        int Size { get; }

        void SetBytes(byte[] data);

        byte[] GetBytes();

        IRuntimeFieldEditor[] FieldEditors { get; }
    }
}
