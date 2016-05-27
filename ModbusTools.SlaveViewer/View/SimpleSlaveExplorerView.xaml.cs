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
    public partial class SimpleSlaveExplorerView
    {
        public SimpleSlaveExplorerView()
        {
            InitializeComponent();

            var preferencesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ModbusTools", "registerViewerPreferences.xml");

            var messageBoxService = new MessageBoxService();
            var preferences = new Preferences(preferencesPath);

            DataContext = new SlaveExplorerViewModel(messageBoxService, preferences);
        }
    }
}
