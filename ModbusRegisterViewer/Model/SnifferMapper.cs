using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusRegisterViewer.ViewModel;

namespace ModbusRegisterViewer.Model
{
    public static class SnifferMapper
    {
        public static SnifferCapture ToModel(this IEnumerable<PacketViewModel> packets)
        {
            return new SnifferCapture()
                {
                    Packets = packets.Select(p => p.ToModel()).ToArray()
                };
        }

        public static Packet ToModel(this PacketViewModel packet)
        {
            if (packet == null)
                return null;

            return new Packet()
                {
                    Message = packet.Message,
                    Timestamp = packet.Timestamp,
                    Direction = packet.Direction,
                    PreviousPacket = packet.PreviousPacket.ToModel()
                };
        }

        public static IEnumerable<PacketViewModel> FromModel(this SnifferCapture capture)
        {
            if (capture == null)
               throw new ArgumentNullException("capture");

            return capture.Packets.Select(p => p.FromModel());
        }

        public static PacketViewModel FromModel(this Packet packet)
        {
            if (packet == null)
                return null;

            //Check to see if this is a valid packet.
            if (string.IsNullOrEmpty(packet.InvalidReason))
            {
                return PacketViewModel.CreateValidPacket(packet.Timestamp,
                                                         packet.Message,
                                                         packet.Direction,
                                                         packet.PreviousPacket.FromModel());
            }

            //This was an invalid packet.
            return PacketViewModel.CreateInvalidPacket(packet.Timestamp,
                                                       packet.Message,
                                                       packet.InvalidReason);
        }
    }
}
