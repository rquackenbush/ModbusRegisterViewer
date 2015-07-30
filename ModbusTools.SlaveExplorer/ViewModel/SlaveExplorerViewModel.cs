using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class SlaveExplorerViewModel : ViewModelBase
    {
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly ObservableCollection<SlaveViewModel> _slaves = new ObservableCollection<SlaveViewModel>();

        public SlaveExplorerViewModel()
        {
            CreateNewSlaveCommand = new RelayCommand(CreateNewSlave, CanCreateNewSlave);
        }

        public ICommand CreateNewSlaveCommand { get; private set; }

        private void CreateNewSlave()
        {
            var slave = new SlaveViewModel();

            _slaves.Add(slave);
        }

        private bool CanCreateNewSlave()
        {
            return true;
        }

        public IEnumerable<SlaveViewModel> Slaves
        {
            get { return _slaves; }
        }

        public void RemoveSlave(SlaveViewModel slave)
        {
            if (slave == null) 
                throw new ArgumentNullException("slave");

            _slaves.Remove(slave);
        }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }
    }
}
