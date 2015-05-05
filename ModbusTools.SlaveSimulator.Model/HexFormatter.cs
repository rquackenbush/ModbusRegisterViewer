using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTools.SlaveSimulator.Model
{
    internal static class HexFormatter
    {
        internal static string FormatHex(IEnumerable<byte> data)
        {
            var builder = new StringBuilder();

            foreach (var sample in data)
            {
                builder.AppendFormat("{0:X2} ", sample);
            }

            return builder.ToString();
        }
    }
}
