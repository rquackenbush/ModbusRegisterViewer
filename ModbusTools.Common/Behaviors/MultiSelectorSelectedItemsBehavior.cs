using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace ModbusTools.Common.Behaviors
{
    public class MultiSelectorSelectedItemsBehavior : Behavior<MultiSelector>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(MultiSelectorSelectedItemsBehavior), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IList SelectedItems
        {
            get
            {
                return (IList)this.GetValue(MultiSelectorSelectedItemsBehavior.SelectedItemsProperty);
            }
            set
            {
                this.SetValue(MultiSelectorSelectedItemsBehavior.SelectedItemsProperty, (object)value);
            }
        }

        static MultiSelectorSelectedItemsBehavior()
        {
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(this.AssociatedObjectSelectionChanged);
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectionChanged -= new SelectionChangedEventHandler(this.AssociatedObjectSelectionChanged);
        }

        private void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object[] objArray = new object[this.AssociatedObject.SelectedItems.Count];
            this.AssociatedObject.SelectedItems.CopyTo((Array)objArray, 0);
            this.SelectedItems = (IList)objArray;
        }
    }
}
