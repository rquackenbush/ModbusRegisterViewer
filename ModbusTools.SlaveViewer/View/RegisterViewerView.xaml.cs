using System;
using System.IO;
using System.Windows;
using ModbusTools.Common;
using ModbusTools.Common.Services;
using ModbusTools.SlaveViewer.ViewModel;

namespace ModbusTools.SlaveViewer.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RegisterViewerView : Window
    {
        public RegisterViewerView()
        {
            InitializeComponent();

            var preferencesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ModbusTools", "registerViewerPreferences.xml");

            var messageBoxService = new MessageBoxService(this);
            var preferences = new Preferences(preferencesPath);

            DataContext = new RegisterViewerViewModel(messageBoxService, preferences);
        }

        private void RegisterViewerView_OnClosed(object sender, EventArgs e)
        {
            var viewModel = DataContext as RegisterViewerViewModel;

            if (viewModel != null)
            {
                viewModel.Closed();
            }
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
