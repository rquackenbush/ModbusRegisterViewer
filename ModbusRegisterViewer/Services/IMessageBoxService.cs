using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModbusRegisterViewer.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string messageBoxText);

        MessageBoxResult Show(string messageBoxTest, string caption);
    }
}
