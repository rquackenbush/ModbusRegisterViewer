using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTools.SlaveSimulator.Model
{
    public static class IRegisterStorageExtensions
    {
        public static void WriteSingle(this IRegisterStorage registerStorage, ushort address, ushort value)
        {
            registerStorage.Write(address,
                new []
                {
                    value
                });
        }

        public static ushort ReadSingle(this IRegisterStorage registerStorage, ushort address)
        {
            return registerStorage.Read(address, 1)[0];
        }
    }
}
