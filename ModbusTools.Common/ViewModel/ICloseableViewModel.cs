using System;

namespace ModbusTools.Common.ViewModel
{
    public interface ICloseableViewModel
    {
        event EventHandler<CloseEventArgs> Close;

        bool CanClose();

        void Closed();
    }
}
