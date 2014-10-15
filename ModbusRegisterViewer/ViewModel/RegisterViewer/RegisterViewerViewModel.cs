using System;
using System.Collections.ObjectModel;
using System.IO;
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
using ModbusRegisterViewer.Services;
using ModbusRegisterViewer.ViewModel.RegisterViewer;

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
        private const byte DefaultBlockSize = 125;

        private readonly object _communicationLock = new object();
        private bool _isRunning;

        private RegisterTypeViewModel _registerType;
        private byte _slaveAddress;
        private ushort _startingRegister;
        private ushort _numberOfregisters;
        private bool _writeIndividually;
        private byte _blockSize = DefaultBlockSize;
        private ObservableCollection<WriteableRegisterViewModel> _registers;
        private readonly AboutViewModel _aboutViewModel = new AboutViewModel();
        private readonly ExceptionViewModel _exceptionViewModel = new ExceptionViewModel();

        private readonly Timer _autoRefreshTimer = new Timer(2000);

        private readonly IMessageBoxService _messageBoxService;

        private readonly ObservableCollection<AdapterViewModel> _adapters = new ObservableCollection<AdapterViewModel>();
        private AdapterViewModel _selectedAdapter;

        private static readonly RegisterTypeViewModel _registerTypeInput = new RegisterTypeViewModel(Model.RegisterType.Input, "Input");
        private static readonly RegisterTypeViewModel _registerTypeHolding = new RegisterTypeViewModel(Model.RegisterType.Holding, "Holding");

        private readonly List<RegisterTypeViewModel> _registerTypes = new List<RegisterTypeViewModel>()
        {
            _registerTypeInput,
            _registerTypeHolding
        };

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public RegisterViewerViewModel(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;

            this.ReadCommand = new RelayCommand(Read, CanRead);
            this.WriteCommand = new RelayCommand(Write, CanWrite);
            this.ExitCommand = new RelayCommand(Exit);
            this.OpenCommand = new RelayCommand(Open, CanOpen);
            this.SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            this.RefreshAdaptersCommand = new RelayCommand(RefreshAdapters);
            this.ExportToCsvCommand = new RelayCommand(ExportToCsv, CanExportToCsv);

            if (IsInDesignMode)
            {
                this.Registers = new ObservableCollection<WriteableRegisterViewModel>()
                {
                    new WriteableRegisterViewModel(1000, 0),
                    new WriteableRegisterViewModel(1001, 20),
                    new WriteableRegisterViewModel(1002, 23),
                    new WriteableRegisterViewModel(1003, 24),
                    new WriteableRegisterViewModel(1004, 25),
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
            DispatcherHelper.CheckBeginInvokeOnUI(Read);
        }
        
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand RefreshAdaptersCommand { get; private set; }
        public ICommand ExportToCsvCommand { get; private set; }

        private void FillInRegisters()
        {
            Dictionary<ushort, WriteableRegisterViewModel> existingValues;

            if (this.Registers == null)
            {
                existingValues = new Dictionary<ushort, WriteableRegisterViewModel>();
            }
            else
            {
                existingValues = this.Registers.ToDictionary(r => r.RegisterNumber, r => r);
            }

            //This is where the new reigsters will be
            var newRegisters = new ObservableCollection<WriteableRegisterViewModel>();

            
            //This is the register number for each iteration
            var currentRegisterNumber = StartingRegister;

            //Iterate through the new number of registers
            for (int registerIndex = 0; registerIndex < NumberOfRegisters; registerIndex++)
            {
                WriteableRegisterViewModel registerViewModel;

                if (!existingValues.TryGetValue(currentRegisterNumber, out registerViewModel))
                {
                    registerViewModel = new WriteableRegisterViewModel(currentRegisterNumber, 0);
                }
                    
                newRegisters.Add(registerViewModel);

                currentRegisterNumber++;
            }
            

            this.Registers = newRegisters;
        }

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

            this.Registers = new ObservableCollection<WriteableRegisterViewModel>(
                snapshot.Registers.Select(v => new WriteableRegisterViewModel(registerNumber++, v))
            );
        }

        private bool CanSaveAs()
        {
            return !this.IsAutoRefresh
                   && this.Registers != null
                   && this.Registers.Any()
                   && this.RegisterType != null;
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
                    RegisterType = RegisterType.RegisterType,
                    SlaveId = SlaveAddress,
                    StartingRegister = (ushort) StartingRegister,
                    Registers = Registers.Select(r => r.Value).ToArray()
                };

            DataContractUtilities.ToFile(saveFileDialog.FileName, snapshot);
        }

        private bool CanExportToCsv()
        {
            return Registers != null && Registers.Any();
        }

        private void ExportToCsv()
        {
            var fileDialog = new SaveFileDialog()
                {
                    Filter = "Comma Separated Value (.csv)|*.csv"
                };

            if (fileDialog.ShowDialog() != true)
                return;

            using (var writer = File.CreateText(fileDialog.FileName))
            {
                writer.WriteLine("Register,Value");

                foreach (var register in this.Registers)
                {
                    writer.WriteLine("{0},{1}", register.RegisterNumber, register.Value);
                }
            }
        }

        private bool CanWrite()
        {
            return this.RegisterType == _registerTypeHolding && this.Registers != null && this.Registers.Any();
        }

        private void Write()
        {
            try
            {
                if (this.WriteIndividually)
                {
                    var changedRegisters = this.Registers.Where(r => r.IsDirty).ToArray();

                    if (!changedRegisters.Any())
                        return;

                    ExecuteComm(context =>
                        {
                            foreach (var register in changedRegisters)
                            {
                                context.Value.Master.WriteSingleRegister(SlaveAddress, (ushort)(register.RegisterNumber - 1), register.Value);

                                register.IsDirty = false;
                            }        
                        });
                }
                else
                {
                    var data = this.Registers.Select(r => r.Value).ToArray();

                    ExecuteComm(context => context.Value.Master.WriteMultipleRegisters(SlaveAddress, (ushort)(StartingRegister - 1), data, BlockSize));

                    MarkRegistersClean();
                }
            }
            catch (Exception ex)
            {
                this.Exception.ShowCommand.Execute(ex);
            }
        }

        private void MarkRegistersClean()
        {
            if (this.Registers == null)
                return;

            foreach (var register in this.Registers)
            {
                register.IsDirty = false;
            }
        }

        private bool CanRead()
        {
            return !this.IsAutoRefresh && this.SelectedAdapter != null;
        }

        /// <summary>
        /// Executes code against a communication context
        /// </summary>
        /// <param name="action"></param>
        private void ExecuteComm(Action<Lazy<MasterContext>> action)
        {
            lock (_communicationLock)
            {
                //We don't want to have to create this if we don't have to, so use a lazy loader
                Lazy<MasterContext> context = new Lazy<MasterContext>(() =>
                    {
                        var result = new MasterContext(this.SelectedAdapter.SerialNumber);

                        result.Master.Transport.ReadTimeout = 2000;
                        result.Master.Transport.WriteTimeout = 2000;

                        return result;
                    });

                try
                {
                    _isRunning = true;

                    //Execute the action
                    action(context);
                }
                finally
                {
                    _isRunning = false;

                    if (context.IsValueCreated)
                    {
                        context.Value.Dispose();
                    }
                }
            }
        }

        private void Read()
        {
            if (_isRunning)
                return;

            try
            {
                if (RegisterType == null)
                    return;

                //Save theese query criteria
                var settings = Properties.Settings.Default;

                settings.SlaveAddress = SlaveAddress;
                settings.StartingRegister = StartingRegister;
                settings.NumberOfRegisters = NumberOfRegisters;
                settings.RegisterType = (int)RegisterType.RegisterType;

                if (this.SelectedAdapter == null)
                    settings.AdapterSerialNumber = null;
                else
                    settings.AdapterSerialNumber = this.SelectedAdapter.SerialNumber;

                settings.Save();

                ushort[] results = null;

                ExecuteComm(c =>
                    {
                        switch (RegisterType.RegisterType)
                        {
                            case Model.RegisterType.Input:

                                results = c.Value.Master.ReadInputRegisters(SlaveAddress,
                                                                        (ushort) (StartingRegister - 1),
                                                                        NumberOfRegisters,
                                                                        BlockSize);

                                break;

                            case Model.RegisterType.Holding:

                                results = c.Value.Master.ReadHoldingRegisters(SlaveAddress,
                                                                                (ushort)(StartingRegister - 1),
                                                                                NumberOfRegisters,
                                                                                this.BlockSize);
                                

                                break;

                            default:
                                throw new InvalidOperationException(string.Format("Unrecognized enum value {0}",
                                                                                    RegisterType.RegisterType));
                        }
                    });

                ushort registerNumber = StartingRegister;

                if (results != null)
                {
                    var rows = results.Select(r => new WriteableRegisterViewModel(registerNumber++, r));

                    Registers = new ObservableCollection<WriteableRegisterViewModel>(rows);
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

        public ObservableCollection<WriteableRegisterViewModel> Registers
        {
            get { return _registers; }
            private set
            {
                _registers = value;
                RaisePropertyChanged(() => Registers);
            }
        }

        public byte BlockSize
        {
            get { return _blockSize; }
            set
            {
                _blockSize = value;
                RaisePropertyChanged(() => BlockSize);
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
                        Read();
                    }

                    RaisePropertyChanged(() => IsAutoRefresh);
                }
            }
        }

        public bool WriteIndividually
        {
            get { return _writeIndividually; }
            set
            {
                _writeIndividually = value;
                RaisePropertyChanged(() => WriteIndividually);
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

        public byte SlaveAddress
        {
            get { return _slaveAddress; }
            set
            {
                _slaveAddress = value;
                RaisePropertyChanged(() => SlaveAddress);
            }
        }

        public ushort StartingRegister
        {
            get { return _startingRegister; }
            set
            {
                _startingRegister = value;
                RaisePropertyChanged(() => StartingRegister);
                FillInRegisters();
            }
        }

        public ushort NumberOfRegisters
        {
            get { return _numberOfregisters; }
            set
            {
                _numberOfregisters = value;
                RaisePropertyChanged(() => NumberOfRegisters);
                FillInRegisters();
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