using System;
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
                case FieldType.UINT16:

                    return new UINT16RuntimeField(fieldModel.Name, fieldModel.Offset);

                case FieldType.UINT32:

                    return new UINT32RuntimeField(fieldModel.Name, fieldModel.Offset);

                default:
                    throw new ApplicationException(string.Format("Unsupported field type {0}", fieldModel.FieldType));
            }
        }
    }
}
