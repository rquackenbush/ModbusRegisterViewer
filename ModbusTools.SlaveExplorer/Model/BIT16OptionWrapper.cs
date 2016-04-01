//using System.Collections.Generic;

//namespace ModbusTools.SlaveExplorer.Model
//{
//    public class BIT16OptionWrapper : FieldOptionWrapper
//    {
//        internal BIT16OptionWrapper(IEnumerable<FieldOptionModel> options) 
//            : base(options)
//        {
//        }

//        private string GetNameKey(int bitIndex)
//        {
//            return $"BIT_{bitIndex}_NAME";
//        }

//        private string GetName(int bitIndex)
//        {
//            var key = GetNameKey(bitIndex);
//            var defaultValue = $"Bit {bitIndex}";

//            return GetString(key, defaultValue);
//        }

//        private void SetName(int bitIndex, string value)
//        {
//            var key = GetNameKey(bitIndex);

//            SetString(key, value);
//        }

//        public string Bit0Name
//        {
//            get { return GetName(0); }
//            set { SetName(0, value); }
//        }

//        public string Bit1Name
//        {
//            get { return GetName(1); }
//            set { SetName(1, value); }
//        }

//        public string Bit2Name
//        {
//            get { return GetName(2); }
//            set { SetName(2, value); }
//        }

//        public string Bit3Name
//        {
//            get { return GetName(3); }
//            set { SetName(3, value); }
//        }

//        public string Bit4Name
//        {
//            get { return GetName(4); }
//            set { SetName(4, value); }
//        }

//        public string Bit5Name
//        {
//            get { return GetName(5); }
//            set { SetName(5, value); }
//        }

//        public string Bit6Name
//        {
//            get { return GetName(6); }
//            set { SetName(6, value); }
//        }

//        public string Bit7Name
//        {
//            get { return GetName(7); }
//            set { SetName(7, value); }
//        }

//        public string Bit8Name
//        {
//            get { return GetName(8); }
//            set { SetName(8, value); }
//        }

//        public string Bit9Name
//        {
//            get { return GetName(9); }
//            set { SetName(9, value); }
//        }

//        public string Bit10Name
//        {
//            get { return GetName(10); }
//            set { SetName(10, value); }
//        }

//        public string Bit11Name
//        {
//            get { return GetName(11); }
//            set { SetName(11, value); }
//        }

//        public string Bit12Name
//        {
//            get { return GetName(12); }
//            set { SetName(12, value); }
//        }

//        public string Bit13Name
//        {
//            get { return GetName(13); }
//            set { SetName(13, value); }
//        }

//        public string Bit14Name
//        {
//            get { return GetName(14); }
//            set { SetName(14, value); }
//        }

//        public string Bit15Name
//        {
//            get { return GetName(15); }
//            set { SetName(15, value); }
//        }

//    }
//}