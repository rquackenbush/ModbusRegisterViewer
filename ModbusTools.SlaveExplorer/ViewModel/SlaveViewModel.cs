using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        private byte _slaveAddress = 1;
        private readonly ObservableCollection<RangeViewModelBase> _ranges = new ObservableCollection<RangeViewModelBase>();

        private int _newRangeNumber = 1;

        public SlaveViewModel()
        {
            AddHoldingRegistersCommand = new RelayCommand(AddHoldingRegisters, CanAdd);
            AddInputRegistersCommand = new RelayCommand(AddInputRegisters, CanAdd);
            AddCoilsCommand = new RelayCommand(AddCoils, CanAdd);
            AddDiscretesCommand = new RelayCommand(AddDiscrete, CanAdd);
        }

        public byte SlaveAddress
        {
            get { return _slaveAddress; }
            set
            {
                _slaveAddress = value; 
                RaisePropertyChanged();
            }
        }

        public ICommand AddHoldingRegistersCommand { get; private set; }
        public ICommand AddInputRegistersCommand { get; private set; }
        public ICommand AddCoilsCommand { get; private set; }
        public ICommand AddDiscretesCommand { get; private set; }

        private bool CanAdd()
        {
            return true;
        }

        private string CreateNewRangeName()
        {
            return string.Format("Range {0}", _newRangeNumber++);
        }

        private void AddHoldingRegisters()
        {
            _ranges.Add(new RegisterRangeViewModel()
            {
                Name = CreateNewRangeName()
            });
        }

        private void AddInputRegisters()
        {
            
        }

        private void AddCoils()
        {
            
        }

        private void AddDiscrete()
        {
            
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
