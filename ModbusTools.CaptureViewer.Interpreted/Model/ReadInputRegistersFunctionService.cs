using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class ReadInputRegistersFunctionService : ReadRegistersFunctionService
    {
        public ReadInputRegistersFunctionService() 
            : base(FunctionCode.ReadInputRegisters)
        {
        }
    }
}