using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;

namespace ModbusTools.Common
{
    public static class FtdiLatencyConfigurator
    {
        private const string FtdiRootKey = @"SYSTEM\CurrentControlSet\Enum\FTDIBUS";
        private const string LatencyTimerKey = "latencyTimer";

        public const int LatencyMs = 1;

        public static bool RequiresLatencyChanges()
        {
            var ftdiRootKey = Registry.LocalMachine.OpenSubKey(FtdiRootKey);

            if (ftdiRootKey == null)
                return false;

            //Get the key for each device
            var deviceNames = ftdiRootKey.GetSubKeyNames();

            //Iterate through each FTDI configuration
            foreach (var deviceName in deviceNames)
            {
                //Get the path to the latency parameter
                var latencyPath = string.Format("{0}\\{1}\\0000\\Device Parameters", FtdiRootKey, deviceName);

                //Try to load up the sub key
                var deviceParamtersKey = Registry.LocalMachine.OpenSubKey(latencyPath, false);

                //Make sure that we found something
                if (deviceParamtersKey != null)
                {
                    //Get the existing values
                    var existingValue = deviceParamtersKey.GetValue(LatencyTimerKey);

                    //Check to see if we need to change the value
                    if (existingValue == null || ((int)existingValue) != LatencyMs)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the latency of the FTDI cables.
        /// </summary>
        /// <param name="latencyMs"></param>
        /// <remarks>http://www.ftdichip.com/Support/Documents/AppNotes/AN_107_AdvancedDriverOptions_AN_000073.pdf</remarks>
        /// <returns></returns>
        public static bool SetLatency()
        {
            var ftdiRootKey = Registry.LocalMachine.OpenSubKey(FtdiRootKey);

            if (ftdiRootKey == null)
                return false;

            //Get the key for each device
            var deviceNames = ftdiRootKey.GetSubKeyNames();

            bool hasChanged = false;

            //Iterate through each FTDI configuration
            foreach (var deviceName in deviceNames)
            {
                //Get the path to the latency parameter
                var latencyPath = string.Format("{0}\\{1}\\0000\\Device Parameters", FtdiRootKey, deviceName);

                //Try to load up the sub key
                var deviceParamtersKey = Registry.LocalMachine.OpenSubKey(latencyPath, true);

                //Make sure that we found something
                if (deviceParamtersKey != null)
                {
                    //Get the existing values
                    var existingValue = deviceParamtersKey.GetValue(LatencyTimerKey);

                    //Check to see if we need to change the value
                    if (existingValue == null || ((int)existingValue) != LatencyMs)
                    {
                        Console.WriteLine("Changing FTDI latency value for {0}", deviceName);

                        //Set the value
                        deviceParamtersKey.SetValue(LatencyTimerKey, LatencyMs);

                        //We have changed a value. It was glorious.
                        hasChanged = true;
                    }
                }
            }

            return hasChanged;
        }

        public static void LaunchConfigurationTool()
        {
            const string executable = "ModbusTools.Configuration.exe";

            var process = Process.Start(executable);

            if (process == null)
            {
                MessageBox.Show(string.Format("Unable to find or launch '{0}'", executable));
            }
            else
            {
                process.WaitForExit();    
            }
        }

        public static bool CheckForLatency(Window window)
        {
            try
            {
                if (RequiresLatencyChanges())
                {
                    const string message = "One or more FTDI ports are configured incorrectly on this computer.\n\nAttempt to auto correct?";

                    var result = MessageBox.Show(window, message, "Incompatible Port Settings", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:

                            LaunchConfigurationTool();

                            //SetLatency();
                            break;

                        case MessageBoxResult.No:
                            break;

                        case MessageBoxResult.Cancel:
                            return false;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(window, ex.Message, "An error occurred while attempting to configure the FTDI drivers",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return true;
        }
    }
}
