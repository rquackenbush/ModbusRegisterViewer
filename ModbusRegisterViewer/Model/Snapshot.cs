using System.Runtime.Serialization;

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
