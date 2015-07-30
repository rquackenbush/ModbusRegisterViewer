using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class RegisterRangeEditorViewModel : ViewModelBase, ICloseableViewModel
    {
        private const byte DefaultBlockSize = 125;

        private bool _isZeroBased;
        private string _name;
        private byte _blockSize = DefaultBlockSize;
        private ushort _startingRegisterIndex;
        private FieldType _fieldType;

        private static readonly KeyValuePair<RegisterType, string> _registerTypeInput = new KeyValuePair<RegisterType, string>(RegisterType.Input, "Input");
        private static readonly KeyValuePair<RegisterType, string> _registerTypeHolding = new KeyValuePair<RegisterType, string>(RegisterType.Holding, "Holding");

        private readonly ObservableCollection<EditFieldViewModel> _fields = new ObservableCollection<EditFieldViewModel>();

        private readonly KeyValuePair<RegisterType, string>[] _registerTypes =
        {
            _registerTypeHolding,
            _registerTypeInput
        };

        private RegisterType _registerType;
        private EditFieldViewModel _selectedField;

        public RegisterRangeEditorViewModel(RangeModel rangeModel)
        {
            if (rangeModel == null) 
                throw new ArgumentNullException("rangeModel");

            Name = rangeModel.Name;
            StartingRegisterIndex = rangeModel.StartIndex;

            if (rangeModel.Fields != null)
            {
                foreach (var field in rangeModel.Fields)
                {
                    Fields.Add(new EditFieldViewModel()
                    {
                        Name = field.Name,
                        FieldType = field.FieldType
                    });
                }
            }
            

            OkCommand = new RelayCommand(Ok, CanOk);
            CancelCommand = new RelayCommand(Cancel);

            _fields.CollectionChanged += FieldsOnCollectionChanged;
        }

        private void FieldsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            CalculateOffsets();

            var newItems = new Lazy<List<EditFieldViewModel>> (() => args.NewItems.Cast<EditFieldViewModel>().ToList());
            var oldItems = new Lazy<List<EditFieldViewModel>>(() => args.NewItems.Cast<EditFieldViewModel>().ToList());

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    newItems.Value.ForEach(f => f.Parent = this);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    newItems.Value.ForEach(f => f.Parent = this);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    oldItems.Value.ForEach(f => f.Parent = null);
                    newItems.Value.ForEach(f => f.Parent = this);
                    break;
            }

        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void Ok()
        {
            Close.RaiseEvent(new CloseEventArgs(true));
        }

        private bool CanOk()
        {
            if (Fields.Count == 0)
                return false;

            if (string.IsNullOrWhiteSpace(Name))
                return false;

            return true;
        }

        private void Cancel()
        {
            Close.RaiseEvent(new CloseEventArgs(false));
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

        public RegisterType RegisterType
        {
            get { return _registerType; }
            set
            {
                _registerType = value; 
                RaisePropertyChanged();
            }
        }

        public KeyValuePair<RegisterType, string>[] RegisterTypes
        {
            get { return _registerTypes; }
        }

        public FieldType FieldType
        {
            get { return _fieldType; }
            set
            {
                _fieldType = value; 
                RaisePropertyChanged();
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

        public byte BlockSize
        {
            get { return _blockSize; }
            set
            {
                _blockSize = value;
                RaisePropertyChanged(() => BlockSize);
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

        public ObservableCollection<EditFieldViewModel> Fields
        {
            get { return _fields; }
        }

        public EditFieldViewModel SelectedField
        {
            get { return _selectedField; }
            set
            {
                _selectedField = value; 
                RaisePropertyChanged();
            }
        }

        internal void CalculateOffsets()
        {
            var offset = 0;

            foreach (var field in Fields)
            {
                field.Offset = offset;

                offset += field.Size;
            }

            RaisePropertyChanged(() => ByteCount);
            RaisePropertyChanged(() => RegisterCount);
        }

        public int ByteCount
        {
            get { return _fields.Sum(f => f.Size); }
        }

        public double RegisterCount
        {
            get { return ByteCount/2.0; }
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
