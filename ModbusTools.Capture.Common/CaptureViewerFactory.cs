using System;
using System.Windows;

namespace ModbusTools.Capture.Common
{
    public class CaptureViewerFactory : ICaptureViewerFactory
    {
        private readonly string _name;
        private readonly Func<string, Window> _viewFactory;

        public CaptureViewerFactory(string name, Func<string, Window> viewFactory)
        {
            if (viewFactory == null) throw new ArgumentNullException(nameof(viewFactory));

            _name = name;
            _viewFactory = viewFactory;
        }

        public string Name
        {
            get { return _name; }
        }

        public Window Open(string filename)
        {
            return _viewFactory(filename);
        }
    }
}
