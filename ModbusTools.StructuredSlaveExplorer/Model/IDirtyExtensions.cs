using ModbusTools.StructuredSlaveExplorer.Interfaces;

namespace ModbusTools.StructuredSlaveExplorer.Model
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
