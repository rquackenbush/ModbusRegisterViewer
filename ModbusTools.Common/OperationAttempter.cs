using System;

namespace ModbusTools.Common
{
    public static class OperationAttempter
    {
        /// <summary>
        /// Attempts an operation.
        /// </summary>
        /// <param name="action">The operation to attempt.</param>
        /// <param name="exceptionHandler">This is code to execute if the action fails. If null, exceptions are swallowed.</param>
        public static void Attempt(Action action, Action<Exception> exceptionHandler = null)
        {
            try
            {
                //Attempt to perform the operation
                action();
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
        }
    }
}
