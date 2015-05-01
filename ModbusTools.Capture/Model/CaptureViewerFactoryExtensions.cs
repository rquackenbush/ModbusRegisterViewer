using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTools.Capture.Model
{
    public static class CaptureViewerFactoryExtensions
    {
        public static void ShowCapture(this CaptureViewerFactory factory, string path)
        {
            var window = factory.Open(path);

            window.Show();
        }
    }
}
