﻿using System;
using System.Windows.Media;
using ModbusTools.StructuredSlaveExplorer.Interfaces;

namespace ModbusTools.StructuredSlaveExplorer.Runtime
{
    public class RuntimeFieldEditor<TVisual> : IRuntimeFieldEditor
        where TVisual : Visual
    {
        private readonly string _name;
        private readonly TVisual _visual;

        public RuntimeFieldEditor(string name, TVisual visual)
        {
            if (visual == null) 
                throw new ArgumentNullException(nameof(visual));

            _name = name;
            _visual = visual;
        }

        public string Name
        {
            get { return _name; }
        }

        public TVisual Visual
        {
            get { return _visual; }
        }

        string IRuntimeFieldEditor.Name
        {
            get { return _name; }
        }

        Visual IRuntimeFieldEditor.Visual
        {
            get { return _visual; }
        }
    }
}
