namespace ModbusTools.SlaveSimulator.Persistence
{
    public class Slave
    {
        public byte SlaveId { get; set; }

        public Point<ushort>[] HoldingRegisters { get; set; }

        public Point<ushort>[] InputRegisters { get; set; }

        public Point<bool>[] Inputs { get; set; }

        public Point<bool>[] Discretes { get; set; }
    }
}