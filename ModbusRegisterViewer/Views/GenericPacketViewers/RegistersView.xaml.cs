using System.Collections.Generic;
using System.Linq;
using ModbusRegisterViewer.ViewModel.Sniffer;
using ModbusTools.Common.ViewModel;

namespace ModbusRegisterViewer.Views.GenericPacketViewers
{
    /// <summary>
    /// Interaction logic for ReadRegistersResponseView.xaml
    /// </summary>
    public partial class RegistersView
    {
        private readonly PacketViewModel _packet;

        public RegistersView(PacketViewModel packet, ushort startingRegisterIndex, IEnumerable<ushort> values)
        {
            InitializeComponent();

            _packet = packet;
            
            ushort registerIndex = startingRegisterIndex;

            var registers = values.Select(v => new RegisterViewModel(registerIndex++, v));

            this.RegistersDataGrid.ItemsSource = registers;
        }
    }
}
