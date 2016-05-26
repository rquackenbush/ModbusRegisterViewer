using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusTools.Common
{
    public static class HexFormatter
    {
        public static string FormatHex(this IEnumerable<byte> data, string separator = " ")
        {
            return string.Join(separator, data.Select(b => b.ToString("X2")));
        }
    }
}
