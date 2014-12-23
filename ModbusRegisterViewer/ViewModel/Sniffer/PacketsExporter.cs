using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.ViewModel.Sniffer
{
    public static class PacketsExporter
    {
        public static void Export(string filename, ObservableCollection<PacketViewModel> packets)
        {
            using(var writer = File.CreateText(filename))
            {
                writer.WriteLine("Time,Milliseconds,Address,Func,Description,Direction,Interval,CRC,Size");

                foreach(var packet in packets)
                {
                    writer.WriteLine("{0},{1},{2},\"{3}\",\"{4}\",{5},{6},{7}",
                        packet.Time,
                        packet.Address,
                        packet.Function,
                        packet.Type,
                        packet.Direction,
                        packet.ResponseTime,
                        packet.CRC,
                        packet.Bytes);
                }
            }
        }
    }
}
