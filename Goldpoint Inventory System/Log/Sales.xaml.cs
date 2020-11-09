using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Office2019Colorful.WPF;
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
    public partial class Sales : UserControl
    {
        List<SalesDataModel> data = new List<SalesDataModel>();
        List<SalesDataModel> services = new List<SalesDataModel>();

        public Sales()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgDailyService.ItemsSource = services;
            data.Add(new SalesDataModel
            {
                date = "11/3/2020",
                sales = 50123.34
            });
            data.Add(new SalesDataModel
            {
                date = "11/4/2020",
                sales = 67317.43
            });
            data.Add(new SalesDataModel
            {
                date = "11/5/2020",
                sales = 73423.73
            });
            chartSales.ItemsSource = data;

            services.Add(new SalesDataModel
            {
                service = "Photocopy",
                sales = 898.43,
            });
            services.Add(new SalesDataModel
            {
                service = "Tarpaulin",
                sales = 500,
            });
            services.Add(new SalesDataModel
            {
                service = "Load",
                sales = 300,
            });
            services.Add(new SalesDataModel
            {
                service = "NA Supplies",
                sales = 150,
            });
            services.Add(new SalesDataModel
            {
                service = "Print",
                sales = 85,
            });
        }

        /*
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
        */
    }
}
