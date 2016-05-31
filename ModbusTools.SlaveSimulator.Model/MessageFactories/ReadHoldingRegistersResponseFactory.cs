using System;
using ModbusTools.Common;

namespace ModbusTools.SlaveSimulator.Model.MessageFactories
{
    public static class ReadHoldingRegistersResponseFactory
    {
        public static byte[] Create(byte slaveAddress, ushort[] registers)
        {
            if (registers == null) throw new ArgumentNullException(nameof(registers));
            if (registers.Length == 0) throw new ArgumentOutOfRangeException(nameof(registers),"registers should have at least one value");

            //Calculate the datasize
            var dataSize = (byte)(2*registers.Length);

            //Calculate the buffersize
            var bufferSize = 5 + dataSize;

            //Create the hresponse buffer
            var buffer = new byte[bufferSize];

            //Slave Address
            buffer[0] = slaveAddress;

            //Function code
            buffer[1] = (byte)FunctionCode.ReadHoldingRegisters;

            //The number of data bytes to follow
            buffer[2] = dataSize;

            byte dataIndex = 3;

            //Copy the registers into the range
            foreach (var register in registers)
            {
                //Get the bytes
                var bytes = BitConverter.GetBytes(register);

                //Copy to the message
                buffer[dataIndex++] = bytes[1];
                buffer[dataIndex++] = bytes[0];
            }

            //Set the crc
            buffer.SetCrc();

            return buffer;
        }
    }
}
