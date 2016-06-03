using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusTools.StructuredSlaveExplorer.Model
{
    public class BITOptionWrapper : FieldOptionWrapper
    {
        private readonly int _numberOfBits;

        public BITOptionWrapper(IEnumerable<FieldOptionModel> options, int numberOfBits) 
            : base(options)
        {
            _numberOfBits = numberOfBits;
        }

        public int NumberOfBits
        {
            get { return _numberOfBits; }
        }

        protected string GetNameKey(int bitIndex)
        {
            return $"BIT_{bitIndex}_NAME";
        }

        protected string GetName(int bitIndex)
        {
            var key = GetNameKey(bitIndex);
            var defaultValue = $"Bit {bitIndex}";

            return GetString(key, defaultValue);
        }

        protected void SetName(int bitIndex, string value)
        {
            var key = GetNameKey(bitIndex);

            SetString(key, value);
        }

        private void ValidateBitIndex(int bitIndex)
        {
            if (bitIndex >= _numberOfBits)
                throw new ArgumentOutOfRangeException($"bitIndex must be between 0 and {_numberOfBits - 1}");
        }

        /// <summary>
        /// Gets or sets the name of the specified bit field.
        /// </summary>
        /// <param name="bitIndex"></param>
        /// <returns></returns>
        public string this[int bitIndex]
        {
            get
            {
                ValidateBitIndex(bitIndex);
                return GetName(bitIndex);                 
            }
            set
            {
                ValidateBitIndex(bitIndex);
                SetName(bitIndex, value); 
            }
        }

        public string[] GetNames()
        {
            var range = Enumerable.Range(0, _numberOfBits);

            return range.Select(bitIndex => this[bitIndex]).ToArray();
        }
    }
}