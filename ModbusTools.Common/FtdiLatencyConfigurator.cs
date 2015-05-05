using System;
using Microsoft.Win32;

namespace ModbusTools.Common
{
    public static class FtdiLatencyConfigurator
    {
        private const string FtdiRootKey = @"SYSTEM\CurrentControlSet\Enum\FTDIBUS";
        private const string LatencyTimerKey = "latencyTimer";

        /// <summary>
        /// Sets the latency of the FTDI cables.
        /// </summary>
        /// <param name="latencyMs"></param>
        /// <remarks>http://www.ftdichip.com/Support/Documents/AppNotes/AN_107_AdvancedDriverOptions_AN_000073.pdf</remarks>
        /// <returns></returns>
        public static bool SetLatency(int latencyMs)
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
                    if (existingValue == null || ((int)existingValue) != latencyMs)
                    {
                        Console.WriteLine("Changing FTDI latency value for {0}", deviceName);

                        //Set the value
                        deviceParamtersKey.SetValue(LatencyTimerKey, latencyMs);

                        //We have changed a value. It was glorious.
                        hasChanged = true;
                    }
                }
            }

            return hasChanged;
        }
    }
}
