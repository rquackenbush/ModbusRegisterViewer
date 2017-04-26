using System;
using System.IO;
using Cas.Common.WPF;
using ModbusTools.Common;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    public static class ViewModelLocator
    {
        public static SlaveExplorerViewModel SlaveExplorer
        {
            get
            {
                var preferencesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ModbusTools", "registerViewerPreferences.xml");

                var messageBoxService = new MessageBoxService();
                var preferences = new Preferences(preferencesPath);

                return new SlaveExplorerViewModel(messageBoxService, preferences);
            }
        }
    }
}