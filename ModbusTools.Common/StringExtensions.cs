using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusTools.Common
{
    public static class StringExtensions
    {
        public static string CreateUnique(this IEnumerable<string> values, string formatString)
        {
            var valuesCopy = values.ToArray();

            var count = 1;

            var proposed = string.Format(formatString, count);

            while (valuesCopy.Any(v => String.Compare(v, proposed, true) == 0))
            {
                count++;

                proposed = string.Format(formatString, count);
            }

            return proposed;
        }
    }
}
