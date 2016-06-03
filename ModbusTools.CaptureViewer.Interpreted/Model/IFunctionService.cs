using ModbusTools.Capture.Model;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public interface IFunctionService
    {
        FunctionCode FunctionCode { get; }

        FunctionServiceResult Process(Sample[] samples);
    }
}