using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public static class RequestSizeCalculator
    {
        public static int GetRequestMessageLength(byte[] frameStart)
        {
            byte functionCode = MessageUtilities.GetFunction(frameStart);

            switch (functionCode)
            {
                case FunctionCodes.ReadCoils:
                case FunctionCodes.ReadDiscreteInputs:
                case FunctionCodes.ReadHoldingRegisters:
                case FunctionCodes.ReadInputRegisters:
                case FunctionCodes.WriteSingleCoil:
                case FunctionCodes.WriteSingleRegister:
                case FunctionCodes.Diagnostic:
                    return 8;

                case FunctionCodes.WriteMultipleCoils:
                case FunctionCodes.WriteMultipleRegisters:

                    byte byteCount = frameStart[6];
                    return 9 + byteCount;

                default:
                    throw new NotImplementedException();
            }
        }

        public static int GetResponseMessageLength(byte[] frameStart)
        {
            byte functionCode = MessageUtilities.GetFunction(frameStart);

            switch (functionCode)
            {
                case FunctionCodes.ReadCoils:
                case FunctionCodes.ReadDiscreteInputs:
                case FunctionCodes.ReadHoldingRegisters:
                case FunctionCodes.ReadInputRegisters:

                    return frameStart[2] + 5;

                    break;

                case FunctionCodes.WriteSingleCoil:
                case FunctionCodes.WriteSingleRegister:
                case FunctionCodes.WriteMultipleCoils:
                case FunctionCodes.WriteMultipleRegisters:
                case FunctionCodes.Diagnostic:

                    return 8;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
