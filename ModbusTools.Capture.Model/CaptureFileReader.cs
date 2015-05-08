using System;
using System.IO;

namespace ModbusTools.Capture.Model
{
    public class CaptureFileReader : IDisposable
    {
        private readonly FileStream _file;
        private readonly BinaryReader _reader;
        private readonly long _ticksPerSecond;
        private readonly int _fileFormatVersion;
        private readonly DateTime _startTime;

        public CaptureFileReader(string path)
        {
            _file = File.OpenRead(path);
            _reader = new BinaryReader(_file);
            _fileFormatVersion =_reader.ReadInt32();                        

            switch(_fileFormatVersion)
            {
                case 2:
                    break;

                default:
                    throw new InvalidOperationException("Invalid file format version.");
            }

            _ticksPerSecond = _reader.ReadInt64();
            _startTime = DateTime.FromBinary(_reader.ReadInt64());
            _reader.ReadInt32();                                    //Reserved
        }

        public DateTime StartTime
        {
            get { return _startTime; }
        }

        public int FileFormatVersion
        {
            get { return _fileFormatVersion; }
        }

        public long TicksPerSecond
        {
            get { return _ticksPerSecond;  }
        }

        public Sample Read()
        {
            try
            {
                if (_file.Position < _file.Length - 1)
                {
                    //Read the timestamp
                    var ticks = _reader.ReadInt64();
                    var value = _reader.ReadByte();

                    return new Sample(ticks, value);
                }
                else
                    return null;
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (_file != null)
                _file.Dispose();

            if (_reader != null)
                _reader.Dispose();
        }
    }
}
