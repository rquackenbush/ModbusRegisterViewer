using System;
using System.Collections.Generic;
using System.Linq;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public static class FunctionServiceManager
    {
        private static readonly Dictionary<FunctionCode, IFunctionService> _functionServices;

        static FunctionServiceManager()
        {
            var services = new IFunctionService[]
            {
                new ReadHoldingRegistersFunctionService(),
                new ReadInputRegistersFunctionService(), 
                new WriteRegistersFunctionService(), 
            };

            _functionServices = services.ToDictionary(s => s.FunctionCode, s => s);
        }

        public static FunctionServiceResult Process(Sample[] samples)
        {
            try
            {  
                IFunctionService service;

                if (samples.Length < 5)
                {
                    return new FunctionServiceResult("Packet not long enough.");
                }

                byte rawFunctionCode = samples[1].Value;

                if ((rawFunctionCode & 0x80) > 0)
                {
                    var exceptionDescription = SlaveExceptionDescriptionFactory.GetExceptionDescription(samples[2].Value);

                    return new FunctionServiceResult(exceptionDescription);
                }

                FunctionCode functionCode = (FunctionCode)rawFunctionCode;

                if (_functionServices.TryGetValue(functionCode, out service))
                {
                    return service.Process(samples);
                }

                return new FunctionServiceResult("Unsupported function code.");

            }
            catch (Exception ex)
            {
                return new FunctionServiceResult(ex.Message);
            }
        }
    }
}