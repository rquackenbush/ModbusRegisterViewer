using System.Linq;
using System.Windows.Controls;
using ModbusRegisterViewer.ViewModel.RegisterViewer;
using ModbusRegisterViewer.ViewModel.Sniffer;

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
