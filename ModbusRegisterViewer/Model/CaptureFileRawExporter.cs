using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    internal static class CaptureFileRawExporter
    {
        public static void Export(string captureFilePath, string exportPath)
        {
            using (var writer = File.CreateText(exportPath))
            using (var reader = new CaptureFileReader(captureFilePath))
            {
                var captureTimerInfo = new CaptureTimerInfo(reader.StartTime, reader.TicksPerSecond);

                writer.WriteLine("Value, Interval(ms)");

                Sample sample = reader.Read();

                Sample previousSample = null;

                var startingTicks = sample.Ticks;

                while (sample != null)
                {
                    long intervalInTicks = 0;

                    if (previousSample != null)
                    {
                        intervalInTicks = sample.Ticks - previousSample.Ticks;
                    }

                    var interveralInMs = captureTimerInfo.TicksToMilliseconds(intervalInTicks);

                    writer.WriteLine("\"{0:X2}\",{1:0.000}", sample.Value, interveralInMs);

                    previousSample = sample;

                    sample = reader.Read();
                }
            }
        }
    }
}
