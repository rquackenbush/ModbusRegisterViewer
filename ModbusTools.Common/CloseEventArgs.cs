using System;

namespace ModbusTools.Common
{
    public class CloseEventArgs : EventArgs
    {
        public bool? DialogResult { get; set; }

        public CloseEventArgs(bool? dialogResult)
        {
            this.DialogResult = dialogResult;
        }
    }
}
