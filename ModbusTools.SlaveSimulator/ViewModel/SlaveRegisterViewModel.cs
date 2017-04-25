﻿using System;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveSimulator.Model;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class SlaveRegisterViewModel : RegisterViewModel
    {
        private readonly SparsePointSource<ushort> _source;

        public SlaveRegisterViewModel(SparsePointSource<ushort> source, ushort registerIndex) 
            : base(registerIndex)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _source = source;
        }

        public override ushort Value
        {
            get { return _source[RegisterIndex]; }
            set
            {
                _source[RegisterIndex] = value;
                OnValueUpdated();
            }
        }

        public void OnValueChanged()
        {
            OnValueUpdated();
        }
    }
}
