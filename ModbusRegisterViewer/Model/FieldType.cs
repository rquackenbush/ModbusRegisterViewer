using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public abstract class FieldTypeBase
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract long GetValueFromRange(ushort startingAddress, ushort[] values, byte fixedDecimalPlaces);

        public abstract void SetValueOnRange(ushort startingAddress, ushort[] values, decimal value, byte fixedDecimalPlaces);
    }
}
