using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.Common
{
    public static class ModbusAdapatersViewModelExtensions
    {
        public static void ApplyPreferences(this ModbusAdaptersViewModel modbusAdaptersViewModel, IPreferences preferences, string key)
        {
            var displayName = preferences[key];

            var item = modbusAdaptersViewModel.Adapters.FirstOrDefault(a => string.Compare(a.DisplayName, displayName, true) == 0);

            if (item != null)
            {
                modbusAdaptersViewModel.SelectedAdapter = item;
            }
        }

        public static void GetPreferences(this ModbusAdaptersViewModel modbusAdaptersViewModel, IPreferences preferences, string key)
        {
            var selectedAdapter = modbusAdaptersViewModel.SelectedAdapter;

            if (selectedAdapter != null)
            {
                preferences[key] = selectedAdapter.DisplayName;
            }
        }
    }
}
