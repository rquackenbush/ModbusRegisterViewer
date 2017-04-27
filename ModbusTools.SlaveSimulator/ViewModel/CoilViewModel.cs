using System;
using GalaSoft.MvvmLight;
using ModbusTools.SlaveSimulator.Model;
using NModbus;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class CoilViewModel : ViewModelBase
    {
        private readonly ushort _address;
        private readonly SparsePointSource<bool> _source;

        public CoilViewModel(SparsePointSource<bool> source, ushort address)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            _address = address;
            _source = source;
        }

        public ushort Address
        {
            get { return _address; }
        }

        public bool Value
        {
            get { return _source[Address]; }
            set { _source[Address] = value; }
        }

        public void OnValueChanged()
        {
            RaisePropertyChanged(() => Value);
        }
    }
}