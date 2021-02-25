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

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for PrintCopies.xaml
    /// </summary>
    public partial class PrintCopies : Window
    {
        public int copies = 1;
        public PrintCopies()
        {
            InitializeComponent();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            copies = (int)txtCopies.Value;
            this.DialogResult = false;
            this.Close();
        }
    }
}
