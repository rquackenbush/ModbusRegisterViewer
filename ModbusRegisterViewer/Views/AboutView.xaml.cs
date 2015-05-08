using System.Windows.Controls;

namespace ModbusRegisterViewer.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();


        }

        private void OnOkClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

    }
}
