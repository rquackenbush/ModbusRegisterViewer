using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.IO;

namespace ModbusRegisterViewer.Model
{
    public static class IStreamReaderExtensions
    {
        public static byte ReadSingleByte(this IStreamResource streamResource)
        {
            var buffer = new byte[1];

            streamResource.Read(buffer, 0, 1);

            return buffer[0];
        }
    }
}
