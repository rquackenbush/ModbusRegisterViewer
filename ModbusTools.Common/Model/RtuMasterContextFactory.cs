using FtdAdapter;

namespace ModbusTools.Common.Model
{
    internal class RtuMasterContextFactory : IMasterContextFactory
    {
        private readonly string _serialNumber;
        private readonly int _baudRate;
        private readonly int _dataBits;
        private readonly FtdParity _parity;
        private readonly FtdStopBits _stopBits;
        private readonly int _readTimeout;
        private readonly int _writeTimeout;

        public RtuMasterContextFactory(string serialNumber = null, int baudRate = 19200, int dataBits = 8, FtdParity parity = FtdParity.Even, FtdStopBits stopBits = FtdStopBits.One, int readTimeout = 2000, int writeTimeout = 2000)
        {
            _serialNumber = serialNumber;
            _baudRate = baudRate;
            _dataBits = dataBits;
            _parity = parity;
            _stopBits = stopBits;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
        }

        public IMasterContext Create()
        {
            return new RtuMasterContext(_serialNumber, _baudRate, _dataBits, _parity, _stopBits, _readTimeout, _writeTimeout);
        }
    }
}
