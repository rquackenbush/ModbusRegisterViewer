namespace ModbusTools.SlaveExplorer.Model
{
    public class FieldModel
    {
        public int Offset { get; set; }

        public string Name { get; set; }

        public FieldType FieldType { get; set; }

        public FieldOptionModel[] Options { get; set; }
    }
}
