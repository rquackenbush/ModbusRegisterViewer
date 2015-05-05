using ModbusTools.Common;

namespace ModbusRegisterViewer.Model
{
    public static class MessageSizeCalculator
    {
        public static int? GetRequestMessageLength(byte[] frameStart)
        {
            var functionCode = MessageUtilities.GetFunction(frameStart);

            switch (functionCode)
            {
                case FunctionCode.ReadCoils:
                case FunctionCode.ReadDiscreteInputs:
                case FunctionCode.ReadHoldingRegisters:
                case FunctionCode.ReadInputRegisters:
                case FunctionCode.WriteSingleCoil:
                case FunctionCode.WriteSingleRegister:
                case FunctionCode.Diagnostic:
                    return 8;

                case FunctionCode.WriteMultipleCoils:
                case FunctionCode.WriteMultipleRegisters:

                    byte byteCount = frameStart[6];
                    return 9 + byteCount;

                default:
                    return null;
            }
        }

        public static int? GetResponseMessageLength(byte[] frameStart)
        {
            var functionCode = MessageUtilities.GetFunction(frameStart);

            switch (functionCode)
            {
                case FunctionCode.ReadCoils:
                case FunctionCode.ReadDiscreteInputs:
                case FunctionCode.ReadHoldingRegisters:
                case FunctionCode.ReadInputRegisters:

                    return frameStart[2] + 5;

                case FunctionCode.WriteSingleCoil:
                case FunctionCode.WriteSingleRegister:
                case FunctionCode.WriteMultipleCoils:
                case FunctionCode.WriteMultipleRegisters:
                case FunctionCode.Diagnostic:

                    return 8;

                default:
                    return null;
            }
        }
    }
}
