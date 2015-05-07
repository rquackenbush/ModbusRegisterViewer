using System;

namespace ModbusTools.SlaveSimulator.Model
{
    /// <summary>
    /// This is a shim that raises events for reads / writes
    /// </summary>
    public class RegisterStorageNotifier : IRegisterStorage
    {
        private readonly IRegisterStorage _source;

        public event EventHandler<RegisterStorageEventArgs> DataWasRead;
        public event EventHandler<RegisterStorageEventArgs> DataWasWritten;

        public RegisterStorageNotifier(IRegisterStorage source)
        {
            if (source == null) throw new ArgumentNullException("source");

            _source = source;
        }

        protected virtual void OnDataWasRead(ushort startingIndex, ushort[] registers)
        {
            var handler = DataWasRead;

            if (handler == null)
                return;

            handler(this, new RegisterStorageEventArgs(startingIndex, registers));
        }

        protected virtual void OnDataWasWritten(ushort startingIndex, ushort[] registers)
        {
            var handler = DataWasWritten;

            if (handler == null)
                return;

            handler(this, new RegisterStorageEventArgs(startingIndex, registers));
        }

        public ushort[] Read(ushort startingIndex, ushort numberOfRegisters)
        {
            var registers = _source.Read(startingIndex, numberOfRegisters);

            OnDataWasRead(startingIndex, registers);

            return registers;
        }

        public void Write(ushort startingIndex, ushort[] values)
        {
            _source.Write(startingIndex, values);

            OnDataWasWritten(startingIndex, values);
        }
    }
}
