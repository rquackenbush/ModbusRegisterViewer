using GalaSoft.MvvmLight;
using ModbusTools.Capture.Model;

namespace ModbusTools.Capture.ViewModel
{
    public class CaptureCompletedActionViewModel : ViewModelBase
    {
        private readonly string _name;
        private readonly CaptureViewerFactory _factory;

        public CaptureCompletedActionViewModel(string name, CaptureViewerFactory factory)
        {
            _name = name;
            _factory = factory;
        }

        public string Name
        {
            get { return _name; }
        }

        public CaptureViewerFactory Factory
        {
            get { return _factory; }
        }

    }
}
