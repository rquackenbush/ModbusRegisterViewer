namespace ModbusTools.SlaveSimulator.Persistence
{
    public class Point<TPoint>
    {
        public ushort Address { get; set; }

        public TPoint Value { get; set; }
    }
}