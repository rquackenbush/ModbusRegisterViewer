using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ModbusTools.Common;

namespace ModbusRegisterViewer.Model
{
    [DataContract(Namespace = DataContractConstants.DataContractNamespace)]
    public class Snapshot
    {
        [DataMember]
        public RegisterType RegisterType { get; set; }

        [DataMember]
        public byte SlaveId { get; set; }

        [DataMember]
        public ushort StartingRegister { get; set; }

        [DataMember]
        public ushort[] Registers { get; set; }
    }
}
