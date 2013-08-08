using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    [DataContract(Namespace = DataContractConstants.DataContractNamespace)]
    public class SnifferCapture
    {
        [DataMember]
        public Packet[] Packets { get; set; }
    }

    [DataContract(Namespace = DataContractConstants.DataContractNamespace)]
    public class Packet
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public byte[] Message { get; set; }

        [DataMember]
        public MessageDirection Direction { get; set; }

        [DataMember]
        public Packet PreviousPacket { get; set; }

        [DataMember]
        public string InvalidReason { get; set; }
    }
}
