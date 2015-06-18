using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common.Model;

namespace ModbusTools.Common.ViewModel
{
    public class ModbusAdaptersViewModel : ViewModelBase, IModbusAdapterProvider
    {
         private readonly ObservableCollection<string> _ports = new ObservableCollection<string>();

         private readonly ObservableCollection<Parity> _parities = new ObservableCollection<Parity>()
        {
            Parity.None,
            Parity.Odd,
            Parity.Even,
            Parity.Mark,
            Parity.Space
        };

         private readonly ObservableCollection<KeyValuePair<StopBits, string>> _stopBitChoices = new ObservableCollection<KeyValuePair<StopBits, string>>()
        {
            new KeyValuePair<StopBits, string>( StopBits.None, "N"),
            new KeyValuePair<StopBits, string>( StopBits.One, "1"),
            new KeyValuePair<StopBits, string>( StopBits.OnePointFive, "1.5"),
            new KeyValuePair<StopBits, string>( StopBits.Two, "2"),
        };

         public ModbusAdaptersViewModel()
        {
            this.RefreshCommand = new RelayCommand(RefreshPorts);

            RefreshPorts();
        }

        public ICommand RefreshCommand { get; private set; }

        public IEnumerable<Parity> Parities
        {
            get { return _parities; }
        }

        public IEnumerable<KeyValuePair<StopBits, string>> StopBitChoices
        {
            get { return _stopBitChoices; }
        }

        private void RefreshPorts()
        {
            var selectedDisplayName = SelectedPort;

            _ports.Clear();

            SelectedPort = null;

            //Get the comm ports
            var serialPortNames = SerialPort.GetPortNames();

            foreach (var serialPortName in serialPortNames)
            {
                _ports.Add(serialPortName);
            }

            var portToSelect = _ports.FirstOrDefault(p => string.Compare(selectedDisplayName, p, true) == 0);

            if (portToSelect == null)
                portToSelect = _ports.FirstOrDefault();

            this.SelectedPort = portToSelect;
        }

        public ObservableCollection<string> Ports
        {
            get { return _ports; }
        }

        private string _selectedPort;
        public string SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                _selectedPort = value;
                RaisePropertyChanged(() => SelectedPort);
            }
        }

        ///// <summary>
        ///// Gets or sets the selected display name. If the value provided is not found during execution of the setter, it is ignored.
        ///// </summary>
        //public string SelectedDisplayName
        //{
        //    get
        //    {
        //        var selectedAdapter = this.SelectedPort;

        //        if (selectedAdapter == null)
        //            return null;

        //        return selectedAdapter.DisplayName;
        //    }
        //    set
        //    {
        //        var adapterToSelect = _ports.FirstOrDefault(a => string.Compare(value, a.DisplayName, true) == 0);

        //        if (adapterToSelect != null)
        //        {
        //            this.SelectedPort = adapterToSelect;
        //        }
        //    }
        //}

        private int _baud = 19200;
        public int Baud
        {
            get { return _baud; }
            set
            {
                _baud = value;
                RaisePropertyChanged();
            }
        }

        private int _dataBits = 8;
        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                _dataBits = value;
                RaisePropertyChanged();
            }
        }

        private Parity _parity = Parity.Even;
        public Parity Parity
        {
            get { return _parity; }
            set
            {
                _parity = value;
                RaisePropertyChanged();
            }
        }

        private StopBits _stopBits = StopBits.One;
        public StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
                RaisePropertyChanged();
            }
        }

        private double _readTimeout = 1.0;
        public double ReadTimeout
        {
            get { return _readTimeout; }
            set
            {
                _readTimeout = value;
                RaisePropertyChanged();
            }
        }

        private double _writeTimeout = 1.0;
        public double WriteTimeout
        {
            get { return _writeTimeout; }
            set
            {
                _writeTimeout = value;
                RaisePropertyChanged();
            }
        }

        #region IModbusAdapterFactoryProvider members

        public bool IsItemSelected
        {
            get { return this.SelectedPort != null; }
        }

        public IMasterContextFactory GetFactory()
        {
            if (this.SelectedPort == null)
                throw new Exception("No adapter is selected.");

            return new SerialMasterContextFactory(SelectedPort, Baud, DataBits, Parity, StopBits,(int)(ReadTimeout * 1000), (int)(WriteTimeout * 1000));
        }

        #endregion

    }

    
}
