using System;
using System.IO.Ports;
using NModbus;
using NModbus;
using NModbus.Serial;

namespace ModbusTools.Common.Model
{
    internal class SerialMasterContext : IMasterContext
    {
        private SerialPort _serialPort;
        private IModbusSerialMaster _master;

        internal SerialMasterContext(SerialPort serialPort, int readTimeout, int writeTimeout)
        {
            var factory = new ModbusFactory();

            var adapter = new SerialPortAdapter(serialPort);

            _serialPort = serialPort;
            _master = factory.CreateRtuMaster(adapter);
            _master.Transport.ReadTimeout = readTimeout;
            _master.Transport.WriteTimeout = writeTimeout;
            _master.Transport.Retries = 0;
        }

        public IModbusMaster Master
        {
            get
            {
                if (_master == null)
                    throw new ObjectDisposedException(GetType().Name);
                
                return _master;
            }
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
