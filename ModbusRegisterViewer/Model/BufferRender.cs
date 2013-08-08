using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public static class BufferRender
    {
        public static string GetDisplayString(IEnumerable<byte> buffer)
        {
            if (buffer == null || !buffer.Any())
                return "<Empty>";

            string display = "";

            bool first = true;

            foreach(var bufferByte in buffer)
            {
                if (!first)
                    display += " ";

                display += string.Format("{0:X}", bufferByte).PadLeft(2, '0');

                first = false;
            }

            return display;
        }
    }
}
