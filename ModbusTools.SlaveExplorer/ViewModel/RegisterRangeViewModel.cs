using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.Runtime;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class RegisterRangeViewModel : RangeViewModelBase
    {
        private readonly IModbusAdapterProvider _modbusAdapterProvider;
        private RangeModel _rangeModel;
        private bool _isZeroBased;
        private ushort _startingRegisterIndex;
        private readonly ObservableCollection<FieldViewModel> _fields = new ObservableCollection<FieldViewModel>();
        private SlaveViewModel _parent;

        public RegisterRangeViewModel(IModbusAdapterProvider modbusAdapterProvider, RangeModel rangeModel, SlaveViewModel parent)
        {
            if (rangeModel == null) 
                throw new ArgumentNullException("rangeModel");

            _parent = parent;
            _modbusAdapterProvider = modbusAdapterProvider;
           
            EditCommand = new RelayCommand(Edit, CanEdit);
            ReadCommand = new RelayCommand(Read, CanRead);
            WriteCommand = new RelayCommand(Write, CanWrite);

            PopulateFromModel(rangeModel);
        }

        private void PopulateFromModel(RangeModel rangeModel)
        {
            Name = rangeModel.Name;

            //Ditch the old fields
            _fields.Clear();

            foreach (var field in rangeModel.Fields)
            {
                var runtimeField = RuntimeFieldFactory.Create(field);

                _fields.Add(new FieldViewModel(runtimeField));
            }

            _rangeModel = rangeModel;
        }

        public ICommand EditCommand { get; private set; }
        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }

        private void Read()
        {
            try
            {
                var contextFactory = _modbusAdapterProvider.GetFactory();

                ushort[] results = null;

                using (var master = contextFactory.Create())
                {
                    switch (_rangeModel.RegisterType)
                    {
                        case RegisterType.Holding:

                            results = master.Master.ReadHoldingRegisters(_parent.SlaveId,
                                                           _rangeModel.StartIndex,
                                                           _rangeModel.NumberOfRegisters,
                                                           _rangeModel.BlockSize);

                            break;


                        case RegisterType.Input:

                            results = master.Master.ReadInputRegisters(_parent.SlaveId,
                                                           _rangeModel.StartIndex,
                                                           _rangeModel.NumberOfRegisters,
                                                           _rangeModel.BlockSize);

                            break;
                    }
                }

                if (results != null)
                {
                    var bytes = results.ToBytes();

                    foreach (var field in Fields)
                    {
                        field.RuntimeField.SetBytes(bytes.Skip(field.Offset).Take(field.RuntimeField.Size).ToArray());
                    }

                    //TODO: Map these back to the fields                
                    Console.WriteLine(results);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanRead()
        {
            if (_rangeModel == null)
                return false;

            if (_modbusAdapterProvider == null)
                return false;

            if (_rangeModel.Fields.Length == 0)
                return false;

            if (_parent == null)
                return false;

            return true;
        }

        private void Write()
        {
            
        }

        private bool CanWrite()
        {
            if (_rangeModel == null)
                return false;

            if (_modbusAdapterProvider == null)
                return false;

            if (_rangeModel.RegisterType != RegisterType.Holding)
                return false;

            if (_rangeModel.Fields.Length == 0)
                return false;

            if (_parent == null)
                return false;

            return true;
        }

        private void Edit()
        {
            var rangeEditorViewModel = new RegisterRangeEditorViewModel(_rangeModel.Clone());

            var view = new RegisterRangeEditorView()
            {
                DataContext = rangeEditorViewModel
            };

            var result = view.ShowDialog();

            if (result == true)
            {
                var rangeModel = rangeEditorViewModel.GetModel();

                PopulateFromModel(rangeModel);
            }
        }

        protected internal override RangeModel GetModel()
        {
            return _rangeModel;
        }

        private bool CanEdit()
        {
            return true;
        }
        
        public ushort StartingRegisterIndex
        {
            get { return _startingRegisterIndex; }
            set
            {
                _startingRegisterIndex = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => StartingRegisterNumber);
            }
        }

        public ushort StartingRegisterNumber
        {
            get
            {
                if (IsZeroBased)
                    return StartingRegisterIndex;

                return (ushort)(StartingRegisterIndex + 1);
            }
            set
            {
                if (IsZeroBased)
                {
                    StartingRegisterIndex = value;
                }
                else
                {
                    StartingRegisterIndex = (ushort)(value - 1);
                }
            }
        }

        public ushort StartingRegisterNumberMin
        {
            get
            {
                if (IsZeroBased)
                    return 0;

                return 1;
            }
        }
        
        public bool IsZeroBased
        {
            get { return _isZeroBased; }
            set
            {
                _isZeroBased = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => StartingRegisterNumber);
                RaisePropertyChanged(() => StartingRegisterNumberMin);
            }
        }

        public IEnumerable<FieldViewModel> Fields
        {
            get { return _fields; }
        }

    }
}
