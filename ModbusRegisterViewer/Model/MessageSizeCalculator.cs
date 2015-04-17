namespace ModbusRegisterViewer.Model
{
    public static class MessageSizeCalculator
    {
        //public static int GetMessageLength(byte[] frameStart, MessageDirection probableMessageDirection)
        //{
        //    switch(probableMessageDirection)
        //    {
        //        case MessageDirection.Request:

        //            //Get it
        //            return GetRequestMessageLength(frameStart);

        //            break;
        //        case MessageDirection.Response:

        //            try
        //            {
        //                return GetResponseMessageLength(frameStart);
        //            }
        //            catch(NotImplementedException)
        //            {
        //                return GetRequestMessageLength(frameStart);
        //            }

        //            break;

        //        default:

        //            //Get it
        //            try
        //            {
        //                return GetRequestMessageLength(frameStart);
        //            }
        //            catch (NotImplementedException)
        //            {
        //                return GetResponseMessageLength(frameStart);
        //            }
        //    }
        //}

        public static int? GetRequestMessageLength(byte[] frameStart)
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
                    return null;
            }
        }

        public static int? GetResponseMessageLength(byte[] frameStart)
        {
            byte functionCode = MessageUtilities.GetFunction(frameStart);

            switch (functionCode)
            {
                case FunctionCodes.ReadCoils:
                case FunctionCodes.ReadDiscreteInputs:
                case FunctionCodes.ReadHoldingRegisters:
                case FunctionCodes.ReadInputRegisters:

                    return frameStart[2] + 5;

                case FunctionCodes.WriteSingleCoil:
                case FunctionCodes.WriteSingleRegister:
                case FunctionCodes.WriteMultipleCoils:
                case FunctionCodes.WriteMultipleRegisters:
                case FunctionCodes.Diagnostic:

                    return 8;

                default:
                    return null;
            }
        }
    }
}
