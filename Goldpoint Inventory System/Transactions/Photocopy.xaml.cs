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
    /// Interaction logic for Photocopy.xaml
    /// </summary>
    public partial class Photocopy : UserControl
    {
        ObservableCollection<PhotocopyDataModel> items = new ObservableCollection<PhotocopyDataModel>();

        public Photocopy()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgPhotocopy.ItemsSource = items;
            items.Add(new PhotocopyDataModel
            {
                item = "Long",
                price = .80,
                qty = 5,
                totalPerItem = 5
            });
            items.Add(new PhotocopyDataModel
            {
                item = "Short",
                price = .70,
                qty = 5,
                totalPerItem = 3.5
            });
            items.Add(new PhotocopyDataModel
            {
                item = "A4",
                price = .90,
                qty = 5,
                totalPerItem = 4.5
            });



        }

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void chkServ_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkServ_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void radiobuttonPayment(object sender, System.Windows.RoutedEventArgs e)
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
            }
        }
    }
}
