using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Modbus.Utility;
using ModbusRegisterViewer.ViewModel;
using Unme.Common;

namespace ModbusRegisterViewer.Views.GenericPacketViewers
{
    /// <summary>
    /// Interaction logic for ReadRegistersResponseView.xaml
    /// </summary>
    public partial class ReadRegistersResponseView : UserControl
    {
        private readonly PacketViewModel _packet;

        public ReadRegistersResponseView(PacketViewModel packet)
        {
            InitializeComponent();

            _packet = packet;
            
            byte length = packet.Message[2];

            var registerBytes = packet.Message.Skip(3).Take(length).ToArray();

            var rawRegisters = ModbusUtility.NetworkBytesToHostUInt16(registerBytes);

            ushort registerNumber = 0;

            if (packet.PreviousPacket != null && packet.PreviousPacket.Message.Length > 3)
            {
                var registerNumberBuffer = packet.PreviousPacket.Message.Slice(2, 2).ToArray();

                registerNumber = ModbusUtility.NetworkBytesToHostUInt16(registerNumberBuffer)[0];

                registerNumber++;
            }

            var registers = rawRegisters.Select(v => new RegisterViewModel(registerNumber++, v));

            this.RegistersDataGrid.ItemsSource = registers;
        }
    }
}
