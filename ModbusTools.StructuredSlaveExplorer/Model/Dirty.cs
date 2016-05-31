using ModbusTools.StructuredSlaveExplorer.Interfaces;

namespace ModbusTools.StructuredSlaveExplorer.Model
{
    public class Dirty : IDirty
    {
        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
        }

        public void MarkDirty()
        {
            if (!_isDirty)
            {
                _isDirty = true;
            }
        }

        public void MarkClean()
        {
            _isDirty = false;
        }
    }
}
