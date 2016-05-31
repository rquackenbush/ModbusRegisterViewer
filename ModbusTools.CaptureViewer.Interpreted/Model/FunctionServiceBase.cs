using System.Windows.Media;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public abstract class FunctionServiceBase : IFunctionService
    {
        private readonly FunctionCode _functionCode;

        protected FunctionServiceBase(FunctionCode functionCode)
        {
            _functionCode = functionCode;
        }

        public FunctionCode FunctionCode
        {
            get { return _functionCode; }
        }

        public abstract FunctionServiceResult Process(Sample[] samples);
    }

    
}