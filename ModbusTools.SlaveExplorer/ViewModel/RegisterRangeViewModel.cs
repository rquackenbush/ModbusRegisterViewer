using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.FieldTypeServices;
using ModbusTools.SlaveExplorer.Interfaces;
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

        private IRuntimeField[] _fields;
        private readonly ObservableCollection<IRuntimeFieldEditor> _fieldEditors = new ObservableCollection<IRuntimeFieldEditor>();

        private readonly SlaveViewModel _parent;
        private readonly IDirty _dirty;

        private ushort[] _previousRegisters;

        public RegisterRangeViewModel(IModbusAdapterProvider modbusAdapterProvider, RangeModel rangeModel, SlaveViewModel parent, IDirty dirty) 
            : base(dirty)
        {
            if (rangeModel == null) 
                throw new ArgumentNullException(nameof(rangeModel));

            _parent = parent;
            _dirty = dirty;
            _modbusAdapterProvider = modbusAdapterProvider;

           
            EditCommand = new RelayCommand(Edit, CanEdit);
            ReadCommand = new RelayCommand(Read, CanRead);
            WriteCommand = new RelayCommand(Write, CanWrite);
            DeleteCommand = new RelayCommand(Delete, CanDelete);

            PopulateFromModel(rangeModel);
        }

        private void PopulateFromModel(RangeModel rangeModel)
        {
            //Set the name
            Name = rangeModel.Name;

            //Ditch the old fields
            _fieldEditors.Clear();

            //Create the fields
            _fields = rangeModel.Fields.Select(f => FieldTypeServiceFactory.GetFieldTypeService(f.FieldType).CreateRuntimeField(f)).ToArray();

            //Set up the field editors
            _fieldEditors.AddRange(_fields.SelectMany(f => f.FieldEditors));

            //Save this
            _rangeModel = rangeModel;

            //To expand or not expand. That is the question. Whether tis nobler to...
            IsExpanded = rangeModel.IsExpanded;
        }

        public ICommand EditCommand { get; private set; }
        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        private void Delete()
        {
            var message = string.Format("Are you sure you want to delete register range '{0}'?", Name);

            var result = MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                _parent.RemoveRange(this);    
            }            
        }

        private bool CanDelete()
        {
            return _parent != null;
        }

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

                //Save this for next time
                _previousRegisters = results;

                if (results != null)
                {
                    var bytes = results.ToBytes();

                    foreach (var field in _fields)
                    {
                        field.SetBytes(bytes.Skip(field.Offset).Take(field.Size).ToArray());
                    }

                    //TODO: Map these back to the fields                
                    Console.WriteLine(results);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name);
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
            try
            {
                if (_previousRegisters == null)
                {
                    _previousRegisters = new ushort[_rangeModel.NumberOfRegisters];
                }

                //Convert this to bytes
                var bytes = _previousRegisters.ToBytes();

                foreach (var field in _fields)
                {
                    var fieldBytes = field.GetBytes();

                    for (int index = 0; index < fieldBytes.Length; index++)
                    {
                        bytes[index + field.Offset] = fieldBytes[index];
                    }
                }

                //Convert back to regsiters
                _previousRegisters = bytes.ToRegisters();

                //Write it back
                using (var master = _modbusAdapterProvider.GetFactory().Create())
                {
                    master.Master.WriteMultipleRegisters(_parent.SlaveId, _rangeModel.StartIndex, _previousRegisters);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name);
            }
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

                _dirty.MarkDirtySafe();
            }
        }

        protected internal override RangeModel GetModel()
        {
            _rangeModel.IsExpanded = IsExpanded;

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

        public IEnumerable<IRuntimeFieldEditor> Fields
        {
            get { return _fieldEditors; }
        }

    }
}
