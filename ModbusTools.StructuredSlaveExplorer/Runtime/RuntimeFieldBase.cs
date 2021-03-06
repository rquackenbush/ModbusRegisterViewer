﻿using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public abstract class RuntimeFieldBase : IRuntimeField
    {
        private readonly FieldModel _fieldModel;

        protected RuntimeFieldBase(FieldModel fieldModel)
        {
            _fieldModel = fieldModel;
        }

        protected FieldModel FieldModel
        {
            get { return _fieldModel; }
        }

        public int Offset
        {
            get { return _fieldModel.Offset; }   
        }

        public abstract int Size { get; }

        public abstract void SetBytes(byte[] data);

        public abstract byte[] GetBytes();

        public abstract IRuntimeFieldEditor[] FieldEditors { get; }
        
    }
}
