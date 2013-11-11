using ModbusRegisterViewer.ViewModel.Sniffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public interface IPacketSnifferStateMachine
    {
        PacketViewModel ProcessSample(Sample sample);
    }
}
