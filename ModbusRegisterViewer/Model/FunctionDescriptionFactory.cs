using ModbusTools.Common;

namespace ModbusRegisterViewer.Model
{
    public static class FunctionDescriptionFactory
    {
        public static string GetFunctionDescription(FunctionCode function)
        {
            switch (function)
            {
                case FunctionCode.Diagnostic:
                    return "Diagnostic";

                case FunctionCode.GetComEventCounter:
                    return "Get Com Event Counter";

                case FunctionCode.GetComEventLog:
                    return "Get Com Event Log";

                case FunctionCode.MaskWriteRegsiter:
                    return "Mask Write Register";

                case FunctionCode.ReadCoils:
                    return "Read Coils";

                case FunctionCode.ReadDeviceIdentification:
                    return "Read Device Identification";

                case FunctionCode.ReadDiscreteInputs:
                    return "Read Discrete Inputs";

                case FunctionCode.ReadExceptionStatus:
                    return "Read Exception Status";

                case FunctionCode.ReadFIFOQueue:
                    return "Read FIFO Queue";

                case FunctionCode.ReadFileRecord:
                    return "Read File Record";

                case FunctionCode.ReadHoldingRegisters:
                    return "Read Holding Registers";

                case FunctionCode.ReportSlaveId:
                    return "Report Slave Id";

                case FunctionCode.WriteFileRecord:
                    return "Write File Record";

                case FunctionCode.WriteMultipleCoils:
                    return "Write Multiple Coils";

                case FunctionCode.WriteMultipleRegisters:
                    return "Write Multiple Registers";

                case FunctionCode.WriteSingleCoil:
                    return "Write Single Coil";

                case FunctionCode.WriteSingleRegister:
                    return "Write Single Register";

                case FunctionCode.ReadInputRegisters:
                    return "Read Input Registers";
               
                default:
                    return "Unknown";
            }
                
        }
    }
}
