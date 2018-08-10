using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using NModbus;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    using System.IO;
    using System.Windows;
    using Microsoft.Win32;
    using ModbusTools.SlaveSimulator.Persistence;
    using Newtonsoft.Json;

    public class SlaveSimulatorViewModel : ViewModelBase
    {
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private readonly ObservableCollection<SlaveViewModel> _slaves = new ObservableCollection<SlaveViewModel>();

        private IModbusSlaveNetwork _slaveNetwork;

        //We have to hold a reference to the running listen task or it will get garbage collected.
        private Task _listenTask;

        private const string Filter = "Slave Simulator (.slavesim)|*.slavesim";

        private string _filename;
        
        public SlaveSimulatorViewModel()
        {
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            AddSlaveCommand = new RelayCommand(AddSlave, CanAddSlave);
            SaveCommand = new RelayCommand(Save, CanSave);
            SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            OpenCommand = new RelayCommand(Open, CanLoad);
            AddSlave();
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand AddSlaveCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand OpenCommand {  get;}

        private void Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_filename))
                {
                    SaveAs();
                }
                else
                {
                    var model = ToModel();

                    string json = JsonConvert.SerializeObject(model, Formatting.Indented);

                    File.WriteAllText(_filename, json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void SaveAs()
        {
            try
            {
                var saveDialog = new SaveFileDialog()
                {
                    Filter = Filter
                };

                if (saveDialog.ShowDialog() == true)
                {
                    Filename = saveDialog.FileName;

                    Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private bool CanSaveAs()
        {
            return _slaveNetwork == null;
        }

        private bool CanSave()
        {
            return _slaveNetwork == null;
        }

        private SimulatorProject ToModel()
        {
            return new SimulatorProject()
            {
                CommPort = ModbusAdapters.SelectedPort,
                ReadTimeout = ModbusAdapters.ReadTimeout,
                WriteTimeout = ModbusAdapters.WriteTimeout,
                Slaves = Slaves
                    .Select(s => s.ToModel())
                    .ToArray()
            };
        }

        private string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                RaisePropertyChanged(() => Title);
            }
        }

        private void Open()
        {
            try
            {
                var openDialog = new OpenFileDialog()
                {
                    Filter = Filter
                };

                if (openDialog.ShowDialog() == true)
                {
                    Filename = openDialog.FileName;

                    string json = File.ReadAllText(openDialog.FileName);

                    var model = JsonConvert.DeserializeObject<SimulatorProject>(json);

                    if (model.Slaves == null)
                        model.Slaves = new Slave[]{};

                    foreach (var slave in model.Slaves)
                    {
                        if (slave.HoldingRegisters == null)
                            slave.HoldingRegisters = new Point<ushort>[0];

                        if (slave.InputRegisters == null)
                            slave.InputRegisters = new Point<ushort>[0];

                        if (slave.Inputs == null)
                            slave.Inputs = new Point<bool>[0];

                        if (slave.Discretes == null)
                            slave.Discretes = new Point<bool>[0];
                    }
                    
                    //Ditch the current slaves
                    Slaves.Clear();

                    foreach (var slaveModel in model.Slaves)
                    {
                        var slave = new SlaveViewModel()
                        {
                            SlaveId = slaveModel.SlaveId
                        };

                        foreach (var point in slaveModel.HoldingRegisters)
                        {
                            slave.HoldingRegisters[point.Address].Value = point.Value;
                        }

                        foreach (var point in slaveModel.InputRegisters)
                        {
                            slave.InputRegisters[point.Address].Value = point.Value;
                        }

                        foreach (var point in slaveModel.Inputs)
                        {
                            slave.CoilInputs[point.Address].Value = point.Value;
                        }

                        foreach (var point in slaveModel.Discretes)
                        {
                            slave.CoilDiscretes[point.Address].Value = point.Value;
                        }

                        Slaves.Add(slave);
                    }

                    if (!string.IsNullOrWhiteSpace(model.CommPort))
                    {
                        if (ModbusAdapters.Ports.Contains(model.CommPort))
                        {
                            ModbusAdapters.SelectedPort = model.CommPort;
                        }
                    }

                    if (model.ReadTimeout != null)
                    {
                        ModbusAdapters.ReadTimeout = model.ReadTimeout.Value;
                    }

                    if (model.WriteTimeout != null)
                    {
                        ModbusAdapters.WriteTimeout = model.WriteTimeout.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private bool CanLoad()
        {
            return _slaveNetwork == null;
        }

        private void AddSlave()
        {
            byte slaveId = 1;

            //Check to see if we already have a slave
            if (_slaves.Any())
            {
                var max = _slaves.Max(s => s.SlaveId);

                slaveId = (byte)(max + 1);
            }

            //Create the new slave
            var slave = new SlaveViewModel()
            {
                SlaveId = slaveId
            };

            //Add it
            _slaves.Add(slave);
        }

        private bool CanAddSlave()
        {
            //We can only add a slave when we're NOT running
            return _slaveNetwork == null;
        }

        private void Start()
        {
            IMasterContextFactory factory = _modbusAdapters.GetFactory();

            _slaveNetwork = factory.CreateSlaveNetwork();

            foreach (var slave in _slaves)
            {
                _slaveNetwork.AddSlave(slave.CreateModbusSlave());

                //Clear activity for each one.
                slave.ClearActivity();
            }

            _listenTask = Task.Factory.StartNew(async () => 
            {
                while (_slaveNetwork != null)
                {
                    try
                    {
                        var slaveNetwork = _slaveNetwork;

                        if (slaveNetwork != null)
                        {
                            await slaveNetwork.ListenAsync();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        //TODO: Log this

                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public string Title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_filename))
                    return "Slave Simulator";

                return $"Slave Simulator - {Path.GetFileName(_filename)}";
            }
        }

        public bool CanCloseSlave()
        {
            return _slaveNetwork == null;
        }

        public void OnSlaveClosed(SlaveViewModel slave)
        {
            if (Slaves.Contains(slave))
            {
                Slaves.Remove(slave);
            }
        }

        private bool CanStart()
        {
            if (_slaveNetwork != null)
                return false;

            if (!_modbusAdapters.IsItemSelected)
                return false;

            if (Slaves.Count == 0)
                return false;

            return true;
        }

        private void Stop()
        {
            if (_slaveNetwork != null)
            {
                _slaveNetwork.Dispose();

                _slaveNetwork = null;
            }

            _listenTask = null;
        }

        private bool CanStop()
        {
            return _slaveNetwork != null;
        }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public ObservableCollection<SlaveViewModel> Slaves
        {
            get { return _slaves; }
        }
    }
}
