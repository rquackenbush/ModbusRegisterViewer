using System.Windows;

namespace ModbusTools.Common.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        private readonly Window _owner;

        public MessageBoxService(Window owner = null)
        {
            _owner = owner;
        }

        public MessageBoxResult Show(string messageBoxText)
        {
            if (_owner == null)
            {
                return MessageBox.Show(messageBoxText);    
            }

            return MessageBox.Show(_owner, messageBoxText);
        }

        public MessageBoxResult Show(string messageBoxText, string caption)
        {
            if (_owner == null)
            {
                return MessageBox.Show(messageBoxText, caption);
            }

            return MessageBox.Show(_owner, messageBoxText, caption);
        }
    }
}
