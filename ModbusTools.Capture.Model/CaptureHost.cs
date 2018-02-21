using System;
using System.Threading.Tasks;
using ModbusTools.Common;
using NModbus.IO;

namespace ModbusTools.Capture.Model
{
    public class CaptureHost : IDisposable
    {
        public event EventHandler SampleReceived;

        private PromiscuousListener _listener;
        
        public CaptureHost(string path, IStreamResource streamResource)
        {
            if (streamResource == null) throw new ArgumentNullException(nameof(streamResource));

            var task = new Task(() =>
            {
                try
                {
                    using (var writer = new CaptureFileWriter(path))
                    {
                        using (_listener = new PromiscuousListener(streamResource))
                        {
                            _listener.Sample += (sender, args) =>
                            {
                                writer.WriteSample(args.Sample);

                                OnSampleReceived();
                            };

                            _listener.Listen();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });     

            //Start up this awesome task
            task.Start();
        }

        protected void OnSampleReceived()
        {
            SampleReceived?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _listener?.Dispose();
        }
    }
}
