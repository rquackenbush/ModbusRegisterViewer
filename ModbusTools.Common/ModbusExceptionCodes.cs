namespace ModbusTools.Common
{
    public enum ModbusExceptionCodes : byte
    {
        FunctionCodeNotSupported = 0x01,
        InvalidAddress = 0x02,
        IllegalDataValue = 0x03,
        FailureInAssociatedDevice = 0x04
    }
}
