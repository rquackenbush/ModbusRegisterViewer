using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Diagnostics;

namespace ModbusTools.Common
{
    public static class FtdiConfiguration
    {
        private const string FtdiRootKey = @"SYSTEM\CurrentControlSet\Enum\FTDIBUS";
        private const string LatencyTimerKey = "latencyTimer";

        public const int LatencyMs = 1;

        private static IEnumerable<string> GetRegistryPathsForLatencyThatNeedChanging()
        {
            var ftdiRootKey = Registry.LocalMachine.OpenSubKey(FtdiRootKey);

            if (ftdiRootKey == null)
                yield break;

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
                        yield return latencyPath;
                    }
                }
            }
        }

        public static bool RequiresLatencyChanges()
        {
            return GetRegistryPathsForLatencyThatNeedChanging().Any();
        }

        //private static void SetLatencyCommandLine(string registryKey)
        //{
        //    var args = string.Format("add \"HKLM\\{0}\" /f /v \"{1}\" /t REG_DWORD /d {2}", registryKey, LatencyTimerKey, LatencyMs);

        //    var process = Process.Start("reg", args);

        //    if (process == null)
        //    {
        //        MessageBox.Show("Unable to start command line process");
                                
        //    }
        //    else
        //    {

        //        process.WaitForExit();
        //    }
        //}

        /// <summary>
        /// Sets the latency of the FTDI cables.
        /// </summary>
        /// <param name="latencyMs"></param>
        /// <remarks>http://www.ftdichip.com/Support/Documents/AppNotes/AN_107_AdvancedDriverOptions_AN_000073.pdf</remarks>
        /// <returns></returns>
        public static void SetLatency()
        {
            var keys = GetRegistryPathsForLatencyThatNeedChanging();

            var regFile = new StringBuilder();

            regFile.AppendLine("Windows Registry Editor Version 5.00");
            regFile.AppendLine();

            foreach (var key in keys)
            {
                regFile.AppendFormat("[HKEY_LOCAL_MACHINE\\{0}]", key);
                regFile.AppendLine();

                regFile.AppendFormat("\"LatencyTimer\"=dword:0000000{0}", LatencyMs);
                regFile.AppendLine();
                regFile.AppendLine();

                //SetLatencyCommandLine(key);

                ////Try to load up the sub key
                //var deviceParamtersKey = Registry.LocalMachine.OpenSubKey(key, true);

                ////Make sure that we found something
                //if (deviceParamtersKey != null)
                //{
                //    //Set the value
                //    deviceParamtersKey.SetValue(LatencyTimerKey, LatencyMs);

                //    //We have changed a value. It was glorious.
                //    hasChanged = true;
                //}
            }

            //Create a temporary path
            var tempPath = Path.Combine(Path.GetTempPath(), string.Format("{0}.reg", Guid.NewGuid().ToString("N")));

            //Write out the file
            File.WriteAllText(tempPath, regFile.ToString());

            var process = Process.Start(tempPath);

            if (process == null)
            {
                MessageBox.Show("Unable to start latency process");
            }
            else
            {
                process.WaitForExit();
            }

            //Delete the file
            File.Delete(tempPath);
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

                            //LaunchConfigurationTool();
                            SetLatency();

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
