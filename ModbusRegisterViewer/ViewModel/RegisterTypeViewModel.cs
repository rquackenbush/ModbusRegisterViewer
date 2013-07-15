using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    public class RegisterTypeViewModel
    {
        public RegisterTypeViewModel(RegisterType registerType, string display)
        {
            this.RegisterType = registerType;
            this.Display = display;
        }

        public string Display { get; private set; }

        public RegisterType RegisterType { get; private set; }
    }
}
