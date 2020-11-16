using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for JobOrder.xaml
    /// </summary>
    public partial class JobOrder : UserControl
    {
        ObservableCollection<JobOrderDataModel> services = new ObservableCollection<JobOrderDataModel>();
        public JobOrder()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgService.ItemsSource = services;
            services.Add(new JobOrderDataModel
            {
                qty = 364,
                unit = "pcs",
                description = "Photocopy of Book Soft Bind (Color - Pink)",
                copy = "1",
                size = "SH",
                material = "BOOK 50",
                unitPrice = .70,
                amount = 254.50
            });
        }

        private void SearchJobOrders_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            JobOrders jo = new JobOrders();
            jo.Show();
        }

        private void CmbJobOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.ComboBoxItemAdv comboBox = (Syncfusion.Windows.Tools.Controls.ComboBoxItemAdv)cmbJobOrder.SelectedItem;

            if (comboBox != null)
            {
                string value = comboBox.Content.ToString();
                if (value == "Printing, Services, etc.")
                {
                    expServ.IsEnabled = true;
                    expTarp.IsEnabled = false;
                }
                else if (value == "Tarpaulin")
                {

                    expServ.IsEnabled = false;
                    expTarp.IsEnabled = true;
                }
                else
                {
                    expServ.IsEnabled = false;
                    expTarp.IsEnabled = false;
                }
            }
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

        private void TxtAddTarp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
