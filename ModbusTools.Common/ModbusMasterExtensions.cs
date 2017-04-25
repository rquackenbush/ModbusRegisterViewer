using System;
using System.Collections.Generic;
using System.Linq;
using NModbus;

namespace ModbusTools.Common
{
    public static class ModbusMasterExtensions
    {
        private static ushort[] BlockRead(ushort startingRegister,
            ushort numberOfRegisters,
            ushort blockSize,
            Func<ushort, ushort, ushort[]> reader)
        {
            ushort soFar = 0;

            //Create a list with enough initial room for all of the registers
            var registers = new List<ushort>(numberOfRegisters);

            while (soFar < numberOfRegisters)
            {
                ushort thisRead = blockSize;

                if (thisRead > numberOfRegisters - soFar)
                {
                    thisRead = (ushort)(numberOfRegisters - soFar);
                }

                var result = reader((ushort)(startingRegister + soFar), thisRead);

                registers.AddRange(result);

                soFar += thisRead;
            }

            return registers.ToArray();
        }

        public static ushort[] ReadInputRegisters(this IModbusMaster master,
            byte slaveAddress,
            ushort startingRegister,
            ushort numberOfRegisters,
            ushort blockSize)
        {
            return BlockRead(startingRegister,
                numberOfRegisters,
                blockSize,
                (start, num) => master.ReadInputRegisters(slaveAddress, start, num));
        }

        public static ushort[] ReadHoldingRegisters(this IModbusMaster master,
            byte slaveAddress,
            ushort startingRegister,
            ushort numberOfRegisters,
            ushort blockSize)
        {
            return BlockRead(startingRegister,
                numberOfRegisters,
                blockSize,
                (start, num) => master.ReadHoldingRegisters(slaveAddress, start, num));
        }

        public static void WriteMultipleRegisters(this IModbusMaster master,
            byte slaveAddress,
            ushort startingRegister,
            ushort[] values,
            ushort blockSize)
        {
            ushort soFar = 0;

            while (soFar < values.Length)
            {
                ushort thisWrite = blockSize;

                if (thisWrite > values.Length - soFar)
                {
                    thisWrite = (ushort)(values.Length - soFar);
                }

                var valuesToWrite = values.Skip(soFar).Take(thisWrite).ToArray();

                master.WriteMultipleRegisters(slaveAddress, (ushort)(startingRegister + soFar), valuesToWrite);

                soFar += thisWrite;
            }
        }
    }
}
