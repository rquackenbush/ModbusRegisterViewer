using System.Collections.ObjectModel;
using Cas.Common.WPF.Interfaces;
using GalaSoft.MvvmLight;
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
        }
        
        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public void Closed()
        {
            _descriptionStore.Save();        
        }

        //private void AddLogEntry(string message)
        //{
        //    var now = DateTime.Now;

        //    var formatted = $"{now.ToShortDateString()} {now.ToShortTimeString()} - {message}";

        //    LogEntries.Add(formatted);
        //}
 
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