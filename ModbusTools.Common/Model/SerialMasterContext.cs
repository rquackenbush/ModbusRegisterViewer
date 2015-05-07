using System;
using System.IO.Ports;
using Modbus.Device;

namespace ModbusTools.Common.Model
{
    internal class SerialMasterContext : IMasterContext
    {
        private SerialPort _serialPort;
        private ModbusSerialMaster _master;

        public IModbusMaster Master
        {
            get
            {
                if (_master == null)
                    throw new ObjectDisposedException(GetType().Name);
                
                return _master;
            }
        }

        internal SerialMasterContext(SerialPort serialPort, int readTimeout, int writeTimeout)
        {
            _serialPort = serialPort;
            _master = ModbusSerialMaster.CreateRtu(_serialPort);
            _master.Transport.ReadTimeout = readTimeout;
            _master.Transport.WriteTimeout = writeTimeout;
            _master.Transport.Retries = 0;
        }

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }

            if (_master == null)
                return;

            _master.Dispose();
            _master = null;
        }
    }
}
