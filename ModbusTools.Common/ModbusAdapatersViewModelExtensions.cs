using System.Linq;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.Common
{
    public static class ModbusAdapatersViewModelExtensions
    {
        public static void ApplyPreferences(this ModbusAdaptersViewModel modbusAdaptersViewModel, IPreferences preferences, string key)
        {
            var displayName = preferences[key];

            var item = modbusAdaptersViewModel.Ports.FirstOrDefault(a => string.Compare(a, displayName, true) == 0);

            if (item != null)
            {
                modbusAdaptersViewModel.SelectedPort = item;
            }
        }

        public static void GetPreferences(this ModbusAdaptersViewModel modbusAdaptersViewModel, IPreferences preferences, string key)
        {
            var selectedPort = modbusAdaptersViewModel.SelectedPort;

            if (selectedPort != null)
            {
                preferences[key] = selectedPort;
            }
        }
    }
}
