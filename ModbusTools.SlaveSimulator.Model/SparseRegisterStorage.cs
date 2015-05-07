using System.Collections.Generic;

namespace ModbusTools.SlaveSimulator.Model
{
    /// <summary>
    /// Sparse storage for registers
    /// </summary>
    public class SparseRegisterStorage : IRegisterStorage
    {
        private readonly Dictionary<ushort, ushort> _values = new Dictionary<ushort, ushort>(); 

        private ushort this[ushort registerIndex]
        {
            get
            {
                ushort value;

                if (_values.TryGetValue(registerIndex, out value))
                    return value;

                return 0;
            }
            set { _values[registerIndex] = value; }
        }


        public ushort[] Read(ushort startingIndex, ushort numberOfRegisters)
        {
            var registers = new ushort[numberOfRegisters];

            for (ushort index = 0; index < numberOfRegisters; index++)
            {
                registers[index] = this[(ushort)(index + startingIndex)];
            }

            return registers;
        }

        public void Write(ushort startingIndex, ushort[] values)
        {
            for (ushort index = 0; index < values.Length; index++)
            {
                this[(ushort)(index + startingIndex)] = values[index];
            }
        }
    }
}
