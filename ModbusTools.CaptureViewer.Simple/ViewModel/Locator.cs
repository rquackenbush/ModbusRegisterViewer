using GalaSoft.MvvmLight;

namespace ModbusTools.CaptureViewer.Simple.ViewModel
{
    public static class Locator
    {
        public static SimpleTextCaptureViewModel Main
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                    return new SimpleTextCaptureViewModel();

                return null;
            }
        }
    }
}
