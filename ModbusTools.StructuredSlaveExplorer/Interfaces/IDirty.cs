namespace ModbusTools.StructuredSlaveExplorer.Interfaces
{
    public interface IDirty
    {
        bool IsDirty { get; }

        void MarkDirty();

        void MarkClean();
    }
}
