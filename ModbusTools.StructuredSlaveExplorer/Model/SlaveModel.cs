namespace ModbusTools.StructuredSlaveExplorer.Model
{
    public class SlaveModel
    {
        public string Name { get; set; }

        public byte SlaveId { get; set; }

        public RangeModel[] Ranges { get; set; }
    }
}
