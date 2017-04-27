using System;
using System.Threading.Tasks;
using ModbusTools.Common;
using NModbus.IO;

namespace ModbusTools.Capture.Model
{
    public class CaptureHost : IDisposable
    {
        public event EventHandler SampleReceived;

        private readonly IStreamResource _streamResource;
        private PromiscuousListener _listener;
        
        public CaptureHost(string path, IStreamResource streamResource)
        {
            if (streamResource == null) throw new ArgumentNullException(nameof(streamResource));

            _streamResource = streamResource;

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
            var handler = SampleReceived;

            if (handler == null)
                return;

            handler(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            var listener = _listener;

            if (listener != null)
            {
                listener.Dispose();
            }

            //streamResource.Dispose();
        }
    }
}
