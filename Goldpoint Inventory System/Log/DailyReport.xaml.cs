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
    /// Interaction logic for Dailyeport.xaml
    /// </summary>
    public partial class DailyReport : UserControl
    {
        public DailyReport()
        {
            InitializeComponent();
        }

        private void CmbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.ComboBoxItemAdv typeItem = (Syncfusion.Windows.Tools.Controls.ComboBoxItemAdv)cmbService.SelectedItem;
            switch (typeItem.Content.ToString())
            {
                case "Original Receipt":
                    break;
                case "Delivery Receipt":
                    break;
                case "Invoice":
                    break;
                    
            }
        }
    }
}
