using System.Windows.Media;

namespace ModbusTools.StructuredSlaveExplorer.Interfaces
{
    public interface IRuntimeFieldEditor
    {
        string Name { get; }

        Visual Visual { get; }
    }
}
