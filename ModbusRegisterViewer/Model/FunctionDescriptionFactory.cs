using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public static class FunctionDescriptionFactory
    {
        public static string GetFunctionDescription(byte function)
        {
            switch (function)
            {
                case FunctionCodes.Diagnostic:
                    return "Diagnostic";

                case FunctionCodes.GetComEventCounter:
                    return "Get Com Event Counter";

                case FunctionCodes.GetComEventLog:
                    return "Get Com Event Log";

                case FunctionCodes.MaskWriteRegsiter:
                    return "Mask Write Register";

                case FunctionCodes.ReadCoils:
                    return "Read Coils";

                case FunctionCodes.ReadDeviceIdentification:
                    return "Read Device Identification";

                case FunctionCodes.ReadDiscreteInputs:
                    return "Read Discrete Inputs";

                case FunctionCodes.ReadExceptionStatus:
                    return "Read Exception Status";

                case FunctionCodes.ReadFIFOQueue:
                    return "Read FIFO Queue";

                case FunctionCodes.ReadFileRecord:
                    return "Read File Record";

                case FunctionCodes.ReadHoldingRegisters:
                    return "Read Holding Registers";

                case FunctionCodes.ReportSlaveId:
                    return "Report Slave Id";

                case FunctionCodes.WriteFileRecord:
                    return "Write File Record";

                case FunctionCodes.WriteMultipleCoils:
                    return "Write Multiple Coils";

                case FunctionCodes.WriteMultipleRegisters:
                    return "Write Multiple Registers";

                case FunctionCodes.WriteSingleCoil:
                    return "Write Single Coil";

                case FunctionCodes.WriteSingleRegister:
                    return "Write Single Register";
               
                default:
                    return "Unknown";
            }
                
        }
    }
}
