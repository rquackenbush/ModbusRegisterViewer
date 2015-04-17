using ModbusRegisterViewer.ViewModel.Sniffer;

namespace ModbusRegisterViewer.Model
{
    public interface IPacketSnifferStateMachine
    {
        PacketViewModel ProcessSample(Sample sample);
    }
}
