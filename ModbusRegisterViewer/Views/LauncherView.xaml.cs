using System.Windows;
using System.Windows.Input;
using ModbusRegisterViewer.ViewModel;
using ModbusTools.Capture.View;
using ModbusTools.SlaveSimulator.View;

namespace ModbusRegisterViewer.Views
{
    /// <summary>
    /// Interaction logic for LauncherView.xaml
    /// </summary>
    public partial class LauncherView : Window
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

        private void LaunchView<TView>() 
            where TView : Window, new()
        {
            var view = new TView();

            view.Show();
        }

        private void LaunchRegisterViewer(object sender, RoutedEventArgs e)
        {
            LaunchView<ModbusTools.SlaveViewer.View.RegisterViewerView>();
        }
      
        private void LaunchCapture(object sender, RoutedEventArgs e)
        {
            LaunchView<CaptureView>();
        }

        private void LaunchMultipleSlaveSimulator(object sender, RoutedEventArgs e)
        {
            LaunchView<SlaveSimulatorView>();
        }

        private void AboutButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            var aboutView = new AboutView()
            {
                DataContext = new AboutViewModel(),
                Owner = this
            };

            aboutView.ShowDialog();
        }
    }
}
