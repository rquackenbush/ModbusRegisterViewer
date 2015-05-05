using System;

namespace ModbusTools.SlaveSimulator.Model.MessageFactories
{
    public static class MessageExtensions
    {
        public static void SetUInt16(this byte[] array, int start, ushort value)
        {
            var bytes = BitConverter.GetBytes(value);

            array[start] = bytes[1];
            array[start + 1] = bytes[0];
        }
    }
}
