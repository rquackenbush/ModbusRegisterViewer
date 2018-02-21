using System;
using System.IO;
using NModbus.IO;

namespace ModbusTools.Common
{
    public class SampleEventArgs : EventArgs
    {
        public SampleEventArgs(byte sample)
        {
            this.Sample = sample;
        }

        public byte Sample { get; }
    }

    public class PromiscuousListener : IDisposable
    {
        public event EventHandler<SampleEventArgs> Sample;

        private readonly IStreamResource _streamResource;

        public PromiscuousListener(IStreamResource streamResource)
        {
            if (streamResource == null)
                throw new ArgumentNullException(nameof(streamResource));

            _streamResource = streamResource;
        }

        public void Listen()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        //Get a single byte
                        byte sample = _streamResource.ReadSingleByte();

                        //Raise the event
                        RaiseSampleEvent(sample);
                    }

                    catch (IOException)
                    {
                        if (_streamResource != null)
                        {
                            //_streamResource.DiscardInBuffer();
                        }
                    }
                    catch (TimeoutException)
                    {
                        _streamResource?.DiscardInBuffer();
                    }
                }
                catch (InvalidOperationException)
                {
                    // when the underlying transport is disposed
                    break;
                }
            }
        }

        private void RaiseSampleEvent(byte data)
        {
            Sample?.Invoke(this, new SampleEventArgs(data));
        }
        
        public void Dispose()
        {
            _streamResource.Dispose();            
        }
    }
}
