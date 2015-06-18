using System.IO.Ports;
using ModbusTools.Common.Model;

namespace ModbusTools.Common.ViewModel
{
    internal class SerialPortModbusAdapterFactory : IModbusAdapterFactory
    {
        private readonly string _portName;
        private readonly int _baudRate;
        private readonly int _dataBits;
        private readonly System.IO.Ports.Parity _parity;
        private readonly System.IO.Ports.StopBits _stopBits;
        private readonly int _readTimeout;
        private readonly int _writeTimeout;

        internal SerialPortModbusAdapterFactory(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, int readTimeout, int writeTimeout)
        {
            _portName = portName;
            _dataBits = dataBits;
            _baudRate = baudRate;
            _parity = parity;
            _stopBits = stopBits;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
        }

        public string DisplayName
        {
            get { return _portName; }
        }

        public IMasterContextFactory CreateMasterContext()
        {
            return new SerialMasterContextFactory(_portName, _baudRate, _dataBits, _parity, _stopBits, _readTimeout, _writeTimeout);
        }
    }
}
