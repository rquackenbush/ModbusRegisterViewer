using System;
using ModbusTools.Common.Model;
using ModbusTools.Common.ViewModel;
using ModbusTools.SlaveViewer.Model;

namespace ModbusTools.SlaveViewer.ViewModel
{
    public class SlaveExplorerRegisterViewModel : WriteableRegisterViewModel
    {
        private readonly DescriptionStore _descriptionStore;

        internal SlaveExplorerRegisterViewModel(ushort registerIndex, ushort value, DescriptionStore descriptionStore)
         : base(registerIndex, value)
        {
            if (descriptionStore == null) throw new ArgumentNullException("descriptionStore");

            _descriptionStore = descriptionStore;
        }

        public string Description
        {
            get { return _descriptionStore[RegisterNumber]; }
            set
            {
                _descriptionStore[RegisterNumber] = value;
                RaisePropertyChanged(() => Description);
            }
        }

        internal void RaiseDescriptionPropertyChanged()
        {
            RaisePropertyChanged(() => Description);
        }
    }
}
