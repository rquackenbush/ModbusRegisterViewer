using FtdAdapter;
using ModbusTools.Common.Model;

namespace ModbusTools.Common.ViewModel
{
    internal class FtdModbusAdapterFactory : IModbusAdapterFactory
    {
        private readonly FtdDeviceInfo _deviceInfo;

        internal FtdModbusAdapterFactory(FtdDeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
        }

        public string DisplayName
        {
            get { return string.Format("{0} - {1}", _deviceInfo.Description, _deviceInfo.SerialNumber); }
        }

        public IMasterContextFactory CreateMasterContext()
        {
            return new RtuMasterContextFactory(_deviceInfo.SerialNumber);
        }
    }
}
