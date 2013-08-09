using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Unme.Common;

namespace ModbusRegisterViewer.Model
{
    public  class CaptureFileWriter : IDisposable
    {
        private FileStream _file;
        private BinaryWriter _writer;

        private const int FileFormatVersion = 1;

        public long _sampleCount;
        private Stopwatch _stopWatch = new Stopwatch();
        

        public CaptureFileWriter(string path, long ticksPerMillisecond)
        {
            _file = File.Create(path);

            _writer = new BinaryWriter(_file);
            
            _writer.Write(FileFormatVersion);
            _writer.Write(ticksPerMillisecond);  //Reserved
            _writer.Write(0);  //Reserved

            _stopWatch.Start();
        }

        public void WriteSample(byte sample)
        {
            _writer.Write(_stopWatch.ElapsedTicks);
            _writer.Write(sample);
            _sampleCount++;
        }

        public long SampleCount
        {
            get { return _sampleCount; }
        }

        public void Dispose()
        {
            DisposableUtility.Dispose(ref _file);
            DisposableUtility.Dispose(ref _writer);
        }
    }
}
