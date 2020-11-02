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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Goldpoint_Inventory_System.Log
{
    /// <summary>
    /// Interaction logic for TransactDetails.xaml
    /// </summary>
    public partial class TransactDetails : UserControl
    {
        public TransactDetails()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();

        }
    }
}
