using System;
using System.Diagnostics;
using System.IO;

namespace ModbusTools.Capture.Model
{
    public  class CaptureFileWriter : IDisposable
    {
        private readonly FileStream _file;
        private readonly BinaryWriter _writer;

        private const int FileFormatVersion = 2;

        public long _sampleCount;
        private readonly Stopwatch _stopWatch = new Stopwatch();

        public CaptureFileWriter(string path)
        {
            _file = File.Create(path);

            _writer = new BinaryWriter(_file);
            
            _writer.Write(FileFormatVersion);
            _writer.Write(Stopwatch.Frequency);  //ticks per millisecond
            _writer.Write(DateTime.Now.ToBinary());  //Write the start time of this capture.  This will be local time.
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
            if (_stopWatch.IsRunning)
                _stopWatch.Stop();

            _file?.Dispose();
            _writer?.Dispose();
        }
    }
}
