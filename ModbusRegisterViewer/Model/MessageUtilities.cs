using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
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
        public static byte GetFunction(byte[] message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Length < 2)
                throw new ArgumentException("message must be at least two bytes long");

            //Return the second byte
            return message[1];
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
    }
}
