using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Modbus.Data;
using Modbus.Device;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;

namespace ModbusRegisterViewer.ViewModel.SlaveSimulator
{
    public class SlaveSimulatorViewModel : ViewModelBase
    {
        //private readonly ModbusAdaptersViewModel _adapters = new ModbusAdaptersViewModel();
        //private IModbusAdapterFactory _selectedAdapter;
        ////private FtdUsbPort _port;
        //private ModbusSerialSlave _slave;
        //private int? _slaveAddress;
        //private readonly ObservableCollection<ActivityLogViewModel>  _activityLog = new ObservableCollection<ActivityLogViewModel>();

        //private readonly DataStore _dataStore;
        //private List<RegisterViewModel> _holdingRegisters;
        //private List<RegisterViewModel> _inputRegisters;

        //public SlaveSimulatorViewModel()
        //{
        //    this.StartCommand = new RelayCommand(Start, CanStart);
        //    this.StopCommand = new RelayCommand(Stop, CanStop);
        //    this.ClearCommand = new RelayCommand(Clear, CanClear);

        //    var settings = Properties.Settings.Default;

        //    this.SlaveAddress = settings.SlaveSimulatorSlaveAddress;
        //    SelectAdapter(settings.SlaveSimulatorAdapterSerialNumber);

        //    //Create the data store
        //    _dataStore = DataStoreFactory.CreateDefaultDataStore();

        //    //Here are the listeners
        //    _dataStore.DataStoreReadFrom += DataStoreOnDataStoreReadFrom;
        //    _dataStore.DataStoreWrittenTo += DataStoreOnDataStoreWrittenTo;

        //    //Set up the holding registers
        //    {
        //        ushort registerNumber = 0;

        //        this.HoldingRegisters =
        //            _dataStore.HoldingRegisters.Select(
        //                r => new RegisterViewModel(_dataStore.HoldingRegisters, registerNumber++)).ToList();
        //    }

        //    //Set up the input registers
        //    {
        //        ushort registerNumber = 0;

        //        this.InputRegisters =
        //            _dataStore.InputRegisters.Select(
        //                r => new RegisterViewModel(_dataStore.InputRegisters, registerNumber++)).ToList();
        //    }
        //}

        //public ICommand StartCommand { get; private set; }
        //public ICommand StopCommand { get; private set; }
        //public ICommand RefreshAdaptersCommand { get; private set; }
        //public ICommand ClearCommand { get; private set; }

        //private void Clear()
        //{
        //    this.ActivityLog.Clear();
        //}

        //private bool CanClear()
        //{
        //    return this.ActivityLog.Count > 0;
        //}

        //private void Start()
        //{
        //    var settings = Properties.Settings.Default;

        //    settings.SlaveSimulatorSlaveAddress = this.SlaveAddress ?? 0;
        //    settings.SlaveSimulatorAdapterSerialNumber = this.SelectedAdapter == null
        //        ? ""
        //        : this.SelectedAdapter.DisplayName;

        //    settings.Save();

        //    var factory = _adapters.GetFactory();


        //    IMasterContext context = factory.Create();



        //    ModbusSerialSlave.CreateRtu(SlaveAddress, context.Master.Transport.)

        //    _port = new FtdUsbPort();

        //    // configure serial port
        //    _port.BaudRate = 19200;
        //    _port.DataBits = 8;
        //    _port.Parity = FtdParity.Even;
        //    _port.StopBits = FtdStopBits.One;

        //    _port.OpenBySerialNumber(this.SelectedAdapter.SerialNumber);

        //    _port.ReadTimeout = 2000;
        //    _port.WriteTimeout = 2000;

        //    var slaveAddresss = (byte) this.SlaveAddress;

        //    _slave = ModbusSerialSlave.CreateRtu(slaveAddresss, _port);

        //    _slave.DataStore = _dataStore;

        //    var task = new Task(() =>
        //    {
        //        try
        //        {
        //            _slave.Listen();
        //        }
        //        catch (IOException)
        //        {
        //            //We'll just ignore this.
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //        }

        //        Stop();

        //        DispatcherHelper.CheckBeginInvokeOnUI(CommandManager.InvalidateRequerySuggested);
                   
        //    });

        //    task.Start();
        // }

        //private bool CanStart()
        //{
        //    return _port == null && this.SelectedAdapter != null && this.SlaveAddress.HasValue;
        //}

        //private void Stop()
        //{
        //    if (_port != null)
        //    {
        //        _port.ReadTimeout = 1;
        //        _port.WriteTimeout = 1;
        //        _port.Dispose();

        //        _port = null;
        //    }

        //    if (_slave != null)
        //    {
        //        _slave.Dispose();

        //        _slave = null;
        //    }
        //}

        //public int? SlaveAddress
        //{
        //    get { return _slaveAddress; }
        //    set
        //    {
        //        _slaveAddress = value;
        //        RaisePropertyChanged(() => SlaveAddress);
        //    }
        //}

        //private bool CanStop()
        //{
        //    return _port != null;
        //}

        //private static ActivityLogViewModel CreateLog(DataStoreEventArgs args, ReadWrite readWrite)
        //{
        //    return new ActivityLogViewModel(DateTime.Now, args.ModbusDataType, args.StartAddress, args.Data, readWrite);
        //}

        //private void DataStoreOnDataStoreWrittenTo(object sender, DataStoreEventArgs args)
        //{
        //    DispatcherHelper.CheckBeginInvokeOnUI(() =>
        //    {
        //        //Add it to the event log
        //        this.ActivityLog.Add(CreateLog(args, ReadWrite.Write));

        //        switch (args.ModbusDataType)
        //        {
        //            case ModbusDataType.InputRegister:

        //                for (ushort registerIndex = args.StartAddress;
        //                    registerIndex < args.StartAddress + args.Data.B.Count;
        //                    registerIndex++)
        //                {
        //                    this.InputRegisters[registerIndex].OnValueUpdated();
        //                }

        //                break;

        //            case ModbusDataType.HoldingRegister:

        //                for (ushort registerIndex = args.StartAddress;
        //                    registerIndex < args.StartAddress + args.Data.B.Count;
        //                    registerIndex++)
        //                {
        //                    this.HoldingRegisters[registerIndex].OnValueUpdated();
        //                }

        //                break;
        //        }

        //    });
        //}

        //private void DataStoreOnDataStoreReadFrom(object sender, DataStoreEventArgs args)
        //{
        //    DispatcherHelper.CheckBeginInvokeOnUI(() => this.ActivityLog.Add(CreateLog(args, ReadWrite.Read)));
        //}


        ///// <summary>
        ///// Selects an adapter.
        ///// </summary>
        ///// <param name="name"></param>
        //private void SelectAdapter(string name)
        //{
        //    var initialAdapter = this.Adapters.Adapters.FirstOrDefault(a => a.DisplayName == name);

        //    if (initialAdapter == null)
        //        initialAdapter = this.Adapters.Adapters.FirstOrDefault();

        //    this.SelectedAdapter = initialAdapter;
        //}

        //public ModbusAdaptersViewModel Adapters
        //{
        //    get { return _adapters; }
        //}

        //public ObservableCollection<ActivityLogViewModel> ActivityLog
        //{
        //    get { return _activityLog; }
        //}

        //public List<RegisterViewModel> HoldingRegisters
        //{
        //    get { return _holdingRegisters; }
        //    private set
        //    {
        //        _holdingRegisters = value;
        //        RaisePropertyChanged(() => HoldingRegisters);
        //    }
        //}

        //public List<RegisterViewModel> InputRegisters
        //{
        //    get { return _inputRegisters; }
        //    private set
        //    {
        //        _inputRegisters = value;
        //        RaisePropertyChanged(() => InputRegisters);
        //    }
        //}


        //public IModbusAdapterFactory SelectedAdapter
        //{
        //    get { return _selectedAdapter; }
        //    set
        //    {
        //        _selectedAdapter = value;
        //        RaisePropertyChanged(() => SelectedAdapter);
        //    }
        //}

    }
}
