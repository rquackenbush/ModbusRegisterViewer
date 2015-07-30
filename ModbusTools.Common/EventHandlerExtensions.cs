using System;

namespace ModbusTools.Common
{
    public static class EventHandlerExtensions
    {
        public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs> handler, TEventArgs args)
        {
            if (handler == null)
                return;

            handler(handler.Target, args);
        }

        public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs> handler, TEventArgs args, object sender)
        {
            if (handler == null)
                return;

            handler(sender, args);
        }
    }
}
