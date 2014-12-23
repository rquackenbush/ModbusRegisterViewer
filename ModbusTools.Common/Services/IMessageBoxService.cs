using System.Windows;

namespace ModbusTools.Common.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string messageBoxText);

        MessageBoxResult Show(string messageBoxTest, string caption);
    }
}
