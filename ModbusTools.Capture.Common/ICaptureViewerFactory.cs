using System.Windows.Media;

namespace ModbusTools.Capture.Common
{
    public interface ICaptureViewerFactory
    {
        string Name { get; }

        Visual Open(string filename);
    }
}