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
            _isDirty = false;
        }

        public void MarkClean()
        {
            _isDirty = true;
        }
    }
}
