using NModbus;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    public class CoilsViewModel : PointsViewModelBase<CoilViewModel, bool>
    {
        public CoilsViewModel(ISlaveExplorerContext context) 
            : base(context)
        {
        }

        protected override bool[] ReadCore(IModbusMaster modbusMaster, byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            return modbusMaster.ReadCoils(slaveId, startAddress, numberOfPoints);
        }

        protected override void WriteCore(IModbusMaster modbusMaster, byte slaveId, ushort startAddress, bool[] values)
        {
            modbusMaster.WriteMultipleCoils(slaveId, startAddress, values);
        }

        public override bool IsWriteable
        {
            get { return true; }
        }

        public override bool SupportsBlockSize
        {
            get { return false; }
        }
    }

   
}