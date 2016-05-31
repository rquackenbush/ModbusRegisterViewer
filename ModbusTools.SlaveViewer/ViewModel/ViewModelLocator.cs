using System;
using System.IO;
using ModbusTools.Common;
using ModbusTools.Common.Services;
using ModbusTools.SlaveViewer.View;

namespace ModbusTools.SlaveViewer.ViewModel
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