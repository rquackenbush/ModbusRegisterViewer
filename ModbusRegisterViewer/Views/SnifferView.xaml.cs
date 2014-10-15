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
using System.Windows.Shapes;
using FtdAdapter;
using Modbus.Data;
using Modbus.Device;
using ModbusRegisterViewer.ViewModel;
using ModbusRegisterViewer.ViewModel.Sniffer;

namespace ModbusRegisterViewer.Views
{
    /// <summary>
    /// Interaction logic for SnifferView.xaml
    /// </summary>
    public partial class SnifferView : Window
    {
        //private FtdUsbPort _port;
        //private ModbusSlave _slave;

        private SnifferViewModel _viewModel;

        public SnifferView()
        {
            InitializeComponent();
        }


        private void SnifferView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.SelectionChanged -= OnSelectedPackatChanged;

            _viewModel = this.DataContext as SnifferViewModel;

            if (_viewModel != null)
                _viewModel.SelectionChanged += OnSelectedPackatChanged;
        }

        private void SetDetailVisual(UIElement element)
        {
            this.GenericDisplayContainer.Children.Clear();

            if (element == null)
                return;

            this.GenericDisplayContainer.Children.Add(element);
        }

        void OnSelectedPackatChanged(object sender, EventArgs e)
        {
            var element = PackatDetailViewFactory.CreateView(_viewModel.SelectedPacket);

            SetDetailVisual(element);
        }
    }
}
