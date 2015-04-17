using System;

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
