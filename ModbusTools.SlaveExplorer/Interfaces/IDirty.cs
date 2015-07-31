namespace ModbusTools.SlaveExplorer.Interfaces
{
    public interface IDirty
    {
        bool IsDirty { get; }

        void MarkDirty();

        void MarkClean();
    }
}
