using ModbusTools.Common.Model;

namespace ModbusTools.Common.ViewModel
{
    internal class SerialPortModbusAdapterFactory : IModbusAdapterFactory
    {
        private readonly string _portName;

        internal SerialPortModbusAdapterFactory(string portName)
        {
            _portName = portName;
        }

        public string DisplayName
        {
            get { return _portName; }
        }

        public IMasterContextFactory CreateMasterContext()
        {
            return new SerialMasterContextFactory(_portName);
        }
    }
}
