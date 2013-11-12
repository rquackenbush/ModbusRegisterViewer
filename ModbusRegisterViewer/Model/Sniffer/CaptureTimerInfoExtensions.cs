using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public static class CaptureTimerInfoExtensions
    {
        /// <summary>
        /// Gets the offset time from ticks
        /// </summary>
        /// <param name="captureTimerInfo"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static DateTime GetOffsetTime(this CaptureTimerInfo captureTimerInfo, long ticks)
        {
            //Get the seconds
            var seconds = captureTimerInfo.TicksToSeconds(ticks);

            //Connvert to seconds
            return captureTimerInfo.StartTime.AddSeconds(seconds);
        }

        /// <summary>
        /// Converts ticks to real world seconds.
        /// </summary>
        /// <param name="captureTimerInfo"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static double TicksToSeconds(this CaptureTimerInfo captureTimerInfo, long ticks)
        {
            return (double)ticks / (double)captureTimerInfo.TicksPerSecond;
        }

        /// <summary>
        /// TickstoMiliseconds
        /// </summary>
        /// <param name="captureTimerInfo"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static double TicksToMilliseconds(this CaptureTimerInfo captureTimerInfo, long ticks)
        {
            //return ((double)ticks * 1000.0) / ((double)captureTimerInfo.TicksPerSecond);

            return captureTimerInfo.TicksToSeconds(ticks) * 1000.0;
        }
    }
}
