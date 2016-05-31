using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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
                var latencyPath = $"{FtdiRootKey}\\{deviceName}\\0000\\Device Parameters";

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

        /// <summary>
        /// Sets the latency of the FTDI cables.
        /// </summary>
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
            }

            //Create a temporary path
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString("N")}.reg");

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
