namespace ModbusTools.Common
{
    public static class NumericExtensions
    {
        public static byte GetMSB(this ushort value)
        {
            return (byte)(value >> 8);
        }

        public static byte GetLSB(this ushort value)
        {
            return (byte) value;
        }
    }
}