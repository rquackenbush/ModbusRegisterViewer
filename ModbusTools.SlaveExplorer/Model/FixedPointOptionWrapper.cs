using System.Collections.Generic;

namespace ModbusTools.SlaveExplorer.Model
{
    internal class FixedPointOptionWrapper : FieldOptionWrapper
    {
        private const string ScaleKey = "SCALE";

        internal FixedPointOptionWrapper(IEnumerable<FieldOptionModel> options) 
            : base(options)
        {
        }

        public double Scale
        {
            get { return GetDouble(ScaleKey, 1); }
            set { SetDouble(ScaleKey, value); }
        }
    }
}
