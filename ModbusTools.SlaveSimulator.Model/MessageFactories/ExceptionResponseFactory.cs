using ModbusTools.Common;

namespace ModbusTools.SlaveSimulator.Model.MessageFactories
{
    public static class ExceptionResponseFactory
    {
        public static byte[] Create(byte slaveId, FunctionCode functionCode, ModbusExceptionCodes exceptionCode)
        {
            var buffer = new byte[5];

            //Set the slave id            
            buffer[0] = slaveId;

            //The function code with the highest bit set
            buffer[1] = (byte)((byte)(functionCode) | (1 << 7));

            //Set the exception code
            buffer[2] = (byte) exceptionCode;

            //Set the crc
            buffer.SetCrc();

            return buffer;

        }
    }
}
