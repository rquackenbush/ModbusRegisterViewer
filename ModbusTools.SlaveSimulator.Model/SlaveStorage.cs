using NModbus;

namespace ModbusTools.SlaveSimulator.Model
{
    public class SlaveStorage : ISlaveDataStore
    {
        private readonly SparsePointSource<bool> _coilDiscretes;
        private readonly SparsePointSource<bool> _coilInputs;
        private readonly SparsePointSource<ushort> _holdingRegisters;
        private readonly SparsePointSource<ushort> _inputRegisters;

        public SlaveStorage()
        {
            _coilDiscretes = new SparsePointSource<bool>();
            _coilInputs = new SparsePointSource<bool>();
            _holdingRegisters = new SparsePointSource<ushort>();
            _inputRegisters = new SparsePointSource<ushort>();
        }

        public SparsePointSource<bool> CoilDiscretes
        {
            get { return _coilDiscretes; }
        }

        public SparsePointSource<bool> CoilInputs
        {
            get { return _coilInputs; }
        }

        public SparsePointSource<ushort> HoldingRegisters
        {
            get { return _holdingRegisters; }
        }

        public SparsePointSource<ushort> InputRegisters
        {
            get { return _inputRegisters; }
        }

        IPointSource<bool> ISlaveDataStore.CoilDiscretes
        {
            get { return _coilDiscretes; }
        }

        IPointSource<bool> ISlaveDataStore.CoilInputs
        {
            get { return _coilInputs; }
        }

        IPointSource<ushort> ISlaveDataStore.HoldingRegisters
        {
            get { return _holdingRegisters; }
        }

        IPointSource<ushort> ISlaveDataStore.InputRegisters
        {
            get { return _inputRegisters; }
        }
    }
}