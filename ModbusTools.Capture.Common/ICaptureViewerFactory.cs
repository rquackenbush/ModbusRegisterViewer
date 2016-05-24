using System.Windows;

namespace ModbusTools.Capture.Common
{
    public interface ICaptureViewerFactory
    {
        string Name { get; }

        Window Open(string filename);
    }
}