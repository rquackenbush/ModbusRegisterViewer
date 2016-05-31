using System;
using GalaSoft.MvvmLight;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public abstract class ActivityViewModel : ViewModelBase
    {
        private readonly DateTime _timestamp;
        private readonly string _operation;
        private readonly int? _startingIndex;

        protected ActivityViewModel(DateTime timestamp, string operation, int? startingIndex = null, bool isZeroBased = false)
        {
            _timestamp = timestamp;
            _operation = operation;
            _startingIndex = startingIndex;
        }

        public string Operation
        {
            get { return _operation; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public int? StartingAddress
        {
            get
            {
                if (_startingIndex == null)
                    return null;

                if (IsZeroBased)
                    return _startingIndex;

                return _startingIndex + 1;
            }
        }

        public virtual int? Count
        {
            get { return null; }
        }

        public abstract string Summary { get; }

        private bool _isZeroBased;
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set
            {
                _isZeroBased = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => StartingAddress);
            }
        }
    }

    public abstract class ActivityViewModel<TData> : ActivityViewModel
        where TData : struct
    {
        
        private readonly TData[] _data;

        protected ActivityViewModel(DateTime timestamp, string operation,  int startingIndex, TData[] data, bool isZeroBased)
            : base(timestamp, operation, startingIndex, isZeroBased)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data;
        }

        public TData[] Data
        {
            get { return _data; }
        }

        public override int? Count
        {
            get { return _data.Length; }
        }

        

        
    }
}
