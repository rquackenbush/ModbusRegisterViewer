namespace ModbusTools.Scanner.ViewModel
{
    public static class ScannerViewModelLocator
    {
        public static SlaveScannerViewModel Scanner
        {
            get {  return new SlaveScannerViewModel(); }
        }
    }
}