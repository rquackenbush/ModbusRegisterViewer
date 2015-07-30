using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.Common
{
    public class CloseableBehavior : Behavior<Window>
    {
        private ICloseableViewModel _closeable;

        protected override void OnAttached()
        {
            AssociatedObject.Closing += AssociatedObject_Closing;
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
            AssociatedObject.Closed += AssociatedObject_Closed;

            _closeable = AssociatedObject.DataContext as ICloseableViewModel;

            if (_closeable != null)
                _closeable.Close += Closeable_Close;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Closing -= AssociatedObject_Closing;
            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
            AssociatedObject.Closed -= AssociatedObject_Closed;
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_closeable != null)
                _closeable.Close -= Closeable_Close;

            _closeable = e.NewValue as ICloseableViewModel;

            if (_closeable == null)
                return;

            _closeable.Close += Closeable_Close;
        }

        private void Closeable_Close(object sender, CloseEventArgs e)
        {
            AssociatedObject.DialogResult = e.DialogResult;
            AssociatedObject.Close();
        }

        private void AssociatedObject_Closing(object sender, CancelEventArgs e)
        {
            if (_closeable == null || _closeable.CanClose())
                return;

            e.Cancel = true;
        }

        private void AssociatedObject_Closed(object sender, EventArgs e)
        {
            if (_closeable == null)
                return;
            _closeable.Closed();
        }
    }
}
