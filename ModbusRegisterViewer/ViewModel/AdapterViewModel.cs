//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FtdAdapter;
//using GalaSoft.MvvmLight;

//namespace ModbusRegisterViewer.ViewModel
//{
//    public class AdapterViewModel : ViewModelBase
//    {
//        private readonly FtdDeviceInfo _deviceInfo;

//        public AdapterViewModel(FtdDeviceInfo deviceInfo)
//        {
//            _deviceInfo = deviceInfo;
//        }

//        public uint Id
//        {
//            get { return _deviceInfo.Id; }
//        }

//        public string SerialNumber
//        {
//            get { return _deviceInfo.SerialNumber; }
//        }

//        public string Description 
//        {
//            get { return string.Format("{0} - {1}", _deviceInfo.Description, _deviceInfo.SerialNumber); }
//        }
//    }
//}
