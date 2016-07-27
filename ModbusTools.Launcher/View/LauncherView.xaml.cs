using System.Windows;
using System.Windows.Input;

namespace ModbusTools.Launcher.View
{
    /// <summary>
    /// Interaction logic for LauncherView.xaml
    /// </summary>
    public partial class LauncherView
    {
        public LauncherView()
        {
            InitializeComponent();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        public ToolType? SelectedToolType { get; private set; }

        private void SelectTool(ToolType toolType)
        {
            SelectedToolType = toolType;

            DialogResult = true;
            Close();
        }

        private void LaunchRegisterViewer(object sender, RoutedEventArgs e)
        {
            SelectTool(ToolType.SimpleSlaveExplorer);
        }
      
        private void LaunchCapture(object sender, RoutedEventArgs e)
        {
            SelectTool(ToolType.ModbusCapture);
        }

        private void LaunchMultipleSlaveSimulator(object sender, RoutedEventArgs e)
        {
            SelectTool(ToolType.SlaveSimulator);
        }

        private void LaunchSlaveExplorer(object sender, RoutedEventArgs e)
        {
            SelectTool(ToolType.StructuredSlaveExplorer);
        }

        private void LaunchSlaveScanner(object sender, RoutedEventArgs e)
        {
            SelectTool(ToolType.SlaveScanner);
        }
    }
}
