using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public static class RuntimeFieldFactory
    {
        public static IRuntimeField Create(FieldModel fieldModel)
        {
            switch (fieldModel.FieldType)
            {
                case FieldType.UINT32:
                    return new UINT32RuntimeField(fieldModel);

                case FieldType.INT32:
                    return new INT32RuntimeField(fieldModel);

                case FieldType.FLOAT32:
                    return new FLOAT32RuntimeField(fieldModel);

                case FieldType.UINT16:
                    return new UINT16RuntimeField(fieldModel);

                case FieldType.INT16:
                    return new INT16RuntimeField(fieldModel);

                case FieldType.UINT8:
                    return new UINT8RuntimeField(fieldModel);

                case FieldType.INT8:
                    return new INT8RuntimeField(fieldModel);

                default:
                    throw new ApplicationException(string.Format("Unsupported field type {0}", fieldModel.FieldType));
            }
        }
    }
}
