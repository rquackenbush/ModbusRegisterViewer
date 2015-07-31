using System;
using System.Linq;
using System.Windows;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;

namespace ModbusTools.SlaveExplorer.View
{
    /// <summary>
    /// Interaction logic for SlaveExplorerView.xaml
    /// </summary>
    public partial class SlaveExplorerView
    {
        public SlaveExplorerView()
        {
            InitializeComponent();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SlaveExplorerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.PerformViewModelAction<SlaveExplorerViewModel>(vm =>
            {
                vm.SlaveAdded += SlaveAdded;

                vm.SlaveRemoved += SlaveRemoved;
            });
        }

        private bool IsSlaveDocument(LayoutDocument layoutDocument, SlaveViewModel slave)
        {
            if (layoutDocument == null)
                return false;

            var element = layoutDocument.Content as FrameworkElement;

            if (element == null)
                return false;

            var viewModel = element.DataContext as SlaveViewModel;

            return viewModel == slave;
        }

        private LayoutDocument GetSlaveDocument(SlaveViewModel slave)
        {
            return MainDocumentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => IsSlaveDocument(d, slave));
        }

        private void SlaveRemoved(object sender, SlaveViewModel slaveViewModel)
        {
            var document = GetSlaveDocument(slaveViewModel);

            if (document == null)
                return;

            document.Closed -= LayoutDocumentOnClosed;

            MainDocumentPane.Children.Remove(document);
        }

        private void SlaveAdded(object sender, SlaveViewModel slaveViewModel)
        {
            var layoutDocument = new LayoutDocument()
            {
                Title = slaveViewModel.DisplayName,
                Content = new SlaveView()
                {
                    DataContext = slaveViewModel
                },
            };

            layoutDocument.Closed += LayoutDocumentOnClosed;
            layoutDocument.Closing += LayoutDocumentClosing;

            MainDocumentPane.Children.Add(layoutDocument);

            slaveViewModel.PropertyChanged += slaveViewModel_PropertyChanged;
        }

        void slaveViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var slaveViewModel = sender as SlaveViewModel;

            if (slaveViewModel == null)
                return;

            if (e.PropertyName == "DisplayName")
            {
                var layoutDocument = GetSlaveDocument(slaveViewModel);

                if (layoutDocument != null)
                {
                    layoutDocument.Title = slaveViewModel.DisplayName;
                }
                    
            }
        }

        private SlaveViewModel GetSlaveViewModelFromSender(object sender)
        {
            var layoutDocument = sender as LayoutDocument;

            if (layoutDocument == null)
                return null;

            var element = layoutDocument.Content as FrameworkElement;

            if (element == null)
                return null;

            return element.DataContext as SlaveViewModel;
        }

        void LayoutDocumentClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = GetSlaveViewModelFromSender(sender);

            if (viewModel == null)
                return;

            var message = string.Format("Delete Modbus slave '{0}'?", viewModel.Name);

            var result = MessageBox.Show(this, message, "Delete?", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void LayoutDocumentOnClosed(object sender, EventArgs eventArgs)
        {
            var viewModel = GetSlaveViewModelFromSender(sender);

            if (viewModel == null)
                return;

            this.PerformViewModelAction<SlaveExplorerViewModel>(vm => vm.RemoveSlave(viewModel));
        }
    }
}
