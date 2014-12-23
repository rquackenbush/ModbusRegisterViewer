using System;
using Modbus.Device;

namespace ModbusTools.Common
{
    public interface IMasterContext : IDisposable
    {
        IModbusMaster Master { get; }
    }
}
