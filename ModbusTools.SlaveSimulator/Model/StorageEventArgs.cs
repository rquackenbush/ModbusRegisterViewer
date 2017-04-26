using System;

namespace ModbusTools.SlaveSimulator.Model
{
    public class StorageEventArgs<TPoint> : EventArgs
    {
        private readonly PointOperation _pointOperation;
        private readonly ushort _startingAddress;
        private readonly TPoint[] _points;

        public StorageEventArgs(PointOperation pointOperation, ushort startingAddress, TPoint[] points)
        {
            _pointOperation = pointOperation;
            _startingAddress = startingAddress;
            _points = points;
        }

        public ushort StartingAddress
        {
            get { return _startingAddress; }
        }

        public TPoint[] Points
        {
            get { return _points; }
        }

        public PointOperation Operation
        {
            get { return _pointOperation; }
        }
    }
}
