using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.IO;
using Modbus.Device;
using Unme.Common;

namespace ModbusRegisterViewer.Model
{
    public class SampleEventArgs : EventArgs
    {
        public SampleEventArgs(byte sample)
        {
            this.Sample = sample;
        }

        public byte Sample { get; private set; }
    }

    public class PromiscuousListener : IDisposable
    {
        public event EventHandler<SampleEventArgs> Sample;

        private IStreamResource _streamResource;

        public PromiscuousListener(IStreamResource streamResource)
        {
            if (streamResource == null)
                throw new ArgumentNullException("streamResource");

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
                        this.Sample.Raise(this, new SampleEventArgs(sample));
                    }

                    catch (IOException ioe)
                    {
                        _streamResource.DiscardInBuffer();
                    }
                    catch (TimeoutException te)
                    {
                        _streamResource.DiscardInBuffer();
                    }
                }
                catch (InvalidOperationException)
                {
                    // when the underlying transport is disposed
                    break;
                }
            }
        }
        
        public void Dispose()
        {
            DisposableUtility.Dispose(ref _streamResource);
        }
    }
}
