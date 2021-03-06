﻿using System;
using GalaSoft.MvvmLight;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class ActivityViewModel : ViewModelBase
    {
        private readonly DateTime _timestamp;
        private readonly string _operation;
        private readonly ushort? _startingIndex;
        private readonly string[] _values;

        public ActivityViewModel(DateTime timestamp, string operation, ushort? startingIndex = null, string[] values = null)
        {
            _timestamp = timestamp;
            _operation = operation;
            _startingIndex = startingIndex;
            _values = values;
        }

        public string Operation
        {
            get { return _operation; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public ushort? StartingAddress
        {
            get { return _startingIndex; }
        }

        public int? Count
        {
            get
            {
                if (_values == null)
                    return null;

                return _values.Length;
            }
        }

        public string Values
        {
            get { return string.Join(" ", _values); }
        }
    }
}
