using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unme.Common;

namespace ModbusRegisterViewer.Model
{
    public class CaptureFileReader : IDisposable
    {
        private FileStream _file;
        private BinaryReader _reader;
        private readonly long _ticksPerSecond;
        private readonly int _fileFormatVersion;
        private DateTime _startTime;

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
            if (_file.Position < _file.Length - 1)
            {
                //Read the timestamp
                var ticks = _reader.ReadInt64();
                var value = _reader.ReadByte();

                return new Sample(ticks, value);
           }

           return null;
        }

        public void Dispose()
        {
            DisposableUtility.Dispose(ref _file);
            DisposableUtility.Dispose(ref _reader);
        }
    }
}
