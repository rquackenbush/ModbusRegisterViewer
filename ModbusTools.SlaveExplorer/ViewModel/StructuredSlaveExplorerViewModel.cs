using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class StructuredSlaveExplorerViewModel : ViewModelBase, ICloseableViewModel
    {
        private string _path;

        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();

        private const string Filter = "Modbus Project Files (*.mbproj)|*.mbproj|All Files (*.*)|*.*";

        private readonly Dirty _dirty = new Dirty();
        private SlaveViewModel _slave;

        public StructuredSlaveExplorerViewModel()
        {
            ExitCommand = new RelayCommand(Exit, CanExit);
            SaveCommand = new RelayCommand(() => Save());
            SaveAsCommand = new RelayCommand(() => SaveAs());
            OpenCommand = new RelayCommand(Open);
            NewCommand = new RelayCommand(New, CanNew);

            //Create a new slave
            New();
        }

        public ICommand ExitCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        public ICommand NewCommand { get; private set; }

        private void New()
        {
            if (!SaveIfDirty())
                return;

            Slave = new SlaveViewModel(_modbusAdapters, new SlaveModel(), _dirty);

            _dirty.MarkClean();
        }

        private bool CanNew()
        {
            return true;
        }

        private bool SaveIfDirty()
        {
            //Easy case - nothing is dirty
            if (!_dirty.IsDirty)
                return true;

            var message = "Project has changed. Save?";

            var result = MessageBox.Show(message, "Save?", MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return Save();

                case MessageBoxResult.No:
                    return true;

                case MessageBoxResult.Cancel:
                    return false;

                default:
                    MessageBox.Show("That was odd.");
                    return false;
            }
        }

        private bool Save()
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                return SaveAs();
            }

            //Create the project model!
            var projectModel = new ProjectModel()
            {
                Slaves = new SlaveModel[]
                {
                    Slave.ToModel()
                }
            };

            //Save it!
            ProjectFactory.SaveProject(projectModel, _path);

            //We're clean
            _dirty.MarkClean();

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
            if (!SaveIfDirty())
                return;

            var dialog = new OpenFileDialog()
            {
                Filter = Filter
            };

            if (dialog.ShowDialog() == true)
            {
                _path = dialog.FileName;

                //Load up the project
                var projectModel = ProjectFactory.LoadProject(_path);

                //Get the slave (we're only going to grab the first one
                var slaveModel = projectModel?.Slaves?.FirstOrDefault() ?? new SlaveModel();

                //Create the view model
                Slave = new SlaveViewModel(_modbusAdapters, slaveModel, _dirty);

                //We're clean people.
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

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public string Title
        {
            get
            {
                const string AppName = "Structured Slave Explorer";

                if (string.IsNullOrWhiteSpace(_path))
                {
                    return AppName;
                }

                return $"{AppName} [{_path}]";
            }
        }

        public SlaveViewModel Slave
        {
            get { return _slave; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _slave = value; 
                RaisePropertyChanged();
            }
        }

        #region ICloseableViewModel

        public event EventHandler<CloseEventArgs> Close;

        public bool CanClose()
        {
            return SaveIfDirty();
        }

        public void Closed()
        {
        }

        #endregion
    }
}
