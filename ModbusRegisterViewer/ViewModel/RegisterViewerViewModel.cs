using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using FtdAdapter;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RegisterViewerViewModel : ViewModelBase
    {
        private readonly object _communicationLock = new object();
        private bool _isRunning = false;

        private RegisterTypeViewModel _registerType;
        private int? _slaveAddress;
        private int? _startingRegister;
        private int? _numberOfregisters;
        private ObservableCollection<RegisterViewModel> _registers;
        private readonly AboutViewModel _aboutViewModel = new AboutViewModel();
        private readonly ExceptionViewModel _exceptionViewModel = new ExceptionViewModel();

        private readonly Timer _autoRefreshTimer = new Timer(2000);

        private readonly ObservableCollection<AdapterViewModel> _adapters = new ObservableCollection<AdapterViewModel>();
        private AdapterViewModel _selectedAdapter;

        private readonly List<RegisterTypeViewModel> _registerTypes = new List<RegisterTypeViewModel>()
        {
            new RegisterTypeViewModel(Model.RegisterType.Input, "Input"),
            new RegisterTypeViewModel(Model.RegisterType.Holding, "Holding")
        };

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public RegisterViewerViewModel()
        {
            this.GetRegistersCommand = new RelayCommand(GetRegisters, CanGetRegisters);
            this.ExitCommand = new RelayCommand(Exit);
            this.OpenCommand = new RelayCommand(Open, CanOpen);
            this.SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            this.RefreshAdaptersCommand = new RelayCommand(RefreshAdapters);

            if (IsInDesignMode)
            {
                this.Registers = new ObservableCollection<RegisterViewModel>()
                {
                    new RegisterViewModel(1000, 0),
                    new RegisterViewModel(1001, 20),
                    new RegisterViewModel(1002, 23),
                    new RegisterViewModel(1003, 24),
                    new RegisterViewModel(1004, 25),
                };
            }

            var settings = Properties.Settings.Default;

            SlaveAddress = settings.SlaveAddress;
            StartingRegister = settings.StartingRegister;
            NumberOfRegisters = settings.NumberOfRegisters;

            RegisterType = _registerTypes.FirstOrDefault(rt => (int)rt.RegisterType == settings.RegisterType);

            _autoRefreshTimer.Elapsed += _autoRefreshTimer_Elapsed;

            RefreshAdapters();

            SelectAdapter(settings.AdapterSerialNumber);
        }

        void _autoRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(GetRegisters);
        }
        
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand GetRegistersCommand { get; private set; }
        public ICommand RefreshAdaptersCommand { get; private set; }

        private void RefreshAdapters()
        {
            string selectedSerialNumber = null;

            if (this.SelectedAdapter != null)
                selectedSerialNumber = this.SelectedAdapter.SerialNumber;

            this.Adapters.Clear();

            var infos = FtdUsbPort.GetDeviceInfos();

            foreach (var info in infos)
            {
                this.Adapters.Add(new AdapterViewModel(info));
            }

            //Attempt to reselect the original adapter.
            SelectAdapter(selectedSerialNumber);
        }

        /// <summary>
        /// Selects an adapter.
        /// </summary>
        /// <param name="serialNumber"></param>
        private void SelectAdapter(string serialNumber)
        {
            var initialAdapter = this.Adapters.FirstOrDefault(a => a.SerialNumber == serialNumber);

            if (initialAdapter == null)
                initialAdapter = this.Adapters.FirstOrDefault();

            this.SelectedAdapter = initialAdapter;
        }

        private bool CanOpen()
        {
            return !this.IsAutoRefresh;
        }

        private void Open()
        {
            var openFileDialog = new OpenFileDialog()
                {
                    Filter = "Xml File|*.xml"
                };

            if (openFileDialog.ShowDialog() != true)
                return;

            var snapshot = DataContractUtilities.FromFile<Snapshot>(openFileDialog.FileName);

            var registerNumber = snapshot.StartingRegister;

            this.Registers = new ObservableCollection<RegisterViewModel>(
                snapshot.Registers.Select(v => new RegisterViewModel(registerNumber++, v))
            );
        }

        private bool CanSaveAs()
        {
            return !this.IsAutoRefresh
                && this.Registers != null 
                && this.Registers.Any() 
                && this.RegisterType != null
                && this.SlaveAddress.HasValue
                && this.StartingRegister.HasValue;
        }

        private void SaveAs()
        {

            var saveFileDialog = new SaveFileDialog()
                {
                    Filter = "Xml File|*.xml"
                };

            if (saveFileDialog.ShowDialog() != true)
                return;

            var snapshot = new Snapshot()
                {
                    RegisterType = this.RegisterType.RegisterType,
                    SlaveId = (byte) this.SlaveAddress.Value,
                    StartingRegister = (ushort) this.StartingRegister.Value,
                    Registers = this.Registers.Select(r => r.Value).ToArray()
                };

            DataContractUtilities.ToFile(saveFileDialog.FileName, snapshot);
        }

        private bool CanGetRegisters()
        {
            return !this.IsAutoRefresh && this.SelectedAdapter != null;
        }

        private void GetRegisters()
        {
            if (_isRunning)
                return;

            try
            {
                lock (_communicationLock)
                {
                    try
                    {
                        _isRunning = true;

                        if (!SlaveAddress.HasValue)
                            return;

                        if (!StartingRegister.HasValue)
                            return;

                        if (!NumberOfRegisters.HasValue)
                            return;

                        var slaveAddress = (byte)SlaveAddress.Value;
                        var startingRegister = (ushort)(StartingRegister.Value);
                        var numberOfRegisters = (ushort)NumberOfRegisters.Value;

                        if (RegisterType == null)
                            return;

                        //Save theese query criteria
                        var settings = Properties.Settings.Default;

                        settings.SlaveAddress = slaveAddress;
                        settings.StartingRegister = startingRegister;
                        settings.NumberOfRegisters = numberOfRegisters;
                        settings.RegisterType = (int)RegisterType.RegisterType;

                        if (this.SelectedAdapter == null)
                            settings.AdapterSerialNumber = null;
                        else
                            settings.AdapterSerialNumber = this.SelectedAdapter.SerialNumber;

                        settings.Save();

                        ushort[] results;

                        using (var context = new MasterContext(this.SelectedAdapter.SerialNumber))
                        {
                            context.Master.Transport.ReadTimeout = 2000;

                            switch (RegisterType.RegisterType)
                            {
                                case Model.RegisterType.Input:

                                    results = context.Master.ReadInputRegisters(slaveAddress,
                                                                                (ushort)(startingRegister - 1),
                                                                                numberOfRegisters);

                                    break;

                                case Model.RegisterType.Holding:

                                    results = context.Master.ReadHoldingRegisters(slaveAddress,
                                                                                  (ushort)(startingRegister - 1),
                                                                                  numberOfRegisters);

                                    break;

                                default:
                                    throw new InvalidOperationException(string.Format("Unrecognized enum value {0}",
                                                                                      RegisterType.RegisterType));
                            }
                        }

                        ushort registerNumber = startingRegister;

                        var rows = results.Select(r => new RegisterViewModel(registerNumber++, r));

                        Registers = new ObservableCollection<RegisterViewModel>(rows);
                    }
                    finally
                    {
                        _isRunning = false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.IsAutoRefresh)
                {
                    this.IsAutoRefresh = false;
                }

                this.Exception.ShowCommand.Execute(ex);
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        public ObservableCollection<RegisterViewModel> Registers
        {
            get { return _registers; }
            private set
            {
                _registers = value;
                RaisePropertyChanged(() => Registers);
            }
        }

        public List<RegisterTypeViewModel> RegisterTypes
        {
            get { return _registerTypes; }
        }

        public bool IsAutoRefresh
        {
            get { return _autoRefreshTimer.Enabled; }
            set
            {
                if (_autoRefreshTimer.Enabled != value)
                {
                    _autoRefreshTimer.Enabled = value;

                    if (value)
                    {
                        GetRegisters();
                    }

                    RaisePropertyChanged(() => IsAutoRefresh);
                }
            }
        }

        public RegisterTypeViewModel RegisterType
        {
            get { return _registerType; }
            set
            {
                _registerType = value;
                RaisePropertyChanged(() => RegisterType);
            }
        }

        public int? SlaveAddress
        {
            get { return _slaveAddress; }
            set
            {
                _slaveAddress = value;
                RaisePropertyChanged(() => SlaveAddress);
            }
        }

        public int? StartingRegister
        {
            get { return _startingRegister; }
            set
            {
                _startingRegister = value;
                RaisePropertyChanged(() => StartingRegister);
            }
        }

        public int? NumberOfRegisters
        {
            get { return _numberOfregisters; }
            set
            {
                _numberOfregisters = value;
                RaisePropertyChanged(() => NumberOfRegisters);
            }
        }

        public AdapterViewModel SelectedAdapter
        {
            get { return _selectedAdapter; }
            set
            {
                _selectedAdapter = value;
                RaisePropertyChanged(() => SelectedAdapter);
             }
        }

        public ObservableCollection<AdapterViewModel>  Adapters
        {
            get { return _adapters; }
        }

        public AboutViewModel About
        {
            get { return _aboutViewModel; }
        }

        public ExceptionViewModel Exception
        {
            get { return _exceptionViewModel; }
        }

    }
}