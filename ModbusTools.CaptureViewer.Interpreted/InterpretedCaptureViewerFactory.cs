using System.Windows;
using ModbusTools.Capture.Common;
using ModbusTools.CaptureViewer.Interpreted.View;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;

namespace ModbusTools.CaptureViewer.Interpreted
{
    public class InterpretedCaptureViewerFactory : ICaptureViewerFactory
    {
        public string Name
        {
            get { return "Interpreted"; }
        }

        public Window Open(string filename)
        {
            var viewModel = new InterpretedCaptureViewModel(filename);

            return new InterpretedCaptureView()
            {
                DataContext = viewModel
            };
        }
    }
}