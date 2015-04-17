using System.Windows;
using ModbusRegisterViewer.Views;

namespace ModbusRegisterViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RegisterViewerView : Window
    {
        public RegisterViewerView()
        {
            InitializeComponent();
        }

        private void StartWindow()
        {
            var window = new SnifferView();

            window.Show();
        }
    }

    
}
