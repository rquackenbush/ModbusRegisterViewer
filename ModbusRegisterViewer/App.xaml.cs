using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using ModbusRegisterViewer.Views;
using ModbusTools.Common;
using ModbusTools.Launcher;
using ApplicationView = ModbusTools.Launcher.ApplicationView;

//using ModbusTools.Launcher;

namespace ModbusRegisterViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const int FtdiLatencyMs = 1;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize();

            var applicationWindow = new ApplicationView();

            applicationWindow.Show();

            //try
            //{
            //    //Attempt to set the latency of the FTDI ports
            //    if (FtdiLatencyConfigurator.SetLatency(FtdiLatencyMs))
            //    {
            //        //We changed at least one, go ahead and let the user know.
            //        var message = string.Format("The latency of the FTDI ports on this machine were automatically set to {0}ms.\n\nThis is done in order to enable adaquate timing resolution for the Modbus Capture utility and the Slave Simulator.", FtdiLatencyMs);

            //        MessageBox.Show(message, "FTDI Latency Set", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString(), "Error setting FTDI latency");
            //}
        }
    }
}
