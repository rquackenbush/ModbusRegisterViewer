using ModbusTools.SlaveExplorer.Interfaces;

namespace ModbusTools.SlaveExplorer.Model
{
    internal static class IDirtyExtensions
    {
        internal static void MarkDirtySafe(this IDirty dirty)
        {
            if (dirty == null)
                return;

            dirty.MarkDirty();
        }
    }
}
