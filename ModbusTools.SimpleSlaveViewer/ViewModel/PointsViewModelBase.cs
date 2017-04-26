﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using NModbus;

namespace ModbusTools.SimpleSlaveExplorer.ViewModel
{
    public abstract class PointsViewModelBase<TPointViewModel, TPointValue> : ViewModelBase
        where TPointViewModel : class, IPointViewModel<TPointValue>, new()
    {
        private const byte DefaultBlockSize = 125;

        private readonly ISlaveExplorerContext _context;
        private ObservableCollection<TPointViewModel> _points = new ObservableCollection<TPointViewModel>();
        private ushort _startAddress;
        private ushort _numberOfPoints;
        private byte _blockSize = DefaultBlockSize;

        protected PointsViewModelBase(ISlaveExplorerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;

            ReadCommand = new RelayCommand(Read, CanRead);
            WriteCommand = new RelayCommand(Write, CanWrite);

            NumberOfPoints = 10;
        }

        public ICommand ReadCommand { get; }

        public ICommand WriteCommand { get; }

        private void Read()
        {
            try
            {
                IMasterContextFactory factory = _context.ModbusAdapterProvider.GetFactory();

                using (IModbusMaster master = factory.CreateMaster())
                {
                    TPointValue[] values = ReadCore(master, _context.SlaveId, StartAddress, NumberOfPoints);

                    for (int index = 0; index < values.Length; index++)
                    {
                        Points[index].SetValue(values[index]);
                    }
                }
            }
            catch (Exception ex)
            {
                _context.MessageBoxService.Show(ex, "Read Failed");
            }
        }

        protected abstract TPointValue[] ReadCore(IModbusMaster modbusMaster, byte slaveId, ushort startAddress, ushort numberOfPoints);

        protected abstract void WriteCore(IModbusMaster modbusMaster, byte slaveId, ushort startAddress, TPointValue[] values);

        public abstract bool IsWriteable { get; }

        public abstract bool SupportsBlockSize { get; }

        private void Write()
        {
            try
            {
                TPointValue[] values = Points
                .Select(p => p.Value)
                .ToArray();

                //Get the factory
                IMasterContextFactory factory = _context.ModbusAdapterProvider.GetFactory();

                //Perform the operation
                using (IModbusMaster master = factory.CreateMaster())
                {
                    WriteCore(master, _context.SlaveId, StartAddress, values);
                }

                //Mark the points as clean.
                foreach (var point in Points)
                {
                    point.IsDirty = false;
                }
            }
            catch (Exception ex)
            {
                _context.MessageBoxService.Show(ex, "Write Failed");
            }
        }

        private bool CanWrite()
        {
            return 
                _context.ModbusAdapterProvider.IsItemSelected 
                &&  NumberOfPoints > 0 
                && IsWriteable;
        }

        public virtual bool CanRead()
        {
            return
                _context.ModbusAdapterProvider.IsItemSelected
                && NumberOfPoints > 0;
        }

        private void FillInPoints()
        {
            Dictionary<ushort, TPointViewModel> existingValues;

            if (_points == null)
            {
                existingValues = new Dictionary<ushort, TPointViewModel>();
            }
            else
            {
                existingValues = _points.ToDictionary(r => r.Address, r => r);
            }

            //This is where the new reigsters will be
            var newRegisters = new ObservableCollection<TPointViewModel>();

            //Iterate through the new number of registers
            for (int index = 0; index < NumberOfPoints; index++)
            {
                TPointViewModel registerViewModel;

                var registerIndex = (ushort)(index + StartAddress);

                if (!existingValues.TryGetValue(registerIndex, out registerViewModel))
                {
                    registerViewModel = new TPointViewModel();

                    registerViewModel.Initialize(registerIndex, default(TPointValue));
                }

                newRegisters.Add(registerViewModel);
            }

            Points = newRegisters;
        }

        public byte BlockSize
        {
            get { return _blockSize; }
            set
            {
                _blockSize = value;
                RaisePropertyChanged(() => BlockSize);
            }
        }


        public ushort StartAddress
        {
            get { return _startAddress; }
            set
            {
                _startAddress = value; 
                RaisePropertyChanged();
            }
        }

        public ushort NumberOfPoints
        {
            get { return _numberOfPoints; }
            set
            {
                _numberOfPoints = value; 
                RaisePropertyChanged();
                FillInPoints();
            }
        }

        public ObservableCollection<TPointViewModel> Points
        {
            get { return _points; }
            set
            {
                _points = value;
                RaisePropertyChanged();
            }
        }
    }

    
    
}