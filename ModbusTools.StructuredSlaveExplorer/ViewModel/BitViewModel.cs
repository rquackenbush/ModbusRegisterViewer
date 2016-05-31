using System;
using GalaSoft.MvvmLight;
using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.ViewModel
{
    public class BitViewModel : ViewModelBase
    {
        private readonly BITOptionWrapper _options;
        private readonly int _bitIndex;

        public BitViewModel(BITOptionWrapper options, int bitIndex)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _options = options;
            _bitIndex = bitIndex;
        }

        public string Label
        {
            get { return $"BIT {_bitIndex}"; }
        }

        public string Name
        {
            get { return _options[_bitIndex]; }
            set { _options[_bitIndex] = value; }
        }
    }
}