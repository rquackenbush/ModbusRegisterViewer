using System;
using FtdAdapter;
using Modbus.Device;

namespace ModbusTools.Common.Model
{
    internal class RtuMasterContext : IMasterContext
    {
        private FtdUsbPort _port;
        private IModbusMaster _master;

        public IModbusMaster Master
        {
            get
            {
                if (_master == null)
                    throw new ObjectDisposedException(GetType().Name);
                
                return _master;
            }
        }

        public RtuMasterContext(string serialNumber, int baudRate, int dataBits, FtdParity parity, FtdStopBits stopBits, int readTimeout, int writeTimeout)
        {
            _port = new FtdUsbPort()
            {
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits
            };

            if (string.IsNullOrEmpty(serialNumber))
                _port.OpenByIndex(0U);
            else
                _port.OpenBySerialNumber(serialNumber);

            _port.ReadTimeout = readTimeout;
            _port.WriteTimeout = writeTimeout;
            _master = ModbusSerialMaster.CreateRtu(_port);
        }

        public void Dispose()
        {
            if (_port != null)
            {
                _port.Dispose();
                _port = null;
            }

            if (_master == null)
                return;

            _master.Dispose();
            _master = null;
        }
    }
}
