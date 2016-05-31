using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class ReadHoldingRegistersFunctionService : ReadRegistersFunctionService
    {
        public ReadHoldingRegistersFunctionService() 
            : base(FunctionCode.ReadHoldingRegisters)
        {
        }

     
    }
}