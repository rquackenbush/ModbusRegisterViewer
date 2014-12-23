namespace ModbusTools.Common
{
    public interface IModbusAdapterProvider
    {
        /// <summary>
        /// Gets a value indicating whether an item is selected or not.
        /// </summary>
        bool IsItemSelected { get; }

        /// <summary>
        /// Gets the factory
        /// </summary>
        /// <returns></returns>
        IMasterContextFactory GetFactory();
    }
}
