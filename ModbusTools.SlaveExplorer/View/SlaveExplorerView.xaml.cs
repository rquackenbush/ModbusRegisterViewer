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
                Title = slaveViewModel.Name,
                Content = new SlaveView()
                {
                    DataContext = slaveViewModel
                },                
            };

            layoutDocument.Closed += LayoutDocumentOnClosed;

            MainDocumentPane.Children.Add(layoutDocument);
        }

        private void LayoutDocumentOnClosed(object sender, EventArgs eventArgs)
        {
            var layoutDocument = sender as LayoutDocument;

            if (layoutDocument == null)
                return;

            var element = layoutDocument.Content as FrameworkElement;

            if (element == null)
                return;

            var viewModel = element.DataContext as SlaveViewModel;

            if (viewModel == null)
                return;

            this.PerformViewModelAction<SlaveExplorerViewModel>(vm => vm.RemoveSlave(viewModel));
        }
    }
}
