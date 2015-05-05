namespace ModbusTools.SlaveSimulator.Model
{
    public static class ByteArrayExtensions
    {
        public static ushort ReadUInt16(this byte[] value, int startIndex, bool reverseWords = false)
        {
            if (reverseWords)
            {
                return (ushort) (((ushort) value[startIndex + 1] << 8) + value[startIndex]);
            }

            return (ushort)(((ushort)value[startIndex] << 8) + value[startIndex + 1]);
        }
    }
}
