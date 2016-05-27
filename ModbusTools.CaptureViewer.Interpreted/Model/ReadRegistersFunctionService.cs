using ModbusTools.Capture.Model;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public abstract class ReadRegistersFunctionService : RegistersFunctionService
    {
        protected ReadRegistersFunctionService(FunctionCode functionCode) 
            : base(functionCode)
        {
        }

        public override FunctionServiceResult Process(Sample[] samples)
        {
            if (samples.Length == NumberOfRegistersMessageLength)
            {
                return ProcessNumberOfRegisters(samples);
            }

            return ProcessRegisters(samples);
        }
    }
}