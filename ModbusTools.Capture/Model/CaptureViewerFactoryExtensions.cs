using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusTools.Capture.Common;

namespace ModbusTools.Capture.Model
{
    public static class CaptureViewerFactoryExtensions
    {
        public static void ShowCapture(this ICaptureViewerFactory factory, string path)
        {
            var window = factory.Open(path);

            window.Show();
        }
    }
}
