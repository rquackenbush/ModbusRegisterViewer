using System;
using Cas.Common.WPF.Interfaces;
using ModbusTools.Common;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    public interface ISlaveExplorerContext
    {
        byte SlaveId { get; }

        IModbusAdapterProvider ModbusAdapterProvider { get; }

        IMessageBoxService MessageBoxService { get; }
    }
}