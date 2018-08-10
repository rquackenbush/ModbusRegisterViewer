namespace ModbusTools.SlaveSimulator.Persistence
{
    public class SimulatorProject
    {
        public string CommPort { get; set; }

        public double? ReadTimeout { get; set; }

        public double? WriteTimeout { get; set; }

        public Slave[] Slaves { get; set; }
    }
}