using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Modbus.Data;
using Modbus.Device;
using ModbusRegisterViewer.Properties;
using ModbusTools.Common;
using System.Windows.Data;

namespace ModbusRegisterViewer.ViewModel.SlaveSimulator
{
    public class SlaveSimulatorViewModel : ViewModelBase
    {
        private SerialPort _port;
        private ModbusSerialSlave _slave;
        private int? _slaveAddress;
        private readonly ObservableCollection<ActivityLogViewModel> _activityLog = new ObservableCollection<ActivityLogViewModel>();

        private readonly DataStore _dataStore;

        private readonly ObservableCollection<SlaveSimulatorRegisterViewModel> _holdingRegisters;
        private readonly ObservableCollection<SlaveSimulatorRegisterViewModel> _inputRegisters;

        private readonly ICollectionView _holdingView;
        private readonly ICollectionView _inputView;

        public SlaveSimulatorViewModel()
        {
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            ClearCommand = new RelayCommand(Clear, CanClear);

            var settings = Settings.Default;

            SlaveAddress = settings.SlaveSimulatorSlaveAddress;

            //Create the data store
            _dataStore = DataStoreFactory.CreateDefaultDataStore();

            //Here are the listeners
            _dataStore.DataStoreReadFrom += DataStoreOnDataStoreReadFrom;
            _dataStore.DataStoreWrittenTo += DataStoreOnDataStoreWrittenTo;

            //Set up the holding registers
            {
                ushort registerNumber = 0;

                _holdingRegisters =
                    _dataStore.HoldingRegisters.Select(
                        r => new SlaveSimulatorRegisterViewModel(_dataStore.HoldingRegisters, registerNumber++)).ToObservableCollection();
            }

            //Set up the input registers
            {
                ushort registerNumber = 0;

                _inputRegisters =
                    _dataStore.InputRegisters.Select(
                        r => new SlaveSimulatorRegisterViewModel(_dataStore.InputRegisters, registerNumber++)).ToObservableCollection();
            }

            _holdingView = CollectionViewSource.GetDefaultView(_holdingRegisters);
            _inputView = CollectionViewSource.GetDefaultView(_inputRegisters);

            _holdingView.Filter = RegisterFilter;
            _inputView.Filter = RegisterFilter;

            RefreshPortNamesCommand = new RelayCommand(RefreshPortNames, CanRefreshPortNames);

            RefreshPortNames();

            if (PortNames != null)
            {
                var existing = PortNames.FirstOrDefault(p => p == settings.SlaveSimulatorSerialPortName);

                if (existing == null)
                {
                    PortName = PortNames.FirstOrDefault();
                }
                else
                {
                    PortName = existing;
                }
            }
        }

        private bool RegisterFilter(object item)
        {
            var register = item as SlaveSimulatorRegisterViewModel;

            if (register == null)
                return true;

            if (!OnlyShowTouched)
                return true;

            return register.HasBeenTouched;
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand RefreshPortNamesCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        private void Clear()
        {
            ActivityLog.Clear();
        }

        private bool CanClear()
        {
            return ActivityLog.Count > 0;
        }

        private void Start()
        {
            var settings = Settings.Default;

            if (!SlaveAddress.HasValue)
                return;

            settings.SlaveSimulatorSlaveAddress = SlaveAddress ?? 0;
            settings.SlaveSimulatorSerialPortName = PortName;

            settings.Save();

            _port = new SerialPort(PortName);

            // configure serial port
            _port.BaudRate = 19200;
            _port.DataBits = 8;
            _port.Parity = Parity.Even;
            _port.StopBits = StopBits.One;

            _port.Open();

            //_port.ReadTimeout = 2000;
            //_port.WriteTimeout = 2000;

            var slaveAddresss = (byte) SlaveAddress;

            _slave = ModbusSerialSlave.CreateRtu(slaveAddresss, _port);

            _slave.DataStore = _dataStore;

            var task = new Task(() =>
            {
                try
                {
                    _slave.Listen();
                }
                catch (IOException)
                {
                    //We'll just ignore this.
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Stop();

                DispatcherHelper.CheckBeginInvokeOnUI(CommandManager.InvalidateRequerySuggested);
                   
            });

            task.Start();
         }

        void RefreshPortNames()
        {
            this.PortNames = SerialPort.GetPortNames();
        }

        bool CanRefreshPortNames()
        {
            return _port == null;
        }

        private bool CanStart()
        {
            return _port == null && !string.IsNullOrWhiteSpace(PortName ) && SlaveAddress.HasValue;
        }

        private void Stop()
        {
            if (_port != null)
            {
                _port.ReadTimeout = 1;
                _port.WriteTimeout = 1;
                _port.Dispose();

                _port = null;
            }

            if (_slave != null)
            {
                _slave.Dispose();

                _slave = null;
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

        private bool CanStop()
        {
            return _port != null;
        }

        private static ActivityLogViewModel CreateLog(DataStoreEventArgs args, ReadWrite readWrite)
        {
            return new ActivityLogViewModel(DateTime.Now, args.ModbusDataType, args.StartAddress, args.Data, readWrite);
        }

        private void DataStoreOnDataStoreWrittenTo(object sender, DataStoreEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                //Add it to the event log
                ActivityLog.Add(CreateLog(args, ReadWrite.Write));

                switch (args.ModbusDataType)
                {
                    case ModbusDataType.InputRegister:

                        for (ushort registerIndex = args.StartAddress;
                            registerIndex < args.StartAddress + args.Data.B.Count;
                            registerIndex++)
                        {
                            _inputRegisters[registerIndex].OnValueUpdated();
                        }

                        DispatcherHelper.CheckBeginInvokeOnUI(() => _inputView.Refresh());

                        break;

                    case ModbusDataType.HoldingRegister:

                        for (ushort registerIndex = args.StartAddress;
                            registerIndex < args.StartAddress + args.Data.B.Count;
                            registerIndex++)
                        {
                            _holdingRegisters[registerIndex].OnValueUpdated();
                        }

                        DispatcherHelper.CheckBeginInvokeOnUI(() => _holdingView.Refresh());

                        break;
                }

            });
        }

        private void DataStoreOnDataStoreReadFrom(object sender, DataStoreEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => ActivityLog.Add(CreateLog(args, ReadWrite.Read)));

            switch (args.ModbusDataType)
            {
                case ModbusDataType.InputRegister:

                    for (ushort registerIndex = args.StartAddress;
                           registerIndex < args.StartAddress + args.Data.B.Count;
                           registerIndex++)
                    {
                        _inputRegisters[registerIndex].HasBeenTouched = true;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() => _inputView.Refresh());

                    break;

                case ModbusDataType.HoldingRegister:

                    for (ushort registerIndex = args.StartAddress;
                           registerIndex < args.StartAddress + args.Data.B.Count;
                           registerIndex++)
                    {
                        _holdingRegisters[registerIndex].HasBeenTouched = true;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() => _holdingView.Refresh());

                    break;
            }
        }

        public ObservableCollection<ActivityLogViewModel> ActivityLog
        {
            get { return _activityLog; }
        }

        public ICollectionView HoldingRegisters
        {
            get { return _holdingView; }
        }

        public ICollectionView InputRegisters
        {
            get { return _inputView; }
        }

        private string[] _portNames;
        public string[] PortNames
        {
            get { return _portNames; }
            private set
            {
                _portNames = value; 
                RaisePropertyChanged();
            }
        }

        private string _portName;
        public string PortName
        {
            get { return _portName; }
            set
            {
                _portName = value; 
                RaisePropertyChanged();
            }
        }

        private bool _onlyShowTouched;
        public bool OnlyShowTouched
        {
            get { return _onlyShowTouched; }
            set
            {
                _onlyShowTouched = value; 
                RaisePropertyChanged();

                _holdingView.Refresh();
                _inputView.Refresh();
            }
        }
    }
}
