using System;
using System.Windows;
using ModbusTools.Common;

namespace ModbusTools.Configuration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                FtdiLatencyConfigurator.SetLatency();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error configuring FTDI adapters.");
            }

            Shutdown();
        }
    }
}
