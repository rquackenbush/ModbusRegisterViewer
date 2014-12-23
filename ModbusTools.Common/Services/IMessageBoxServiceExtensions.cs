using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTools.Common.Services
{
    public static class IMessageBoxServiceExtensions
    {
        public static void Show(this IMessageBoxService messageBoxService, Exception ex, string caption)
        {
            messageBoxService.Show(ex.Message, caption);
        }
    }
}
