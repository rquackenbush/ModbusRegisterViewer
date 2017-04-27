using System;
using System.IO.Ports;
using NModbus;
using NModbus.IO;
using NModbus.Serial;

namespace ModbusTools.Common.Model
{
    internal class SerialMasterContextFactory : IMasterContextFactory
    {
        private const int DefaultReadTimeout = 1000;
        private const int DefaultWriteTiemout = 1000;
        private readonly Func<SerialPort> _serialPortFactory;
        private readonly string _portName;
        private readonly int _baudRate;
        private readonly Parity _parity;
        private readonly int _dataBits;
        private readonly StopBits _stopBits;
        private readonly int _readTimeout;
        private readonly int _writeTimeout;

        private static readonly IModbusFactory ModbusFactory = new ModbusFactory();

        public SerialMasterContextFactory(string portName, int baudRate = 19200, int dataBits = 8, Parity parity = Parity.Even, StopBits stopBits = StopBits.One, int readTimeout = DefaultReadTimeout, int writeTimeout = DefaultWriteTiemout)
            : this(readTimeout, writeTimeout)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
        }

        /// <summary>
        /// <param name="readTimeout"/>
        /// <param name="writeTimeout"/>
        /// <param name="serialPortFactory">A func that returns a configured and opened serial port.</param>
        /// </summary>
        public SerialMasterContextFactory(Func<SerialPort> serialPortFactory, int readTimeout = DefaultReadTimeout, int writeTimeout = DefaultWriteTiemout)
            : this(readTimeout, writeTimeout)
        {
            _serialPortFactory = serialPortFactory;
        }

        private SerialMasterContextFactory(int readTimeout, int writeTimeout)
        {
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
        }

        private SerialPort CreateSerialPort()
        {
            SerialPort serialPort;

            if (_serialPortFactory != null)
            {
                serialPort = _serialPortFactory();
            }
            else
            {
                serialPort = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits);
                serialPort.Open();
            }

            return serialPort;
        }

        public IMasterContext Create()
        {
            SerialPort serialPort = CreateSerialPort();

            return new SerialMasterContext(serialPort, _readTimeout, _writeTimeout);
        }

        public IStreamResource CreateStreamResource()
        {
            SerialPort serialPort = CreateSerialPort();

            IStreamResource adapter = new SerialPortAdapter(serialPort);

            return adapter;
        }

        private IModbusRtuTransport CreateTransport()
        {
            IStreamResource streamResource = CreateStreamResource();

            IModbusRtuTransport transport = ModbusFactory.CreateRtuTransport(streamResource);

            transport.ReadTimeout = _readTimeout;
            transport.WriteTimeout = _writeTimeout;
            transport.Retries = 0;

            return transport;
        }

        public IModbusSlaveNetwork CreateSlaveNetwork()
        {
            IModbusRtuTransport transport = CreateTransport();

            return ModbusFactory.CreateSlaveNetwork(transport);
        }

        public IModbusMaster CreateMaster()
        {
            IModbusRtuTransport transport = CreateTransport();

            return ModbusFactory.CreateMaster(transport);
        }
    }
}
