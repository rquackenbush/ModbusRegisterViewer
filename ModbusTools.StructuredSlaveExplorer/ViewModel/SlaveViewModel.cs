using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.StructuredSlaveExplorer.Interfaces;
using ModbusTools.StructuredSlaveExplorer.Model;
using ModbusTools.StructuredSlaveExplorer.View;

namespace ModbusTools.StructuredSlaveExplorer.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        private readonly IModbusAdapterProvider _modbusAdapterProvider;
        private readonly IDirty _dirty;
        private byte _slaveId = 1;
        private readonly ObservableCollection<RangeViewModelBase> _ranges = new ObservableCollection<RangeViewModelBase>();
        private string _name;

        public SlaveViewModel(IModbusAdapterProvider modbusAdapterProvider, SlaveModel slaveModel, IDirty dirty)
        {
            _modbusAdapterProvider = modbusAdapterProvider;
            _dirty = dirty;
            if (slaveModel == null) throw new ArgumentNullException(nameof(slaveModel));

            Name = slaveModel.Name;
            SlaveId = slaveModel.SlaveId;

            AddRegistersCommand = new RelayCommand(AddRegisters, CanAdd);

            if (slaveModel.Ranges != null)
            {
                foreach (var range in slaveModel.Ranges)
                {
                    var rangeViewModel = new RegisterRangeViewModel(modbusAdapterProvider, range, this, _dirty);

                    _ranges.Add(rangeViewModel);
                }
            }

            RenameCommand = new RelayCommand(Rename, CanRename);
        }

        public ICommand RenameCommand { get; private set; }

        private void Rename()
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Name", "Modbus Slave Name", Name, -1, -1);

            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
            }
        }

        private bool CanRename()
        {
            return true;
        }

        internal SlaveModel ToModel()
        {
            return new SlaveModel()
            {
                Name = Name,
                SlaveId = SlaveId,
                Ranges = Ranges.Select(r => r.GetModel()).ToArray()
            };
        }

        public string DisplayName
        {
            get { return string.Format("{0} [{1}]", Name, SlaveId); }
        }

        public byte SlaveId
        {
            get { return _slaveId; }
            set
            {
                _slaveId = value; 
                RaisePropertyChanged();
                _dirty.MarkDirtySafe();
                RaisePropertyChanged(() => DisplayName);
            }
        }

        public ICommand AddRegistersCommand { get; private set; }

        private bool CanAdd()
        {
            return true;
        }

        private string CreateNewRangeName()
        {
            return _ranges.Select(r => r.Name).CreateUnique("Range {0}");
        }

        private void AddRegisters()
        {
            var rangeModel = new RangeModel()
            {
                Name = CreateNewRangeName(),
                StartIndex = 1,
                RegisterType = RegisterType.Holding,
                IsExpanded = true
            };

            var rangeEditorViewModel = new RegisterRangeEditorViewModel(rangeModel);

            var dialog = new RegisterRangeEditorView()
            {
                DataContext = rangeEditorViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var updatedRangeModel = rangeEditorViewModel.GetModel();

                var rangeViewModel = new RegisterRangeViewModel(_modbusAdapterProvider, updatedRangeModel, this, _dirty);

                _ranges.Add(rangeViewModel);

                _dirty.MarkDirtySafe();
            }
        }

        internal void RemoveRange(RangeViewModelBase range)
        {
            if (_ranges.Contains(range))
            {
                _ranges.Remove(range);

                _dirty.MarkDirtySafe();
            }
        }

        public string Name
        {
            get { return _name; }
            private set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                    _dirty.MarkDirtySafe();
                    RaisePropertyChanged(() => DisplayName);
                }
            }
        }

        public IEnumerable<RangeViewModelBase> Ranges
        {
            get { return _ranges; }
        }

        internal void AddRange(RangeViewModelBase range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            _ranges.Add(range);
        }
    }
}
