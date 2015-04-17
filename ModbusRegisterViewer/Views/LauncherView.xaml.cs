using System.Windows;
using System.Windows.Input;

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
            this.Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void LaunchView<TView>() 
            where TView : Window, new()
        {
            var view = new TView();

            view.Show();

            //Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint<TView>));
            //newWindowThread.SetApartmentState(ApartmentState.STA);
            //newWindowThread.IsBackground = true;
            //newWindowThread.Start();
        }

        //private void ThreadStartingPoint<TView>()
        //    where TView : Window, new()
        //{
        //    var view = new TView();
        //    view.Show();
        //    System.Windows.Threading.Dispatcher.Run();
        //}

        private void LaunchRegisterViewer(object sender, RoutedEventArgs e)
        {
            LaunchView<ModbusTools.SlaveViewer.View.RegisterViewerView>();
        }

        private void LaunchSpy(object sender, RoutedEventArgs e)
        {
            LaunchView<SnifferView>();
        }

        private void LaunchSlaveSimulator(object sender, RoutedEventArgs e)
        {
            LaunchView<SlaveSimulatorView>();
        }
    }
}
