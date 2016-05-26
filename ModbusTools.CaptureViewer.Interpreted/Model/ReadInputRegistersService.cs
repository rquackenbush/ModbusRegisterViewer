using ModbusTools.Capture.Model;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class ReadInputRegistersService : ReadRegistersService
    {
        public ReadInputRegistersService() 
            : base(FunctionCode.ReadInputRegisters)
        {
        }
    }
}