using System.Windows.Media;
using ModbusTools.SlaveExplorer.Interfaces;

namespace ModbusTools.SlaveExplorer.Runtime
{
    public abstract class RuntimeFieldBase : IRuntimeField
    {
        private readonly string _name;
        private readonly int _offset;

        protected RuntimeFieldBase(string name, int offset)
        {
            _name = name;
            _offset = offset;
        }

        public int Offset
        {
            get { return _offset; }   
        }

        public abstract int Size { get; }

        public string Name
        {
            get { return _name; }
        }

        public abstract Visual Visual { get; }

        public abstract void SetBytes(byte[] data);

        public abstract byte[] GetBytes();
    }
}
