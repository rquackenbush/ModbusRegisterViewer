using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using Modbus.Device;
using ModbusRegisterViewer.Model;
using ModbusTools.Common;
using ModbusTools.Common.Services;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveViewer.Model;

namespace ModbusTools.SlaveViewer.ViewModel
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

        private ushort _startingRegister;
        private ushort _numberOfregisters;
        private bool _writeIndividually;
        private byte _blockSize = DefaultBlockSize;
        private ObservableCollection<WriteableRegisterViewModel> _registers;

        private readonly Timer _autoRefreshTimer = new Timer(2000);

        private readonly IMessageBoxService _messageBoxService;
        private readonly IPreferences _preferences;
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly RegisterViewerPreferences _registerViewerPreferences;

        private static readonly RegisterTypeViewModel _registerTypeInput = new RegisterTypeViewModel(Common.RegisterType.Input, "Input");
        private static readonly RegisterTypeViewModel _registerTypeHolding = new RegisterTypeViewModel(Common.RegisterType.Holding, "Holding");

        private readonly DescriptionStore _descriptionStore = new DescriptionStore();

        private readonly List<RegisterTypeViewModel> _registerTypes = new List<RegisterTypeViewModel>()
        {
            _registerTypeInput,
            _registerTypeHolding
        };

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public RegisterViewerViewModel(IMessageBoxService messageBoxService, IPreferences preferences)
        {
            _messageBoxService = messageBoxService;
            _preferences = preferences;

            _registerViewerPreferences = new RegisterViewerPreferences(preferences);

            this.ReadCommand = new RelayCommand(Read, CanRead);
            this.WriteCommand = new RelayCommand(Write, CanWrite);
            this.ExitCommand = new RelayCommand(Exit);
            this.OpenCommand = new RelayCommand(Open, CanOpen);
            this.SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            this.ExportToCsvCommand = new RelayCommand(ExportToCsv, CanExportToCsv);

            if (IsInDesignMode)
            {
                this.Registers = new ObservableCollection<WriteableRegisterViewModel>()
                {
                    new WriteableRegisterViewModel(1000, 0, _descriptionStore),
                    new WriteableRegisterViewModel(1001, 20, _descriptionStore),
                    new WriteableRegisterViewModel(1002, 23, _descriptionStore),
                    new WriteableRegisterViewModel(1003, 24, _descriptionStore),
                    new WriteableRegisterViewModel(1004, 25, _descriptionStore),
                };
            }

            SlaveAddress = _registerViewerPreferences.SlaveAddress;
            StartingRegister = _registerViewerPreferences.StartingRegister;
            NumberOfRegisters = _registerViewerPreferences.NumberOfRegisters;

            RegisterType = _registerTypes.FirstOrDefault(rt => rt.RegisterType == _registerViewerPreferences.RegisterType);

            ModbusAdapters.ApplyPreferences(_preferences, RegisterViewerPreferences.Keys.ModbusAdapter);
            

            _autoRefreshTimer.Elapsed += _autoRefreshTimer_Elapsed;

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
        public ICommand ExportToCsvCommand { get; private set; }

         public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

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
                    registerViewModel = new WriteableRegisterViewModel(currentRegisterNumber, 0, _descriptionStore);
                }
                    
                newRegisters.Add(registerViewModel);

                currentRegisterNumber++;
            }

            this.Registers = newRegisters;
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
                snapshot.Registers.Select(v => new WriteableRegisterViewModel(registerNumber++, v, _descriptionStore))
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
            return _modbusAdapters.IsItemSelected && this.RegisterType == _registerTypeHolding && this.Registers != null && this.Registers.Any();
        }

        private void Write()
        {
            SavePreferences();

            try
            {
                if (this.WriteIndividually)
                {
                    var changedRegisters = this.Registers.Where(r => r.IsDirty).ToArray();

                    if (!changedRegisters.Any())
                        return;

                    ExecuteComm(m =>
                        {
                            foreach (var register in changedRegisters)
                            {
                                m.WriteSingleRegister(SlaveAddress, (ushort)(register.RegisterNumber - 1), register.Value);

                                register.IsDirty = false;
                            }        
                        });
                }
                else
                {
                    var data = this.Registers.Select(r => r.Value).ToArray();

                    ExecuteComm(m => m.WriteMultipleRegisters(SlaveAddress, (ushort)(StartingRegister - 1), data, BlockSize));

                    MarkRegistersClean();
                }
            }
            catch (Exception ex)
            {
                _messageBoxService.Show(ex, "Unable to Write");
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
            return !IsAutoRefresh && _modbusAdapters.IsItemSelected;
        }

        public void Closed()
        {
            _descriptionStore.Save();        }

        /// <summary>
        /// Executes code against a communication context
        /// </summary>
        /// <param name="action"></param>
        private void ExecuteComm(Action<IModbusMaster> action)
        {
            var contextFactory = _modbusAdapters.GetFactory();

            lock (_communicationLock)
            {
                try
                {
                    _isRunning = true;

                    using (var master = contextFactory.Create())
                    {
                        action(master.Master);
                    }
                }
                finally
                {
                    _isRunning = false;                
                }
            }
        }

        private void SavePreferences()
        {
            if (RegisterType == null)
                return;


            _registerViewerPreferences.SlaveAddress = SlaveAddress;
            _registerViewerPreferences.StartingRegister = StartingRegister;
            _registerViewerPreferences.NumberOfRegisters = NumberOfRegisters;
            _registerViewerPreferences.RegisterType = RegisterType.RegisterType;

            _modbusAdapters.GetPreferences(_preferences, RegisterViewerPreferences.Keys.ModbusAdapter);

            _preferences.Save();
        }

        private void Read()
        {
            if (_isRunning)
                return;

            try
            {
                if (RegisterType == null)
                    return;

                SavePreferences();
                
                ushort[] results = null;

                ExecuteComm(m =>
                    {
                        switch (RegisterType.RegisterType)
                        {
                            case Common.RegisterType.Input:

                                results = m.ReadInputRegisters(SlaveAddress,
                                                                        (ushort) (StartingRegister - 1),
                                                                        NumberOfRegisters,
                                                                        BlockSize);

                                break;

                            case Common.RegisterType.Holding:

                                results = m.ReadHoldingRegisters(SlaveAddress,
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
                    var rows = results.Select(r => new WriteableRegisterViewModel(registerNumber++, r, _descriptionStore));

                    Registers = new ObservableCollection<WriteableRegisterViewModel>(rows);
                }

            }
            catch (Exception ex)
            {
                _autoRefreshTimer.Enabled = false;

                if (this.IsAutoRefresh)
                {
                    this.IsAutoRefresh = false;
                }

                _messageBoxService.Show(ex, "Read Failed");
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void DescriptionChanged()
        {
            if (this.Registers == null)
                return;

            foreach (var register in this.Registers)
            {
                register.RaiseDescriptionPropertyChanged();
            }
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
            get { return RegisterTypes.FirstOrDefault(rt => rt.RegisterType == _descriptionStore.RegisterType); }
            set
            {
                if (value != null)
                {
                    _descriptionStore.RegisterType = value.RegisterType;
                }
                RaisePropertyChanged(() => RegisterType);
                DescriptionChanged();
            }
        }

        public byte SlaveAddress
        {
            get { return _descriptionStore.DeviceAddress; }
            set
            {
                _descriptionStore.DeviceAddress = value;
                RaisePropertyChanged(() => SlaveAddress);
                DescriptionChanged();
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
    }
}