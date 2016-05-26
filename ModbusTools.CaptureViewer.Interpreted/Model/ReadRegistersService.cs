using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.View;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public abstract class ReadRegistersService : FunctionServiceBase
    {
        protected ReadRegistersService(FunctionCode functionCode) 
            : base(functionCode)
        {
        }

        public override FunctionServiceResult Process(Sample[] samples)
        {
            string summary;

            Func<Visual> visualFactory = null;

            if (samples.Length == 8)
            {
                //This is a request
                byte[] addressBytes = new[]
                {
                    samples[2].Value,
                    samples[3].Value
                };

                byte[] numberBytes = new[]
                {
                    samples[4].Value,
                    samples[5].Value
                };

                ushort address = EndianBitConverter.Big.ToUInt16(addressBytes, 0);
                ushort number = EndianBitConverter.Big.ToUInt16(numberBytes, 0);

                summary = $"Read {number} registers starting at {address}";
            }
            else
            {
                //We'll assume that this is a response
                byte numberOfBytes = samples[2].Value;

                var numberOfRegisters = numberOfBytes / 2;

                var registers = new List<ushort>(numberOfRegisters);

                int sampleIndex = 4;

                for (int registerIndex = 0; registerIndex < numberOfRegisters; registerIndex++)
                {
                    var registerBytes = new byte[]
                    {
                        samples[sampleIndex++].Value,
                        samples[sampleIndex++].Value,
                    };

                    var register = EndianBitConverter.Big.ToUInt16(registerBytes, 0);

                    registers.Add(register);
                }

                visualFactory = () =>
                {
                    var viewModel = new RegistersViewModel(registers.ToArray());

                    return new RegistersView()
                    {
                        DataContext = viewModel
                    };
                };

                summary = $"{numberOfRegisters} returned from slave.";
            }

            return new FunctionServiceResult(summary, visualFactory);
        }
    }
}