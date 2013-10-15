using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FtdAdapter;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Modbus.Data;
using Modbus.Device;
using Xceed.Wpf.DataGrid.FilterCriteria;

namespace ModbusRegisterViewer.ViewModel
{
    public class SlaveSimulatorViewModel : ViewModelBase
    {
        private readonly ObservableCollection<AdapterViewModel> _adapters = new ObservableCollection<AdapterViewModel>();
        private AdapterViewModel _selectedAdapter;
        private FtdUsbPort _port;
        private ModbusSerialSlave _slave;
        private int? _slaveAddress;
        private readonly ObservableCollection<ActivityLogViewModel>  _activityLog = new ObservableCollection<ActivityLogViewModel>();

        public SlaveSimulatorViewModel()
        {
            RefreshAdapters();

            this.StartCommand = new RelayCommand(Start, CanStart);
            this.StopCommand = new RelayCommand(Stop, CanStop);
            this.RefreshAdaptersCommand = new RelayCommand(RefreshAdapters);
            this.ClearCommand = new RelayCommand(Clear, CanClear);

            var settings = Properties.Settings.Default;

            this.SlaveAddress = settings.SlaveSimulatorSlaveAddress;
            SelectAdapter(settings.SlaveSimulatorAdapterSerialNumber);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand RefreshAdaptersCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        private void Clear()
        {
            this.ActivityLog.Clear();
        }

        private bool CanClear()
        {
            return this.ActivityLog.Count > 0;
        }

        private void Start()
        {
            var settings = Properties.Settings.Default;

            settings.SlaveSimulatorSlaveAddress = this.SlaveAddress ?? 0;
            settings.SlaveSimulatorAdapterSerialNumber = this.SelectedAdapter == null
                ? ""
                : this.SelectedAdapter.SerialNumber;

            settings.Save();

            _port = new FtdUsbPort();

            // configure serial port
            _port.BaudRate = 19200;
            _port.DataBits = 8;
            _port.Parity = FtdParity.Even;
            _port.StopBits = FtdStopBits.One;

            _port.OpenBySerialNumber(this.SelectedAdapter.SerialNumber);

            _port.ReadTimeout = 2000;
            _port.WriteTimeout = 2000;

            var slaveAddresss = (byte) this.SlaveAddress;

            _slave = ModbusSerialSlave.CreateRtu(slaveAddresss, _port);

            _slave.DataStore = DataStoreFactory.CreateDefaultDataStore();

            //slave.ModbusSlaveRequestReceived += SlaveRequestReceived;
            _slave.DataStore.DataStoreReadFrom += DataStoreOnDataStoreReadFrom;
            _slave.DataStore.DataStoreWrittenTo += DataStoreOnDataStoreWrittenTo;

            var task = new Task(() =>
            {
                _slave.Listen();
            });

            task.Start();              
        }

        private bool CanStart()
        {
            return _port == null && this.SelectedAdapter != null && this.SlaveAddress.HasValue;
        }

        private void Stop()
        {
            _port.ReadTimeout = 1;
            _port.WriteTimeout = 1;
            _port.Dispose();
            _slave.Dispose();

            _port = null;
            _slave = null;
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
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.ActivityLog.Add(CreateLog(args, ReadWrite.Write)));
        }

        private void DataStoreOnDataStoreReadFrom(object sender, DataStoreEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.ActivityLog.Add(CreateLog(args, ReadWrite.Read)));
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

        public ObservableCollection<AdapterViewModel> Adapters
        {
            get { return _adapters; }
        }

        public ObservableCollection<ActivityLogViewModel> ActivityLog
        {
            get { return _activityLog; }
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

    }
}
