using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.Model;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.ViewModel
{
    public class PacketViewModel
    {
        private readonly Sample[] _samples;
        private readonly FunctionServiceResult _serviceResult;

        public PacketViewModel(IEnumerable<Sample> samples)
        {
            if (samples == null) throw new ArgumentNullException(nameof(samples));

            _samples = samples.ToArray();

            if (_samples.Length > 0)
            {
                SlaveId = _samples[0].Value;
            }

            if (_samples.Length > 1)
            {
                FunctionCode = _samples[1].Value;

                FunctionCodeDescription = FunctionCodeDescriptionFactory.GetFunctionCodeDescription(_samples[1].Value);
            }

            //Get the raw message in bytes
            var message = _samples.Select(s => s.Value).ToArray();

            if (_samples.Length < 5)
            {
                Error = $"Only {_samples.Length} samples - not enough samples to make a valid Modbus packet.";
            }
            else
            {
                if (!message.DoesCrcMatch())
                {
                    Error = "CRC Mismatch";
                }
            }

            if (!HasError)
            {
                _serviceResult = FunctionServiceManager.Process(_samples);

                Summary = _serviceResult.Summary;
            }
           
        }

        public byte? SlaveId { get; private set; }

        public byte? FunctionCode { get; private set; }
     
        public string FunctionCodeDescription { get; private set; }

        public string Summary { get; private set; }

        public bool HasError 
        {
            get { return !string.IsNullOrWhiteSpace(Error); }
        }

        public string Error { get; private set; }

        public int Length
        {
            get { return _samples.Length; }
        }

        public Sample[] Samples
        {
            get { return _samples; }
        }

        public Visual Visual
        {
            get { return _serviceResult?.Visual; }
        }
    }
}