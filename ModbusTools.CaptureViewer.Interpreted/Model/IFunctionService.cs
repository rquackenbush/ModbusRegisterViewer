using System.Windows.Media;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public interface IFunctionService
    {
        FunctionCode FunctionCode { get; }

        FunctionServiceResult Process(Sample[] samples);
    }
}