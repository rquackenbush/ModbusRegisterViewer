using System.Windows.Controls;
using System.Windows.Input;

namespace ModbusTools.StructuredSlaveExplorer.View
{
    /// <summary>
    /// Interaction logic for SlaveView.xaml
    /// </summary>
    public partial class SlaveView
    {
        public SlaveView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// http://stackoverflow.com/a/6693503/232566
        /// </remarks>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
