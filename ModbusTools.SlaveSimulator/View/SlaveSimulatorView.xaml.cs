using System;
using System.Windows;
using ModbusTools.Common;
using ModbusTools.SlaveSimulator.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;

namespace ModbusTools.SlaveSimulator.View
{
    /// <summary>
    /// Interaction logic for SlaveSimulatorView.xaml
    /// </summary>
    public partial class SlaveSimulatorView : Window
    {
        public SlaveSimulatorView()
        {
            InitializeComponent();
        }

        private void AddSlave(SlaveViewModel slave)
        {
            var view = new SlaveView()
            {
                DataContext = slave
            };

            var layoutDocument = new LayoutDocument()
            {
                Content = view,
                Title = "Slave"
            };

            layoutDocument.Closing += SlaveClosing;
            layoutDocument.Closed += SlaveClosed;

            //Add it to the UI
            SlaveDocumentPane.Children.Add(layoutDocument);

            //Select it
            SlaveDocumentPane.SelectedContentIndex = SlaveDocumentPane.ChildrenCount - 1;
        }

        private void PerformViewModelAction(Action<SlaveSimulatorViewModel> action)
        {
            var viewModel = DataContext as SlaveSimulatorViewModel;

            if (viewModel == null)
                throw new Exception("Unable to find view model");

            action(viewModel);
        }

        private void SlaveClosed(object sender, EventArgs eventArgs)
        {
            PerformViewModelAction(viewModel =>
            {
                var slaveContainer = sender as LayoutDocument;

                if (slaveContainer == null)
                    return;

                var slaveView = slaveContainer.Content as SlaveView;

                if (slaveView == null)
                    return;

                var slave = slaveView.DataContext as SlaveViewModel;

                if (slave == null)
                    return;

                viewModel.OnSlaveClosed(slave);

            });
        }

        private void SlaveClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PerformViewModelAction(viewModel =>
            {
                if (!viewModel.CanCloseSlave())
                {
                    e.Cancel = true;
                }    
            });
        }

        private void OnSlaveCreated(object sender, SlaveEvent slaveEvent)
        {
            AddSlave(slaveEvent.Slave);            
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as SlaveSimulatorViewModel;

            if (viewModel != null)
            {
                viewModel.SlaveCreated += OnSlaveCreated;

                foreach (var slave in viewModel.Slaves)
                {
                    AddSlave(slave);
                }
            }

            if (!FtdiLatencyConfigurator.CheckForLatency(this))
                Close();
        }
    }
}
