using System.Windows;

namespace ModbusTools.SlaveExplorer.View
{
    /// <summary>
    /// Interaction logic for SlaveExplorerView.xaml
    /// </summary>
    public partial class SlaveExplorerView : Window
    {
        public SlaveExplorerView()
        {
            InitializeComponent();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
