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
    }
}
