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

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for StockOut.xaml
    /// </summary>
    public partial class StockOut : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        public StockOut()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgStockOut.ItemsSource = items;
            items.Add(new ItemDataModel
            {
                itemCode = "2020-1100001",
                description = "Black Ballpen",
                type = "Ballpen",
                brand = "HBW",
                size = "N\\A",
                qty = 10,
                totalPerItem = 80,
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
                totalPerItem = 500,
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
                totalPerItem = 27,
                remarks = "",
            });

        }

        private void radiobuttonPayment(object sender, RoutedEventArgs e)
        {
            RadioButton radiobtn = (RadioButton)sender;
            string value = radiobtn.Content.ToString();
            switch (value)
            {
                case "Unpaid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    break;
                case "Down payment":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = true;
                    break;
                case "Paid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    break;
                case "Company Use":
                    break;
            }
        }

        private void checkboxService(object sender, RoutedEventArgs e)
        {
            if (chkCompany.IsChecked == true)
            {
                chkDR.IsChecked = false;
                chkInv.IsChecked = false;
                chkOR.IsChecked = false;
            }
            else
            {
                CheckBox chkbox = (CheckBox)sender;
                string value = chkbox.Content.ToString();

                if(chkbox.IsChecked == true && value == "Company Use")
                {
                    chkDR.IsChecked = false;
                    chkInv.IsChecked = false;
                    chkOR.IsChecked = false;

                    txtInv.IsEnabled = false;
                    txtDRNo.IsEnabled = false;
                    txtORNo.IsEnabled = false;
                }

                if (chkbox.IsChecked == true && value == "Delivery Receipt")
                {
                    txtDRNo.IsEnabled = true;
                }
                if (chkbox.IsChecked == true && value == "Original Receipt")
                {
                    txtORNo.IsEnabled = true;
                }
                if (chkbox.IsChecked == true && value == "Invoice")
                {
                    txtInv.IsEnabled = true;
                }

            }
        }

        private void unCheckBoxService(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            string value = chkbox.Content.ToString();

            if (chkbox.IsChecked == false && value == "Invoice")
            {
                txtInv.IsEnabled = false;
            }
            if (chkbox.IsChecked == false && value == "Delivery Receipt")
            {
                txtDRNo.IsEnabled = false;
            }
            if (chkbox.IsChecked == false && value == "Original Receipt")
            {
                txtORNo.IsEnabled = false;
            }
        }
    }
}
