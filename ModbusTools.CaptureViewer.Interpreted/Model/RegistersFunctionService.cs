using System;
using System.Collections.Generic;
using System.Linq;
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

        protected FunctionServiceResult ProcessNumberOfRegisters(Sample[] samples, PacketType? packetType)
        {
            if (samples.Length != NumberOfRegistersMessageLength)
                throw new ArgumentException($"The message must be 8 bytes long. It was {samples.Length}.");

            //Get the raw bytes
            var messageBytes = samples
                .Select(s => s.Value)
                .ToArray();

            ushort address = EndianBitConverter.Big.ToUInt16(messageBytes, 2);
            ushort number = EndianBitConverter.Big.ToUInt16(messageBytes, 4);

            var summary = $"Read {number} registers starting at {address}";

            return new FunctionServiceResult(summary, packetType: packetType);
        }

        protected FunctionServiceResult ProcessReadRegisters(Sample[] samples, PacketType? packetType)
        {
            //Get the raw bytes
            var messageBytes = samples
                .Select(s => s.Value)
                .ToArray();

            //We'll assume that this is a response
            byte numberOfBytes = messageBytes[2];

            //Get the number of registers
            var numberOfRegisters = numberOfBytes / 2;

            //Get the registers themselves
            var registers = GetRegisters(messageBytes, numberOfRegisters, 3);

            //Create the visual factory
            Func<Visual> visualFactory = () =>
            {
                var viewModel = new RegistersViewModel(registers);

                return new RegistersView()
                {
                    DataContext = viewModel
                };
            };

            var summary = $"{numberOfRegisters} registers.";

            return new FunctionServiceResult(summary, visualFactory, packetType);
        }

        protected ushort[] GetRegisters(byte[] messageBytes, int numberOfRegisters, int startOffset)
        {
            int currentOffset = startOffset;

            var registers = new ushort[numberOfRegisters];

            for (int registerIndex = 0; registerIndex < numberOfRegisters; registerIndex++)
            {
                //Convert each register
                registers[registerIndex] = EndianBitConverter.Big.ToUInt16(messageBytes, currentOffset);

                //Move to the next offset
                currentOffset += 2;
            }

            return registers;
        }
    }
}