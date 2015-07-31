using System.Windows.Media;

namespace ModbusTools.SlaveExplorer.Interfaces
{
    public interface IRuntimeFieldEditor
    {
        string Name { get; }

        Visual Visual { get; }
    }
}
