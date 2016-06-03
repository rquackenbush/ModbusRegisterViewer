using System;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.FieldTypeServices
{
    public abstract class FieldTypeServiceBase : IFieldTypeService
    {
        private readonly FieldType _fieldType;

        protected FieldTypeServiceBase(FieldType fieldType)
        {
            _fieldType = fieldType;
        }

        public FieldType FieldType
        {
            get { return _fieldType; }
        }

        public abstract IRuntimeField CreateRuntimeField(FieldModel fieldModel);
        
        public abstract int GetSize(FieldModel fieldModel);

        public virtual bool SupportsOptions
        {
            get { return false; }
        }

        public virtual FieldOptionModel[] EditOptions(FieldOptionModel[] Options)
        {
            if (!SupportsOptions)
                throw new ApplicationException(string.Format("The field type {0} does not support options. EditOptions shoulds not be called.", FieldType));

            throw new ApplicationException(string.Format("No implementation was provided for EditOptions on type {0}", FieldType));
        }
    }
}
