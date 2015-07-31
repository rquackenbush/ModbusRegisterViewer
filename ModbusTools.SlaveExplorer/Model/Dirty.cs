using System;
using ModbusTools.SlaveExplorer.Interfaces;

namespace ModbusTools.SlaveExplorer.Model
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
