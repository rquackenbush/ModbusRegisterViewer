using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class SlaveExplorerViewModel : ViewModelBase, ICloseableViewModel
    {
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly ObservableCollection<SlaveViewModel> _slaves = new ObservableCollection<SlaveViewModel>();

        public EventHandler<SlaveViewModel> SlaveAdded;
        

        public SlaveExplorerViewModel()
        {
            CreateNewSlaveCommand = new RelayCommand(CreateNewSlave, CanCreateNewSlave);
            ExitCommand = new RelayCommand(Exit, CanExit);
        }

        public ICommand ExitCommand { get; private set; }
        public ICommand CreateNewSlaveCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        private void Exit()
        {
            Close.RaiseEvent(new CloseEventArgs(true));
        }

        private bool CanExit()
        {
            return true;
        }

        private void CreateNewSlave()
        {
            var name = Slaves.Select(s => s.Name).CreateUnique("Modbus Slave {0}");

            var slaveModel = new SlaveModel()
            {
                SlaveId = 1,
                Name = name
            };

            var slave = new SlaveViewModel(slaveModel);

            _slaves.Add(slave);

            SlaveAdded.RaiseEvent(slave);
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

        #region ICloseableViewModel

        public event EventHandler<CloseEventArgs> Close;

        public bool CanClose()
        {
            return true;
        }

        public void Closed()
        {
        }

        #endregion
    }
}
