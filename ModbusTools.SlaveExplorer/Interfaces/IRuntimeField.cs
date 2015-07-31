using System.Windows.Media;

namespace ModbusTools.SlaveExplorer.Interfaces
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
