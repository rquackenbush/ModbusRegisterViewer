using System;
using System.Windows.Media;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class FunctionServiceResult
    {
        private readonly string _summary;
        private readonly Lazy<Visual> _visual;

        public FunctionServiceResult(string summary, Func<Visual> visualFactory = null)
        {
            _summary = summary;

            if (visualFactory != null)
            {
                _visual = new Lazy<Visual>(visualFactory);
            }
        }

        public string Summary
        {
            get { return _summary; }
        }

        public Visual Visual
        {
            get { return _visual?.Value; }
        }
    }
}