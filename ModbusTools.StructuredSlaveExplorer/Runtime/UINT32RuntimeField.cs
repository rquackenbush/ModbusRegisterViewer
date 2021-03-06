﻿using System;
using System.Windows;
using MiscUtil.Conversion;
using ModbusTools.Common;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public class UINT32RuntimeField : RuntimeFieldBase
    {
        private readonly RuntimeFieldEditor<LongUpDown> _editor;
        
        public UINT32RuntimeField(FieldModel fieldModel) 
            : base(fieldModel)
        {
            _editor = new RuntimeFieldEditor<LongUpDown>(
                fieldModel.Name,
                new LongUpDown()
                {
                    Minimum = UInt32.MinValue,
                    Maximum = UInt32.MaxValue,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    BorderThickness = new Thickness(0),
                    ClipValueToMinMax = true
                });
        }

        public override int Size
        {
            get { return 4; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToUInt32(data, 0);

            _editor.Visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (UInt32)(_editor.Visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }

        public override IRuntimeFieldEditor[] FieldEditors
        {
            get { return _editor.ToSingletonArray<IRuntimeFieldEditor>(); }
        }
    }
}
