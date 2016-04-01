namespace ModbusTools.SlaveExplorer.Model
{
    public enum FieldType
    {
        UINT32 = 0,

        INT32 = 1, 

        UINT16 = 2,

        INT16 = 3,

        UINT8 = 4,

        INT8 = 5,

        BIT8 = 6,

        FLOAT32 = 7,

        //Signed 16 bit fixedpoint integer
        FIXED16 = 8,

        //Unsigned 16 bit fixedpoint integer
        UFIXED16 = 9,

        BIT16 = 10,

        BIT32 = 11,
    }
}
