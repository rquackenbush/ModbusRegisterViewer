using System;
using System.Collections.Generic;

namespace ModbusTools.Common
{
    public static class RegisterExtensions
    {
        public static byte[] ToBytes(this ushort[] values)
        {
            var bytes = new List<byte>(values.Length * 2);

            foreach (var value in values)
            {
                bytes.Add((byte)(value >> 8));
                bytes.Add((byte)value);
            }

            return bytes.ToArray();
        }

        public static ushort[] ToRegisters(this byte[] values)
        {
            if (values.Length % 2 != 0)
                throw new ArgumentException("The provided array must be of an even length.");

            var numberOfRegisters = values.Length / 2;

            var registers = new ushort[numberOfRegisters];

            for (var index = 0; index < numberOfRegisters; index++)
            {
                var msb = (ushort)(((ushort)values[index * 2]) << 8);
                var lsb = (ushort)values[(index * 2) + 1];

                registers[index] = (ushort)(msb | lsb);
            }

            return registers;
        }
    }
}
