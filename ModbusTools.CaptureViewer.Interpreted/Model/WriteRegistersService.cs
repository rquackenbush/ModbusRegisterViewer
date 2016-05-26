using ModbusTools.Capture.Model;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class WriteRegistersService : FunctionServiceBase
    {
        public WriteRegistersService(FunctionCode functionCode) 
            : base(FunctionCode.WriteMultipleRegisters)
        {
        }

        public override FunctionServiceResult Process(Sample[] samples)
        {
            return new FunctionServiceResult("Not done yet.");
        }
    }
}