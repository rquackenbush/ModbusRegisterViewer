using System;
using NModbus;

namespace ModbusTools.Common
{
    public interface IMasterContext : IDisposable
    {
        IModbusMaster Master { get; }
    }
}
