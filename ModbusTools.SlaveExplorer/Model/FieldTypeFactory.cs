using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModbusTools.SlaveExplorer.Model
{
    public static class FieldTypeFactory
    {
        public static IEnumerable<FieldType> GetFieldTypes()
        {
            return Enum.GetValues(typeof (FieldType))
                .Cast<FieldType>()
                .OrderBy(e => e.ToString())
                .ToArray();
        }
    }
}