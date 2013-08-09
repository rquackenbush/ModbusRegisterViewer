using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    /// <summary>
    /// Modbus function code constants
    /// http://en.wikipedia.org/wiki/Modbus
    /// </summary>
    public static class FunctionCodes
    {
        public const byte ReadDiscreteInputs = 2;

        public const byte ReadCoils = 1;

        public const byte ReadInputRegisters = 4;

        public const byte WriteSingleCoil = 5;

        public const byte WriteMultipleCoils = 15;

        public const byte ReadHoldingRegisters = 3;

        public const byte WriteSingleRegister = 6;

        public const byte MaskWriteRegsiter = 22;

        public const byte ReadFIFOQueue = 24;

        public const byte ReadFileRecord = 20;

        public const byte WriteFileRecord = 21;

        public const byte ReadExceptionStatus = 7;

        public const byte Diagnostic = 8;

        public const byte GetComEventCounter = 11;

        public const byte GetComEventLog = 12;

        public const byte ReportSlaveId = 17;

        public const byte ReadDeviceIdentification = 43;

        public const byte WriteMultipleRegisters = 16;
    }
}
