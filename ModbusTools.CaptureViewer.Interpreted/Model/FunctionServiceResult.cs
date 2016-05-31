using System;
using System.Windows.Media;

namespace ModbusTools.CaptureViewer.Interpreted.Model
{
    public class FunctionServiceResult
    {
        private readonly string _summary;
        private readonly PacketType? _packetType;
        private readonly string _error;
        private readonly Lazy<Visual> _visual;

        public FunctionServiceResult(string summary = null, Func<Visual> visualFactory = null, PacketType? packetType = null, string error = null)
        {
            _summary = summary;
            _packetType = packetType;
            _error = error;

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

        public PacketType? PacketType
        {
            get { return _packetType; }
        }

        public string Error
        {
            get { return _error; }
        }
    }
}