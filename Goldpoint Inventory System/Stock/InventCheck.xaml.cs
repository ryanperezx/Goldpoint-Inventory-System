using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Goldpoint_Inventory_System.Stock
{
    /// <summary>
    /// Interaction logic for InventCheck.xaml
    /// </summary>
    public partial class InventCheck : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();

        public InventCheck()
        {
            InitializeComponent();

            dgInventory.ItemsSource = items;

            items.Add(new ItemDataModel
            {
                itemCode = "2020-1100001",
                description = "Black Ballpen",
                type = "Ballpen",
                brand = "HBW",
                size = "N\\A",
                qty = 10,
                msrp = 5,
                criticalLvl = 5,
                criticalState = false,
                price = 6.5,
                fastMoving = "Every 15 days",
                remarks = "",
            });

            items.Add(new ItemDataModel
            {
                itemCode = "2020-2100001",
                description = "Short Ream",
                type = "Bondpaper",
                brand = "Hard Copy",
                size = "N\\A",
                qty = 10,
                msrp = 350,
                criticalLvl = 5,
                criticalState = false,
                price = 500,
                fastMoving = "Every 15 days",
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
                criticalLvl = 5,
                criticalState = true,
                msrp = 5.6,
                price = 8,
                fastMoving = "N\\A",
                remarks = "",
            });


        }

        private void highlightCritical()
        {

            var datarow = items.FirstOrDefault(x => x.qty < x.criticalLvl);
            if (datarow != null)
            {

            }
        }
    }
}
