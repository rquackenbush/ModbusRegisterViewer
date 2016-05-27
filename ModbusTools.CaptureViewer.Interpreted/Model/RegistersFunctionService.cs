using System;
using System.Collections.Generic;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.View;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public abstract class RegistersFunctionService : FunctionServiceBase
    {
        protected const int NumberOfRegistersMessageLength = 8;

        protected RegistersFunctionService(FunctionCode functionCode) 
            : base(functionCode)
        {
        }

        protected FunctionServiceResult ProcessNumberOfRegisters(Sample[] samples)
        {
            if (samples.Length != NumberOfRegistersMessageLength)
                throw new ArgumentException($"The message must be 8 bytes long. It was {samples.Length}.");

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

            var summary = $"Read {number} registers starting at {address}";

            return new FunctionServiceResult(summary);
        }

        protected FunctionServiceResult ProcessRegisters(Sample[] samples)
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

            Func<Visual> visualFactory = () =>
            {
                var viewModel = new RegistersViewModel(registers.ToArray());

                return new RegistersView()
                {
                    DataContext = viewModel
                };
            };

            var summary = $"{numberOfRegisters} registers.";

            return new FunctionServiceResult(summary, visualFactory);
        }
    }
}