using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    public class RangeViewModel : ViewModelBase
    {
        private RegisterType _type;
        public RegisterType Type 
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }    
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public bool IsReadOnly 
        {
            get { return this.Type != RegisterType.Input; }
        }

        private ushort _startingAddress;
        public ushort StartingAddress 
        {
            get { return _startingAddress;  }
            set
            {
                _startingAddress = value;
                RaisePropertyChanged(() => StartingAddress);
            }
        }

        private byte _number;
        public byte Number
        {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged(() => Number);
            }
        }

        private ObservableCollection<FieldViewModel> _fields;
        public ObservableCollection<FieldViewModel> Fields
        {
            get { return _fields; }
            set
            {
                _fields = value;
                RaisePropertyChanged(() => Fields);
            }
        }
    } 
}
