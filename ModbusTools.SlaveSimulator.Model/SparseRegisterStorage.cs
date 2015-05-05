using System.Collections.Generic;

namespace ModbusTools.SlaveSimulator.Model
{
    /// <summary>
    /// Sparse storage for registers
    /// </summary>
    public class SparseRegisterStorage : IRegisterStorage
    {
        private readonly Dictionary<ushort, ushort> _values = new Dictionary<ushort, ushort>(); 

        public ushort this[ushort registerIndex]
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
    }
}
