using System;
using System.Windows;

namespace ModbusTools.Common
{
    public static class FrameworkElementExtensions
    {
        public static void PerformViewModelAction<TViewModel>(this FrameworkElement element, Action<TViewModel> action)
            where TViewModel : class 
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var viewModel = element.DataContext as TViewModel;

            if (viewModel == null)
                throw new Exception("Unable to find view model");

            action(viewModel);
        }
    }
}
