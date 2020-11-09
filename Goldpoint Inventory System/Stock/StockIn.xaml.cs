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

namespace Goldpoint_Inventory_System.Stock
{

    public partial class StockIn : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        public StockIn()
        {
            InitializeComponent();

            dgStockIn.ItemsSource = items;

            items.Add(new ItemDataModel
            {
                itemCode = "2020-1100001",
                description = "Black Ballpen",
                type = "Ballpen",
                brand = "HBW",
                size = "N\\A",
                qty = 10,
                remarks = "",

            });

            items.Add(new ItemDataModel
            {
                itemCode = "2020-2100001",
                description = "Short Ream",
                type = "Bondpaper",
                brand = "Hard Copy",
                size = "N\\A",
                qty = 1,
                remarks = "",
            });

            items.Add(new ItemDataModel
            {
                itemCode = "2020-3100001",
                description = "Short Envelope",
                type = "Envelope",
                brand = "Generic",
                size = "N\\A",
                qty = 5,
                remarks = "",
            });

        }

    }
}
