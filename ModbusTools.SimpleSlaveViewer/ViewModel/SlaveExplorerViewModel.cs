using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Input;
using Cas.Common.WPF.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using ModbusTools.Common;
using ModbusTools.Common.Model;
using ModbusTools.Common.ViewModel;
using ModbusTools.SimpleSlaveExplorer.Model;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class SlaveExplorerViewModel : ViewModelBase, ISlaveExplorerContext
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly IPreferences _preferences;
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly RegisterViewerPreferences _registerViewerPreferences;

        private readonly DescriptionStore _descriptionStore = new DescriptionStore();
        private readonly ObservableCollection<string> _logEntries = new ObservableCollection<string>();

        private int _errorCount;
        private int _readCount;

        private readonly CoilsViewModel _coils;
        private readonly InputsViewModel _inputs;
        private readonly HoldingRegistersViewModel _holdingRegisters;
        private readonly InputRegistersViewModel _inputRegisters;

        private readonly Timer _pollingTimer;
        private bool _isPollingCancelled;
        private IPoints _pointsToPoll;
        private double _pollingInterval = 2.0;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SlaveExplorerViewModel(IMessageBoxService messageBoxService, IPreferences preferences)
        {
            _messageBoxService = messageBoxService;
            _preferences = preferences;

            _registerViewerPreferences = new RegisterViewerPreferences(preferences);

            _coils = new CoilsViewModel(this);
            _inputs = new InputsViewModel(this);
            _holdingRegisters = new HoldingRegistersViewModel(this);
            _inputRegisters = new InputRegistersViewModel(this);
            
            SlaveAddress = _registerViewerPreferences.SlaveAddress;
            
            ModbusAdapters.ApplyPreferences(_preferences, RegisterViewerPreferences.Keys.ModbusAdapter);

            _pollingTimer = new Timer()
            {
                AutoReset = false
            };

            _pollingTimer.Elapsed += PollingTimerEllapsed;

            StopPollingCommand = new RelayCommand(StopPolling, CanStopPolling);
        }

        public ICommand StopPollingCommand { get; }

        private void StopPolling()
        {
            _isPollingCancelled = true;
        }

        private bool CanStopPolling()
        {
            return _pointsToPoll != null && !_isPollingCancelled;
        }
        
        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public void Closed()
        {
            _descriptionStore.Save();        
        }

        private void AddLogEntry(string message)
        {
            var now = DateTime.Now;

            var formatted = $"{now.ToShortDateString()} {now.ToShortTimeString()} - {message}";

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                LogEntries.Add(formatted);
            });
        }

        public int ErrorCount
        {
            get { return _errorCount; }
            private set
            {
                _errorCount = value; 
                RaisePropertyChanged();
            }
        }

        public int ReadCount
        {
            get { return _readCount; }
            private set
            {
                _readCount = value; 
                RaisePropertyChanged();
            }
        }
 
        public byte SlaveAddress
        {
            get { return _descriptionStore.DeviceAddress; }
            set
            {
                _descriptionStore.DeviceAddress = value;
                RaisePropertyChanged(() => SlaveAddress);
            }
        }
        
        public ObservableCollection<string> LogEntries
        {
            get { return _logEntries; }
        }

        public byte SlaveId
        {
            get { return SlaveAddress; }
        }

        public IModbusAdapterProvider ModbusAdapterProvider
        {
            get { return _modbusAdapters; }
        }

        public IMessageBoxService MessageBoxService
        {
            get { return _messageBoxService; }
        }

        /// <summary>
        /// Gets or sets the number of seconds to wait in between polling attempts.
        /// </summary>
        public double PollingInterval
        {
            get { return _pollingInterval; }
            set
            {
                _pollingInterval = value; 
                RaisePropertyChanged();
            }
        }

        public void StartPolling(IPoints points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));

            if (_pointsToPoll != null)
                return;

            _pointsToPoll = points;
            _isPollingCancelled = false;
            ReadCount = 0;
            ErrorCount = 0;

            _pointsToPoll = points;

            _pollingTimer.Interval = PollingInterval * 1000;
            _pollingTimer.Elapsed += PollingTimerEllapsed;
            _pollingTimer.Enabled = true;
        }

        private void PollingTimerEllapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_isPollingCancelled)
                {
                    _pointsToPoll = null;
                }
                else
                {
                    _pointsToPoll.Read();

                    ReadCount++;
                }
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message);

                ErrorCount++;
            }
            finally
            {
                if (!_isPollingCancelled && _pointsToPoll != null)
                {
                    _pollingTimer.Start();
                }
            }
        }

        public bool IsPolling
        {
            get { return _pointsToPoll != null; }
        }

        public CoilsViewModel Coils
        {
            get { return _coils; }
        }

        public InputsViewModel Inputs
        {
            get { return _inputs; }
        }

        public HoldingRegistersViewModel HoldingRegisters
        {
            get { return _holdingRegisters; }
        }

        public InputRegistersViewModel InputRegisters
        {
            get { return _inputRegisters; }
        }
    }
}