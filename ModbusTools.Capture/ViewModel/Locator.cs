namespace ModbusTools.Capture.ViewModel
{
    public static class Locator
    {
        public static CaptureViewModel Capture
        {
            get { return new CaptureViewModel(); }
        }
    }
}
