namespace ModbusTools.Common
{
    public interface IPreferences
    {
        void Clear();

        string this[string key] { get; set; }

        void Save();
    }
}
