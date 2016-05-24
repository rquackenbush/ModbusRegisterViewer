using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted
{
    public static class FunctionCodeDescriptionFactory
    {
        public static string GetFunctionCodeDescription(byte functionCode)
        {
            switch ((FunctionCode) functionCode)
            {
                case FunctionCode.ReadDiscreteInputs:
                    return $"({functionCode}) - Read Discrete Inputs";

                case FunctionCode.ReadCoils:
                    return $"({functionCode}) - ReadCoils";

                case FunctionCode.ReadInputRegisters:
                    return $"({functionCode}) - Read Input Registers";

                case FunctionCode.WriteSingleCoil:
                    return $"({functionCode}) - Write Single Coil";

                case FunctionCode.WriteMultipleCoils:
                    return $"({functionCode}) - Write Multiple Coils";

                case FunctionCode.ReadHoldingRegisters:
                    return $"({functionCode}) - Read Holding Registers";

                case FunctionCode.WriteSingleRegister:
                    return $"({functionCode}) - Write Single Register";

                case FunctionCode.MaskWriteRegsiter:
                    return $"({functionCode}) - Mask Write Regsiter";

                case FunctionCode.ReadFIFOQueue:
                    return $"({functionCode}) - Read FIFO Queue";

                case FunctionCode.ReadFileRecord:
                    return $"({functionCode}) - Read File Record";

                case FunctionCode.WriteFileRecord:
                    return $"({functionCode}) - Write File Record";

                case FunctionCode.ReadExceptionStatus:
                    return $"({functionCode}) - Read Exception Status";

                case FunctionCode.Diagnostic:
                    return $"({functionCode}) - Diagnostic";

                case FunctionCode.GetComEventCounter:
                    return $"({functionCode}) - Get Com Event Counter";

                case FunctionCode.GetComEventLog:
                    return $"({functionCode}) - Get Com Event Log";

                case FunctionCode.ReportSlaveId:
                    return $"({functionCode}) - Report Slave Id";

                case FunctionCode.ReadDeviceIdentification:
                    return $"({functionCode}) - Read Device Identification";

                case FunctionCode.WriteMultipleRegisters:
                    return $"({functionCode}) - Write Multiple Registers";

                default:
                    return $"({functionCode}) - UNKNOWN";
            }
        }
    }
}