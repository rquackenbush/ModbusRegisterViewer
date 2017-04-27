using System;
using Cas.Common.WPF.Interfaces;

namespace ModbusTools.Common
{
    public static class IMessageBoxServiceExtensions
    {
        public static void Show(this IMessageBoxService messageBoxService, Exception ex, string caption)
        {
            messageBoxService.Show(ex.Message, caption);
        }
    }
}
