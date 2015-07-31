﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class SlaveExplorerViewModel : ViewModelBase, ICloseableViewModel
    {
        private string _path;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly ObservableCollection<SlaveViewModel> _slaves = new ObservableCollection<SlaveViewModel>();

        public EventHandler<SlaveViewModel> SlaveAdded;
        public EventHandler<SlaveViewModel> SlaveRemoved;

        private const string Filter = "Modbus Project Files (*.mbproj)|*.mbproj|All Files (*.*)|*.*";

        private readonly Dirty _dirty = new Dirty();

        public SlaveExplorerViewModel()
        {
            CreateNewSlaveCommand = new RelayCommand(CreateNewSlave, CanCreateNewSlave);
            ExitCommand = new RelayCommand(Exit, CanExit);
            SaveCommand = new RelayCommand(() => Save());
            SaveAsCommand = new RelayCommand(() => SaveAs());
            OpenCommand = new RelayCommand(Open);
        }

        public ICommand ExitCommand { get; private set; }
        public ICommand CreateNewSlaveCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        private bool Save()
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                return SaveAs();
            }
            
            var projectModel = new ProjectModel()
            {
                Slaves = Slaves.Select(s => s.GetModel()).ToArray()
            };

            ProjectFactory.SaveProject(projectModel, _path);

            return true;
        }

        private bool SaveAs()
        {
            var dialog = new SaveFileDialog()
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() == true)
            {
                _path = dialog.FileName;

                RaisePropertyChanged(() => Title);

                return Save();
            }

            return false;
        }

        private void Open()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() == true)
            {
                _path = dialog.FileName;

                //Remove the slaves
                foreach (var slave in _slaves.ToArray())
                {
                    RemoveSlave(slave);
                }

                //Load up the project
                var projectModel = ProjectFactory.LoadProject(_path);

                //Now add all of the slaves
                foreach (var slave in projectModel.Slaves)
                {
                    var slaveViewModel = new SlaveViewModel(_modbusAdapters, slave);

                    _slaves.Add(slaveViewModel);

                    SlaveAdded.RaiseEvent(slaveViewModel);
                }

                _dirty.MarkClean();
            }

            RaisePropertyChanged(() => Title);
        }

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

            var slave = new SlaveViewModel(_modbusAdapters, slaveModel);

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

            if (!_slaves.Contains(slave))
                return;

            _slaves.Remove(slave);

            SlaveRemoved.RaiseEvent(slave);
        }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public string Title
        {
            get
            {
                const string AppName = "Slave Explorer";

                if (string.IsNullOrWhiteSpace(_path))
                {
                    return AppName;
                }

                return string.Format("{0} [{1}]", AppName, _path);
            }
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
