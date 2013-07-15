using System;
using FtdAdapter;
using Modbus.Device;

namespace ModbusRegisterViewer
{
    public class MasterContext : IDisposable
    {
        private FtdUsbPort _port;
        private IModbusSerialMaster _master;

        public MasterContext()
        {
            _port = new FtdUsbPort();

            // configure serial port
            _port.BaudRate = 19200;
            _port.DataBits = 8;
            _port.Parity = FtdParity.Even;
            _port.StopBits = FtdStopBits.One;
            _port.OpenByIndex(0);

            _master = ModbusSerialMaster.CreateRtu(_port);
        }

        public IModbusSerialMaster Master
        {
            get 
            { 
                if (_master == null)
                    throw new ObjectDisposedException(this.GetType().Name);

                return _master; 
            }
        }

        public void Dispose()
        {
            if (_port != null)
            {
                _port.Dispose();
                _port = null;
            }

            if (_master != null)
            {
                _master.Dispose();
                _master = null;
            }
        }
    }
}
