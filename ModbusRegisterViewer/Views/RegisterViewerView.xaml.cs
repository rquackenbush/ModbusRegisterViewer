using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using ModbusRegisterViewer.Model;
using ModbusRegisterViewer.Views;

namespace ModbusRegisterViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartWindow()
        {
            var window = new SnifferView();

            window.Show();
        }

        private void ModbusSniffer_MenuClick(object sender, RoutedEventArgs e)
        {
            //Thread thread = new Thread(StartWindow);
            //thread.SetApartmentState(ApartmentState.STA);

            //thread.Start();

            StartWindow();
        }
    }

    
}
