using ModbusTools.Common;

namespace ModbusTools.SlaveSimulator.Model.MessageFactories
{
    public static class WriteHoldingRegistersResponseFactory
    {
        public static byte[] Create(byte slaveAddress, ushort startingAddress, ushort numberOfRegisters)
        {
            //Create the hresponse buffer
            var buffer = new byte[8];

            //Slave Address
            buffer[0] = slaveAddress;

            //Function code
            buffer[1] = (byte)FunctionCode.WriteMultipleRegisters;

            //The number or registers
            buffer.SetUInt16(2, startingAddress);

            //Set the starting address
            buffer.SetUInt16(4, numberOfRegisters);

            //Set the crc
            buffer.SetCrc();

            return buffer;
        }
    }
}
