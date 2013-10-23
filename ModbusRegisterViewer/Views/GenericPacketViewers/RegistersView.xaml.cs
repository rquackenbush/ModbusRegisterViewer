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
using ModbusRegisterViewer.ViewModel.RegisterViewer;
using ModbusRegisterViewer.ViewModel.Sniffer;
using Unme.Common;

namespace ModbusRegisterViewer.Views.GenericPacketViewers
{
    /// <summary>
    /// Interaction logic for ReadRegistersResponseView.xaml
    /// </summary>
    public partial class RegistersView : UserControl
    {
        private readonly PacketViewModel _packet;

        public RegistersView(PacketViewModel packet, ushort startingRegister, ushort[] values)
        {
            InitializeComponent();

            _packet = packet;
            
            byte length = packet.Message[2];

            

            ushort registerNumber = startingRegister;

            var registers = values.Select(v => new RegisterViewModel(registerNumber++, v));

            this.RegistersDataGrid.ItemsSource = registers;
        }
    }
}
