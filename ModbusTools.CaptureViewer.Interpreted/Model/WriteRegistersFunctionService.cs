using System;
using System.Linq;
using System.Windows.Media;
using MiscUtil.Conversion;
using ModbusTools.Capture.Model;
using ModbusTools.CaptureViewer.Interpreted.View;
using ModbusTools.CaptureViewer.Interpreted.ViewModel;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class WriteRegistersFunctionService : RegistersFunctionService
    {
        public WriteRegistersFunctionService() 
            : base(FunctionCode.WriteMultipleRegisters)
        {
        }

        private FunctionServiceResult ProcessWriteRegisters(Sample[] samples)
        {
            // http://www.simplymodbus.ca/FC16.htm

            //Get the raw bytes
            var messageBytes = samples
                .Select(s => s.Value)
                .ToArray();

            var dataAddress = EndianBitConverter.Big.ToUInt16(messageBytes, 2);
            var numberOfRegisters = EndianBitConverter.Big.ToUInt16(messageBytes, 4);

            var bytesToFollow = messageBytes[6];

            if (bytesToFollow != numberOfRegisters*2)
            {
                return
                    new FunctionServiceResult(
                        $"The bytes to follow {bytesToFollow} does not match the number of registers {numberOfRegisters}.");
            }

            var registers = GetRegisters(messageBytes, numberOfRegisters, 7);

            Func<Visual> visualFactory = () =>
            {
                var viewModel = new RegistersViewModel(registers.ToArray(), dataAddress);

                return new RegistersView()
                {
                    DataContext = viewModel
                };
            };

            var summary = $"Write {numberOfRegisters} registers starting at register {dataAddress}";

            return new FunctionServiceResult(summary, visualFactory, PacketType.Request);
        }

        public override FunctionServiceResult Process(Sample[] samples)
        {
            if (samples.Length == NumberOfRegistersMessageLength)
            {
                return ProcessNumberOfRegisters(samples, PacketType.Response);
            }

            return ProcessWriteRegisters(samples);
        }
    }
}