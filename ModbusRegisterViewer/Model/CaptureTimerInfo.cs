﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.Model
{
    public class CaptureTimerInfo
    {
        private readonly DateTime _startTime;
        private readonly long _ticksPerSecond;

        public CaptureTimerInfo(DateTime startTime, long ticksPerSecond)
        {
            _startTime = startTime;
            _ticksPerSecond = ticksPerSecond;
        }

        public DateTime StartTime
        {
            get { return _startTime; }
        }

        public long TicksPerSecond
        {
            get { return _ticksPerSecond; }
        }
    }
}
