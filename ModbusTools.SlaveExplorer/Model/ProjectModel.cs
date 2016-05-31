namespace ModbusTools.SlaveExplorer.Model
{
    public class ProjectModel
    {
        /// <summary>
        /// So here's the deal - this used to support multiple slaves. The new approach does not do that. There is no value in that!
        /// </summary>
        public SlaveModel[] Slaves { get; set; }
    }
}
