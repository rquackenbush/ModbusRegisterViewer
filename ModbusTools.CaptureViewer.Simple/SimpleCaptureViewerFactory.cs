using System.Windows;
using ModbusTools.Capture.Common;
using ModbusTools.CaptureViewer.Simple.View;
using ModbusTools.CaptureViewer.Simple.ViewModel;

namespace ModbusTools.CaptureViewer.Simple
{
    public class SimpleCaptureViewerFactory : ICaptureViewerFactory
    {
        public string Name
        {
            get { return "Simple"; }
        }

        public Window Open(string filename)
        {
            var viewModel = new SimpleTextCaptureViewModel(filename);

            return new SimpleTextCaptureView()
            {
                DataContext = viewModel
            };
        }
    }
}