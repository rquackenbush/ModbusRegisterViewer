using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        private byte _slaveId = 1;
        private readonly ObservableCollection<RangeViewModelBase> _ranges = new ObservableCollection<RangeViewModelBase>();
        private string _name;

        public SlaveViewModel(SlaveModel slaveModel)
        {
            if (slaveModel == null) throw new ArgumentNullException("slaveModel");

            Name = slaveModel.Name;
            SlaveId = slaveModel.SlaveId;


            AddRegistersCommand = new RelayCommand(AddRegisters, CanAdd);
            //AddInputRegistersCommand = new RelayCommand(AddInputRegisters, CanAdd);
            //AddCoilsCommand = new RelayCommand(AddCoils, CanAdd);
            //AddDiscretesCommand = new RelayCommand(AddDiscrete, CanAdd);

            if (slaveModel.Ranges != null)
            {
                foreach (var range in slaveModel.Ranges)
                {
                    var rangeViewModel = new RegisterRangeViewModel(range);

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
        //public ICommand AddInputRegistersCommand { get; private set; }
        //public ICommand AddCoilsCommand { get; private set; }
        //public ICommand AddDiscretesCommand { get; private set; }

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

                var rangeViewModel = new RegisterRangeViewModel(updatedRangeModel);

                _ranges.Add(rangeViewModel);
            }
        }

        public string Name
        {
            get { return _name; }
            set
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
