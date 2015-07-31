using System;
using System.Windows.Media;
using MiscUtil.Conversion;
using Xceed.Wpf.Toolkit;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public class UINT32RuntimeField : RuntimeFieldBase
    {
        private readonly LongUpDown _visual;

        public UINT32RuntimeField(string name, int offset) 
            : base(name, offset)
        {
            _visual = new LongUpDown()
            {
                Minimum = 0,
                Maximum = UInt32.MaxValue
            };
        }

        public override int Size
        {
            get { return 4; }
        }

        public override Visual Visual
        {
            get { return _visual; }
        }

        public override void SetBytes(byte[] data)
        {
            var value = EndianBitConverter.Big.ToUInt32(data, 0);

            _visual.Value = value;
        }

        public override byte[] GetBytes()
        {
            var value = (UInt32)(_visual.Value ?? 0);

            return EndianBitConverter.Big.GetBytes(value);
        }
    }
}
