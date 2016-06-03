using System;
using System.Threading.Tasks;
using ModbusTools.Common;

namespace ModbusTools.Capture.Model
{
    public class CaptureHost : IDisposable
    {
        public event EventHandler SampleReceived;

        private readonly IMasterContext _masterContext;
        private PromiscuousListener _listener;
        
        public CaptureHost(string path, IMasterContext masterContext)
        {
            if (masterContext == null) throw new ArgumentNullException(nameof(masterContext));

            _masterContext = masterContext;

            var task = new Task(() =>
            {
                try
                {
                    using (var writer = new CaptureFileWriter(path))
                    {
                        //Get the port
                        var port = masterContext.Master.Transport.GetStreamResource();

                        using (_listener = new PromiscuousListener(port))
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

            _masterContext.Dispose();
        }
    }
}
