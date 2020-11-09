using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Office2019Colorful.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        ObservableCollection<UserTransactionDataModel> customer = new ObservableCollection<UserTransactionDataModel>();
        public StartPage()
        {
            InitializeComponent();
            dgCritical.ItemsSource = items;
            dgRecentTransact.ItemsSource = customer;
            items.Add(new ItemDataModel
            {
                itemCode = "2020-3100001",
                description = "Short Envelope",
                qty = 5,
                criticalLvl = 5,
                fastMoving = "N\\A",
            });

            customer.Add(new UserTransactionDataModel
            {
                date = "11/3/2020",
                transaction = "Job Order (Printing, Services, etc.)",
                service = "Job Order",
                serviceNo = "1",
                customerName = "Juan Dela Cruz",
            });
        }
    }
}
