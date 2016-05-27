﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModbusTools.Capture.View;
using ModbusTools.Launcher.View;
using ModbusTools.SlaveExplorer.View;
using ModbusTools.SlaveSimulator.View;
using ModbusTools.SlaveViewer.View;
using Xceed.Wpf.AvalonDock.Layout;

namespace ModbusTools.Launcher
{
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

        private void AddTool(ToolType toolType)
        {
            switch (toolType)
            {
                case ToolType.SimpleSlaveExplorer:
                    AddTool<RegisterViewerView>("Simple Slave Explorer");
                    break;

                case ToolType.StructuredSlaveExplorer:

                    AddTool<SlaveExplorerView>("Structured Slave Explorer");
                    break;

                case ToolType.ModbusCapture:
                    AddTool<CaptureView>("Capture");
                    break;

                case ToolType.SlaveSimulator:
                    AddTool<SlaveSimulatorView>("Slave Simulator");
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

        private void AddNewSlaveExplorerMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AddTool(ToolType.SimpleSlaveExplorer);
        }

        private void AddNewStructuredSlaveExplorerMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AddTool(ToolType.StructuredSlaveExplorer);
        }

        private void AddNewModbusCaptureMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AddTool(ToolType.ModbusCapture);
        }

        private void AddNewSlaveSimulatorMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AddTool(ToolType.SlaveSimulator);
        }

        private void ToolLauncherMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ShowToolLauncher();
        }
    }
}
