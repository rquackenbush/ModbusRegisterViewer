using System;
using ModbusTools.Common.Model;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.SlaveViewer.ViewModel
{
    public class SlaveExplorerRegisterViewModel : RegisterViewModel
    {
        private readonly DescriptionStore _descriptionStore;

        internal SlaveExplorerRegisterViewModel(ushort registerIndex, ushort value, DescriptionStore descriptionStore)
         : base(registerIndex, value)
        {
            if (descriptionStore == null) throw new ArgumentNullException(nameof(descriptionStore));

            _descriptionStore = descriptionStore;
        }

        public string Description
        {
            get { return _descriptionStore[RegisterIndex]; }
            set
            {
                _descriptionStore[RegisterIndex] = value;
                RaisePropertyChanged(() => Description);
            }
        }

        internal void RaiseDescriptionPropertyChanged()
        {
            RaisePropertyChanged(() => Description);
        }
    }
}
