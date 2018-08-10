using System;
using System.Windows;
using System.Windows.Media;
using ModbusTools.Capture.View;
using ModbusTools.Launcher.View;
using ModbusTools.Scanner.View;
using ModbusTools.SimpleSlaveExplorer.View;
using ModbusTools.SlaveSimulator.View;
using ModbusTools.StructuredSlaveExplorer.View;
using Xceed.Wpf.AvalonDock.Layout;

namespace ModbusTools.Launcher
{
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for MainWIndow.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        public ApplicationView()
        {
            InitializeComponent();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddTool<TView>(string name) 
            where TView : Visual, new()
        {
            var view = new TView();

            var layoutDocument = new LayoutDocument()
            {
                Content = view,
                Title = name
            };

            MainDocumentPane.Children.Add(layoutDocument);
            MainDocumentPane.SelectedContentIndex = MainDocumentPane.ChildrenCount - 1;
        }

        private void AddToolWithTitle<TView>(string name)
            where TView : FrameworkElement, new()
        {
            var view = new TView();

            var layoutDocument = new LayoutDocument()
            {
                Content = view,
            };

            Binding binding = new Binding("Title")
            {
                Source = view.DataContext
            };

            BindingOperations.SetBinding(layoutDocument, LayoutContent.TitleProperty, binding);

            MainDocumentPane.Children.Add(layoutDocument);
            MainDocumentPane.SelectedContentIndex = MainDocumentPane.ChildrenCount - 1;
        }


        private void AddTool(ToolType toolType)
        {
            switch (toolType)
            {
                case ToolType.SimpleSlaveExplorer:
                    AddTool<SimpleSlaveExplorerView>("Simple Slave Explorer");
                    break;

                case ToolType.StructuredSlaveExplorer:

                    AddTool<StructuredSlaveExplorerView>("Structured Slave Explorer");
                    break;

                case ToolType.ModbusCapture:
                    AddTool<CaptureView>("Modbus Capture");
                    break;

                case ToolType.SlaveSimulator:
                    AddToolWithTitle<SlaveSimulatorView>("Slave Simulator");
                    break;

                case ToolType.SlaveScanner:
                    AddTool<SlaveScannerView>("Slave Scanner");
                    break;
            }
        }

        private void ShowToolLauncher()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                var launcher = new LauncherView()
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };

                if (launcher.ShowDialog() == true)
                {
                    if (launcher.SelectedToolType.HasValue)
                    {
                        AddTool(launcher.SelectedToolType.Value);
                    }
                }
            }));
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var about = new AboutView()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
            };

            about.ShowDialog();
        }

        private void ApplicationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ShowToolLauncher();
        }

        private void ToolLauncherMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ShowToolLauncher();
        }
    }
}
