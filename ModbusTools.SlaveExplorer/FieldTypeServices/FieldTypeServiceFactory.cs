using System;
using System.Collections.Generic;
using System.Linq;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;

namespace ModbusTools.SlaveExplorer.FieldTypeServices
{
    internal static class FieldTypeServiceFactory
    {
        private static readonly Dictionary<FieldType, IFieldTypeService> _services;

        static FieldTypeServiceFactory()
        {
            var services = new IFieldTypeService[]
            {
                new BIT8FieldTypeService(),
                new BIT16FieldTypeService(), 
                new BIT32FieldTypeService(),  
                new FIXED16FieldTypeService(),
                new FLOAT32FieldTypeService(),
                new INT16FieldTypeService(),
                new INT32FieldTypeService(),
                new INT8FieldTypeService(),
                new UFIXED16FieldTypeService(),
                new UINT32FieldTypeService(),
                new UINT16FieldTypeService(),
                new UINT8FieldTypeService()
            };

            _services = services.ToDictionary(s => s.FieldType, s => s);
        }

        internal static IFieldTypeService GetFieldTypeService(FieldType fieldType)
        {
            return _services[fieldType];
        }
    }
}
