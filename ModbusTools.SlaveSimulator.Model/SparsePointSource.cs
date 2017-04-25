using System;
using System.Collections.Generic;
using NModbus;

namespace ModbusTools.SlaveSimulator.Model
{
    /// <summary>
    /// Sparse storage for registers
    /// </summary>
    public class SparsePointSource<TPoint> : IPointSource<TPoint>
    {
        private readonly Dictionary<ushort, TPoint> _values = new Dictionary<ushort, TPoint>();

        public event EventHandler<StorageEventArgs<TPoint>> StorageOperationOccurred;

        /// <summary>
        /// Gets or sets the value of an individual point wih tout 
        /// </summary>
        /// <param name="registerIndex"></param>
        /// <returns></returns>
        public TPoint this[ushort registerIndex]
        {
            get
            {
                TPoint value;

                if (_values.TryGetValue(registerIndex, out value))
                    return value;

                return default(TPoint);
            }
            set
            {
                _values[registerIndex] = value; 
            }
        }

        public TPoint[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            var points = new TPoint[numberOfPoints];

            for (ushort index = 0; index < numberOfPoints; index++)
            {
                points[index] = this[(ushort)(index + startAddress)];
            }

            StorageOperationOccurred?.Invoke(this, new StorageEventArgs<TPoint>(PointOperation.Read, startAddress, points));

            return points;
        }

        public void WritePoints(ushort startAddress, TPoint[] points)
        {
            for (ushort index = 0; index < points.Length; index++)
            {
                this[(ushort)(index + startAddress)] = points[index];
            }

            StorageOperationOccurred?.Invoke(this, new StorageEventArgs<TPoint>(PointOperation.Write, startAddress, points));
        }
    }
}
