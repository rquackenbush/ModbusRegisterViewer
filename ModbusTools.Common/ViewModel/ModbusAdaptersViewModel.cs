using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ModbusTools.Common.ViewModel
{
    public class ModbusAdaptersViewModel : ViewModelBase, IModbusAdapterProvider
    {
         private readonly ObservableCollection<IModbusAdapterFactory> _adapters = new ObservableCollection<IModbusAdapterFactory>();

         public ModbusAdaptersViewModel()
        {
            this.RefreshCommand = new RelayCommand(RefreshAdapters);

            RefreshAdapters();
        }

        public ICommand RefreshCommand { get; private set; }

        private void RefreshAdapters()
        {
            var selectedDisplayName = this.SelectedDisplayName;

            _adapters.Clear();
            this.SelectedAdapter = null;

            //Get the comm ports
            var serialPortNames = SerialPort.GetPortNames();

            foreach (var serialPortName in serialPortNames)
            {
                _adapters.Add(new SerialPortModbusAdapterFactory(serialPortName));
            }

            var adapterToSelect = _adapters.FirstOrDefault(a => string.Compare(selectedDisplayName, a.DisplayName, true) == 0);

            if (adapterToSelect == null)
                adapterToSelect = _adapters.FirstOrDefault();

            this.SelectedAdapter = adapterToSelect;
        }

        public ObservableCollection<IModbusAdapterFactory> Adapters
        {
            get { return _adapters; }
        }

        private IModbusAdapterFactory _selectedAdapter;
        public IModbusAdapterFactory SelectedAdapter
        {
            get { return _selectedAdapter; }
            set
            {
                _selectedAdapter = value;
                RaisePropertyChanged(() => SelectedAdapter);
            }
        }

        /// <summary>
        /// Gets or sets the selected display name. If the value provided is not found during execution of the setter, it is ignored.
        /// </summary>
        public string SelectedDisplayName
        {
            get
            {
                var selectedAdapter = this.SelectedAdapter;

                if (selectedAdapter == null)
                    return null;

                return selectedAdapter.DisplayName;
            }
            set
            {
                var adapterToSelect = _adapters.FirstOrDefault(a => string.Compare(value, a.DisplayName, true) == 0);

                if (adapterToSelect != null)
                {
                    this.SelectedAdapter = adapterToSelect;
                }
            }
        }

        #region IModbusAdapterFactoryProvider members

        public bool IsItemSelected
        {
            get { return this.SelectedAdapter != null; }
        }

        public IMasterContextFactory GetFactory()
        {
            if (this.SelectedAdapter == null)
                throw new Exception("No adapter is selected.");

            return _selectedAdapter.CreateMasterContext();
        }

        #endregion

    }

    
}
