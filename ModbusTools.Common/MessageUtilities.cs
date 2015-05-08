using System;
using System.Linq;
using Modbus.Utility;

namespace ModbusTools.Common
{
    public static class MessageUtilities
    {
        /// <summary>
        /// Gets the slave id from a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte GetSlaveId(byte[] message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Length < 1)
                throw new ArgumentException("message must be at least one byte long");

            //Return the first byte
            return message[0];
        }

        /// <summary>
        /// Gets the slave id from a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static FunctionCode GetFunction(byte[] message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Length < 2)
                throw new ArgumentException("message must be at least two bytes long");

            //Return the second byte
            return (FunctionCode)(message[1] & 0x7f);
        }

        /// <summary>
        /// Gets the CRC of the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ushort GetCRC(byte[] message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Length < 4)
                throw new ArgumentException("message must be at least four bytes long");

            return BitConverter.ToUInt16(message, message.Length - 2);
        }

        public static ushort NetworkBytesToUInt16(byte[] message, int bufferPosition)
        {
            var bytes = message.Skip(bufferPosition).Take(2).ToArray();

            return ModbusUtility.NetworkBytesToHostUInt16(bytes)[0];
        }
    }
}
