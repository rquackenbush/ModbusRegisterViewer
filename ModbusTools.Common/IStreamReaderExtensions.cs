using NModbus.IO;

namespace ModbusTools.Common
{
    public static class IStreamReaderExtensions
    {
        public static byte ReadSingleByte(this IStreamResource streamResource)
        {
            var buffer = new byte[1];

            streamResource?.Read(buffer, 0, 1);

            return buffer[0];
        }
    }
}
