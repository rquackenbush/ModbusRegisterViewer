using System.Windows;
using ModbusTools.Common;

namespace ModbusTools.Capture.View
{
    /// <summary>
    /// Interaction logic for CaptureView.xaml
    /// </summary>
    public partial class CaptureView
    {
        public CaptureView()
        {
            InitializeComponent();
        }

        private void CaptureView_OnLoaded(object sender, RoutedEventArgs e)
        {
            FtdiConfiguration.CheckForLatency(null);
        }
    }
}
