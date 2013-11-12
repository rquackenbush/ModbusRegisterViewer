using Modbus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model.Sniffer
{
    public class CrcComparer
    {
        public static bool DoesCrcMatch(byte[] message)
        {
            var messageFrame = message.Take(message.Length - 2).ToArray();

            //Calculate the CRC with the given set of bytes
            var calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(messageFrame), 0);   

            //Get the crc that is stored in the message
            var messageCrc = MessageUtilities.GetCRC(message);

            return calculatedCrc == messageCrc;
        }
    }
}
