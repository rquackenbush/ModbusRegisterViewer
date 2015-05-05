using System;
using System.Reflection;
using Modbus.IO;

namespace ModbusTools.Common
{
    public static class ModbusTransportExtensions
    {
        public static IStreamResource GetStreamResource(this ModbusTransport modbusTransport)
        {
            var property = modbusTransport.GetType().GetProperty("StreamResource", BindingFlags.Instance | BindingFlags.NonPublic);

            if (property == null)
                throw new InvalidOperationException("Unable to find StreamResource");

            return property.GetValue(modbusTransport) as IStreamResource;
        }
    }
}
