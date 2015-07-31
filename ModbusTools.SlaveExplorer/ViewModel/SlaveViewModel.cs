using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Interfaces;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
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
            if (slaveModel == null) throw new ArgumentNullException("slaveModel");

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
        }

        internal SlaveModel GetModel()
        {
            return new SlaveModel()
            {
                Name = Name,
                SlaveId = SlaveId,
                Ranges = Ranges.Select(r => r.GetModel()).ToArray()
            };
        }

        public byte SlaveId
        {
            get { return _slaveId; }
            set
            {
                _slaveId = value; 
                RaisePropertyChanged();
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
                RegisterType = RegisterType.Holding
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
                _name = value; 
                RaisePropertyChanged();
            }
        }

        public IEnumerable<RangeViewModelBase> Ranges
        {
            get { return _ranges; }
        }

        internal void AddRange(RangeViewModelBase range)
        {
            if (range == null) throw new ArgumentNullException("range");

            _ranges.Add(range);
        }
    }
}
