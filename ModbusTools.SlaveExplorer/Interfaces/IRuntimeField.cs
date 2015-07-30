using System.Windows.Media;

namespace ModbusTools.SlaveExplorer.Interfaces
{
    public interface IRuntimeField
    {
        int Offset { get; }

        int Size { get; }

        string Name { get; }

        Visual Visual { get; }

        void SetBytes(byte[] data);

        byte[] GetBytes();
    }
}
