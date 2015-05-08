using System;
using System.Linq;
using Modbus.Utility;

namespace ModbusTools.Common
{
    public static class CrcExtensions
    {
        /// <summary>
        /// Determines whether the crc stored in the message matches the calculated crc.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool DoesCrcMatch(this byte[] message)
        {
            var messageFrame = message.Take(message.Length - 2).ToArray();

            //Calculate the CRC with the given set of bytes
            var calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(messageFrame), 0);   

            //Get the crc that is stored in the message
            var messageCrc = MessageUtilities.GetCRC(message);

            Console.WriteLine("CRC {0} vs {1}", calculatedCrc, messageCrc);

            //Determine if they match
            return calculatedCrc == messageCrc;
        }

        /// <summary>
        /// Sets the crc of the message.
        /// </summary>
        /// <param name="message"></param>
        public static void SetCrc(this byte[] message)
        {
            var messageFrame = message.Take(message.Length - 2).ToArray();

            var crc = ModbusUtility.CalculateCrc(messageFrame);

            message[message.Length - 2] = crc[0];
            message[message.Length - 1] = crc[1];
        }
    }
}
