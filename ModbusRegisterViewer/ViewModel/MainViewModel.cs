using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
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
    public class MainViewModel : ViewModelBase
    {
        private RegisterTypeViewModel _registerType;
        private int? _slaveAddress;
        private int? _startingRegister;
        private int? _numberOfregisters;
        private ObservableCollection<RegisterViewModel> _registers;
        private readonly AboutViewModel _aboutViewModel = new AboutViewModel();
        private readonly ExceptionViewModel _exceptionViewModel = new ExceptionViewModel();

        private readonly List<RegisterTypeViewModel> _registerTypes = new List<RegisterTypeViewModel>()
        {
            new RegisterTypeViewModel(Model.RegisterType.Input, "Input"),
            new RegisterTypeViewModel(Model.RegisterType.Holding, "Holding")
        };

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.GetRegistersCommand = new RelayCommand(GetRegisters);

            this.ExitCommand = new RelayCommand(Exit);

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

            RegisterType = _registerTypes.FirstOrDefault(rt => (int) rt.RegisterType == settings.RegisterType);
        }

        private void GetRegisters()
        {
            try
            {
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
                settings.RegisterType = (int) RegisterType.RegisterType;

                settings.Save();

                ushort[] results;

                using (var context = new MasterContext())
                {
                    context.Master.Transport.ReadTimeout = 2000;

                    switch (RegisterType.RegisterType)
                    {
                        case Model.RegisterType.Input:

                            results = context.Master.ReadInputRegisters(slaveAddress, (ushort)(startingRegister - 1), numberOfRegisters);

                            break;

                        case Model.RegisterType.Holding:

                            results = context.Master.ReadHoldingRegisters(slaveAddress, (ushort)(startingRegister - 1), numberOfRegisters);

                            break;

                        default:
                            throw new InvalidOperationException(string.Format("Unrecognized enum value {0}", RegisterType.RegisterType));
                    }
                }

                ushort registerNumber = startingRegister;

                var rows = results.Select(r => new RegisterViewModel(registerNumber++, r));

                Registers = new ObservableCollection<RegisterViewModel>(rows);
            }
            catch (Exception ex)
            {
                this.Exception.ShowCommand.Execute(ex);

                //ex.Display();
                //MessageBox.Show(ex.Message);
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        public ICommand GetRegistersCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public ObservableCollection<RegisterViewModel> Registers
        {
            get { return _registers;  }
            private set
            {
                _registers = value;
                RaisePropertyChanged(() => Registers);
            }
        }
        
        public List<RegisterTypeViewModel> RegisterTypes
        {
            get { return _registerTypes;  }
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
            get { return _startingRegister;  }
            set
            {
                _startingRegister = value;
                RaisePropertyChanged(() => StartingRegister);
            }
        }

        public int? NumberOfRegisters
        {
            get { return _numberOfregisters;  }
            set
            {
                _numberOfregisters = value;
                RaisePropertyChanged(() => NumberOfRegisters);
            }
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