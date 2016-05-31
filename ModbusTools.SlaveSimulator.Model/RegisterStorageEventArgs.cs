using System;

namespace ModbusTools.SlaveSimulator.Model
{
    public class RegisterStorageEventArgs : EventArgs
    {
        private readonly ushort _startingAddress;
        private readonly ushort[] _values;

        public RegisterStorageEventArgs(ushort startingAddress, ushort[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            _startingAddress = startingAddress;
            _values = values;
        }

        public ushort StartingAddress
        {
            get { return _startingAddress; }
        }

        public ushort[] Values
        {
            get { return _values; }
        }
    }
}
