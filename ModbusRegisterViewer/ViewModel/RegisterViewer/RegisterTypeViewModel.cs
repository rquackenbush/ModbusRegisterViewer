using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel.RegisterViewer
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
