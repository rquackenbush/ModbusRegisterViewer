using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModbusTools.SlaveExplorer.View
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
