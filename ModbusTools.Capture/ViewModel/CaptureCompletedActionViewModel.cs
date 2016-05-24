using GalaSoft.MvvmLight;
using ModbusTools.Capture.Common;
using ModbusTools.Capture.Model;

namespace ModbusTools.Capture.ViewModel
{
    public class CaptureCompletedActionViewModel : ViewModelBase
    {
        private readonly string _name;
        private readonly ICaptureViewerFactory _factory;

        public CaptureCompletedActionViewModel(string name, ICaptureViewerFactory factory)
        {
            _name = name;
            _factory = factory;
        }

        public string Name
        {
            get { return _name; }
        }

        public ICaptureViewerFactory Factory
        {
            get { return _factory; }
        }

    }
}
