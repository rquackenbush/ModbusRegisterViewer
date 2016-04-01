using System;
using System.Collections;
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
        private readonly RangeModel _originalRangeModel;
        private const byte DefaultBlockSize = 125;

        private bool _isZeroBased;
        private string _name;
        private byte _blockSize = DefaultBlockSize;
        private ushort _startingRegisterIndex;
        private FieldType _fieldType;

        private static readonly KeyValuePair<RegisterType, string> _registerTypeInput = new KeyValuePair<RegisterType, string>(RegisterType.Input, "Input");
        private static readonly KeyValuePair<RegisterType, string> _registerTypeHolding = new KeyValuePair<RegisterType, string>(RegisterType.Holding, "Holding");

        private readonly ObservableCollection<FieldEditorViewModel> _fields = new ObservableCollection<FieldEditorViewModel>();

        private IList _selectedItems;

        private readonly KeyValuePair<RegisterType, string>[] _registerTypes =
        {
            _registerTypeHolding,
            _registerTypeInput
        };

        private RegisterType _registerType;
        private FieldEditorViewModel _selectedField;

        public RegisterRangeEditorViewModel(RangeModel originalRangeModel)
        {
            if (originalRangeModel == null)
                throw new ArgumentNullException("originalRangeModel");

            _originalRangeModel = originalRangeModel;
            
            _name = originalRangeModel.Name;
            _startingRegisterIndex = originalRangeModel.StartIndex;
            _registerType = originalRangeModel.RegisterType;

            if (originalRangeModel.Fields != null)
            {
                Fields.AddRange(originalRangeModel.Fields.Select(f => new FieldEditorViewModel(f.Clone())));
            }

            OkCommand = new RelayCommand(Ok, CanOk);
            CancelCommand = new RelayCommand(Cancel);

            MoveUpCommand = new RelayCommand(MoveUp, CanMoveUp);
            MoveDownCommand = new RelayCommand(MoveDown, CanMoveDown);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            MoveToTopCommand = new RelayCommand(MoveToTop, CanMoveToTop);
            MoveToBottomCommand = new RelayCommand(MoveToBottom, CanMoveToBottom);
            InsertAboveCommand = new RelayCommand(InsertAbove, CanInsertAbove);
            InsertBelowCommand = new RelayCommand(InsertBelow, CanInsertBelow);

            _fields.CollectionChanged += FieldsOnCollectionChanged;
        }

        private void FieldsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            CalculateOffsets();

            var newItems = new Lazy<List<FieldEditorViewModel>> (() => args.NewItems.Cast<FieldEditorViewModel>().ToList());
            var oldItems = new Lazy<List<FieldEditorViewModel>>(() => args.OldItems.Cast<FieldEditorViewModel>().ToList());

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    newItems.Value.ForEach(f => f.Parent = this);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    oldItems.Value.ForEach(f => f.Parent = this);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    oldItems.Value.ForEach(f => f.Parent = null);
                    newItems.Value.ForEach(f => f.Parent = this);
                    break;
            }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand MoveUpCommand { get; private set; }
        public ICommand MoveDownCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand MoveToTopCommand { get; private set; }
        public ICommand MoveToBottomCommand { get; private set; }
        public ICommand InsertAboveCommand { get; private set; }
        public ICommand InsertBelowCommand { get; private set; }

        private FieldEditorViewModel CreateNewEntry()
        {
            var fieldModel = new FieldModel();

            return new FieldEditorViewModel(fieldModel);
        }

        private bool CanMoveToTop()
        {
            return CanMoveUp();
        }

        private void MoveToTop()
        {
            var newIndex = 0;

            foreach (var entry in SelectedEntries)
            {
                //Get where the item is right now
                var oldIndex = Fields.IndexOf(entry);

                //Move the entry
                Fields.Move(oldIndex, newIndex);

                newIndex++;
            }
        }

        private bool CanMoveToBottom()
        {
            return CanMoveDown();
        }

        private void MoveToBottom()
        {
            foreach (var entry in SelectedEntries)
            {
                var oldIndex = Fields.IndexOf(entry);

                Fields.Move(oldIndex, Fields.Count - 1);
            }
        }

        private bool CanInsertAbove()
        {
            return SelectedEntries.Length == 1;
        }

        private void InsertAbove()
        {
            var firstSelected = SelectedEntries.FirstOrDefault();

            if (firstSelected == null)
                return;

            var index = Fields.IndexOf(firstSelected);

            Fields.Insert(index, CreateNewEntry());
        }

        private bool CanInsertBelow()
        {
            return SelectedEntries.Length == 1;
        }

        private void InsertBelow()
        {
            var firstSelected = SelectedEntries.FirstOrDefault();

            if (firstSelected == null)
                return;

            var index = Fields.IndexOf(firstSelected);

            Fields.Insert(index + 1, CreateNewEntry());
        }

        private bool CanDelete()
        {
            return SelectedEntries.Length > 0;
        }

        private void Delete()
        {
            var selectedEntries = SelectedEntries;

            foreach (var entry in selectedEntries)
            {
                Fields.Remove(entry);
            }
        }

        private void MoveUp()
        {
            foreach (var row in SelectedEntries)
            {
                var index = Fields.IndexOf(row);

                Fields.Move(index, index - 1);
            }
        }

        private bool CanMoveUp()
        {
            var selectedEntries = SelectedEntries;

            if (selectedEntries.Length == 0)
                return false;

            if (selectedEntries.Any(entry => Fields.IndexOf(entry) == 0))
                return false;

            return true;
        }

        private void MoveDown()
        {
            foreach (var row in SelectedEntries.Reverse())
            {
                var index = Fields.IndexOf(row);

                Fields.Move(index, index + 1);
            }
        }

        private bool CanMoveDown()
        {
            var selectedEntries = SelectedEntries.ToArray();

            if (selectedEntries.Length == 0)
                return false;

            if (selectedEntries.Any(entry => Fields.IndexOf(entry) >= Fields.Count - 1))
                return false;

            return true;
        }

        /// <summary>
        /// Use this to bind the selected items in the data grid.
        /// </summary>
        public IList SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => SelectedEntries);
            }
        }

        /// <summary>
        /// Gets a typed collection of the selected entries.
        /// </summary>
        public FieldEditorViewModel[] SelectedEntries
        {
            get
            {
                //Make sure that we have something to work with here.
                if (SelectedItems == null)
                    return new FieldEditorViewModel[] { };

                //We use .OfType so that we avoid grabbing the "new line" row.
                return SelectedItems.OfType<FieldEditorViewModel>().OrderBy(entry => _fields.IndexOf(entry)).ToArray();
            }
        }

        private void Ok()
        {
            CalculateOffsets();

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
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
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

        public ObservableCollection<FieldEditorViewModel> Fields
        {
            get { return _fields; }
        }

        public FieldEditorViewModel SelectedField
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

        internal RangeModel GetModel()
        {
            return new RangeModel()
            {
                Name = Name,
                RegisterType = RegisterType,
                StartIndex = StartingRegisterIndex,
                BlockSize = BlockSize,
                NumberOfRegisters = (ushort)Math.Ceiling(RegisterCount),
                Fields = Fields.Select(f => f.GetModel()).ToArray(),
                IsExpanded = _originalRangeModel.IsExpanded
            };
        }
    }
}
