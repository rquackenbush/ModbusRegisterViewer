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
        private readonly long _ticksPerMillisecond;
        private readonly int _fileFormatVersion;

        public CaptureFileReader(string path)
        {
            _file = File.OpenRead(path);
            _reader = new BinaryReader(_file);

            _reader.ReadInt32();                        //FileFormatVersion
            _ticksPerMillisecond = _reader.ReadInt64(); //TicksPerMillisecond
            _reader.ReadInt32();                        //Reserved
        }

        public int FileFormatVersion
        {
            get { return _fileFormatVersion; }
        }

        public long TicksPerMillisecond
        {
            get { return _ticksPerMillisecond;  }
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
