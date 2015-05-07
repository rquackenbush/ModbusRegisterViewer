using System;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveEvent : EventArgs
    {
        private readonly SlaveViewModel _slave;

        public SlaveEvent(SlaveViewModel slave)
        {
            _slave = slave;
        }

        public SlaveViewModel Slave
        {
            get { return _slave; }
        }
    }
}
