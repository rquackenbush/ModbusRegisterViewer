using System;
using GalaSoft.MvvmLight;
using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    public class EditFieldViewModel : ViewModelBase
    {
        private string _name;
        private FieldType _fieldType = FieldType.UINT16;
        private int _offset;

        internal RegisterRangeEditorViewModel Parent { get; set; }

        private void PerformParentAction(Action<RegisterRangeEditorViewModel> action)
        {
            var parent = Parent;

            if (parent == null)
                return;

            action(parent);
        }

        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                RaisePropertyChanged();
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

        public FieldType FieldType
        {
            get { return _fieldType; }
            set
            {
                _fieldType = value; 
                RaisePropertyChanged();
                RaisePropertyChanged(() => Size);
                PerformParentAction(p => p.CalculateOffsets());
            }
        }

        public int Size
        {
            get
            {
                switch (FieldType)
                {
                    case FieldType.UINT32:
                    case FieldType.INT32:
                    case FieldType.FLOAT32:
                        return 4;

                    case FieldType.UINT16:
                    case FieldType.INT16:
                    //case FieldType.BIT16:
                        return 2;

                    case FieldType.UINT8:
                    case FieldType.INT8:
                    //case FieldType.BIT8:
                        return 1;

                    default:
                        throw new Exception(string.Format("Unexpected value {0}", FieldType));
                }
            }
        }

        public FieldModel GetModel()
        {
            return new FieldModel()
            {
                Name = Name,
                FieldType = FieldType,
                Offset = Offset
            };
        }
    }
}
