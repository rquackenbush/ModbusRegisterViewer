using System.Collections.Generic;
using System.Security.AccessControl;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class ReadHoldingRegistersService : ReadRegistersService
    {
        public ReadHoldingRegistersService() 
            : base(FunctionCode.ReadHoldingRegisters)
        {
        }

     
    }
}