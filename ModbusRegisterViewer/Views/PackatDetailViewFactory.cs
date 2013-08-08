using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModbusRegisterViewer.Model;
using ModbusRegisterViewer.ViewModel;
using ModbusRegisterViewer.Views.GenericPacketViewers;

namespace ModbusRegisterViewer.Views
{
    public static class PackatDetailViewFactory
    {
        public static UIElement CreateView(PacketViewModel packet)
        {
            if (packet == null)
                return null;

            switch (packet.Function)
            {
                case FunctionCodes.ReadHoldingRegisters:

                    if (packet.Direction == MessageDirection.Response)
                        return new ReadRegistersResponseView(packet);

                    break;
            }

            return null;
        }
    }
}
