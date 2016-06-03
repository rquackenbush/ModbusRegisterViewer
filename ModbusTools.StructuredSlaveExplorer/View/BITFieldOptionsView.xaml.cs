using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModbusTools.StructuredSlaveExplorer.View
{
    /// <summary>
    /// Interaction logic for BIT8FieldOptionsView.xaml
    /// </summary>
    public partial class BITFieldOptionsView : Window
    {
        public BITFieldOptionsView()
        {
            InitializeComponent();
        }

        private void BitNameGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            textBox?.SelectAll();
        }
    }
}
