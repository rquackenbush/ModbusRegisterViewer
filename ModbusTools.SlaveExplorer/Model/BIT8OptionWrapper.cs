using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace ModbusTools.SlaveExplorer.Model
{
    internal class BIT8OptionWrapper : FieldOptionWrapper
    {
        internal BIT8OptionWrapper(IEnumerable<FieldOptionModel> options) 
            : base(options)
        {
        }

        private string GetNameKey(int bitIndex)
        {
            return string.Format("BIT_{0}_NAME", bitIndex);
        }

        private string GetName(int bitIndex)
        {
            var key = GetNameKey(bitIndex);
            var defaultValue = string.Format("Bit {0}", bitIndex);

            return GetString(key, defaultValue);
        }

        private void SetName(int bitIndex, string value)
        {
            var key = GetNameKey(bitIndex);

            SetString(key, value);
        }

        public string Bit0Name
        {
            get { return GetName(0); }
            set { SetName(0, value); }
        }

        public string Bit1Name
        {
            get { return GetName(1); }
            set { SetName(1, value); }
        }

        public string Bit2Name
        {
            get { return GetName(2); }
            set { SetName(2, value); }
        }

        public string Bit3Name
        {
            get { return GetName(3); }
            set { SetName(3, value); }
        }

        public string Bit4Name
        {
            get { return GetName(4); }
            set { SetName(4, value); }
        }

        public string Bit5Name
        {
            get { return GetName(5); }
            set { SetName(5, value); }
        }

        public string Bit6Name
        {
            get { return GetName(6); }
            set { SetName(6, value); }
        }

        public string Bit7Name
        {
            get { return GetName(7); }
            set { SetName(7, value); }
        }
    }
}
