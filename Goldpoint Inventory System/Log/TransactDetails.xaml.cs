using Goldpoint_Inventory_System.Transactions;
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

namespace Goldpoint_Inventory_System.Log
{
    /// <summary>
    /// Interaction logic for TransactDetails.xaml
    /// </summary>
    public partial class TransactDetails : UserControl
    {
        ObservableCollection<PhotocopyDataModel> photocopy = new ObservableCollection<PhotocopyDataModel>();
        ObservableCollection<ItemDataModel> stockOut = new ObservableCollection<ItemDataModel>();
        ObservableCollection<PaymentHistoryDataModel> payHist = new ObservableCollection<PaymentHistoryDataModel>();
        ObservableCollection<JobOrderDataModel> services = new ObservableCollection<JobOrderDataModel>();


        public TransactDetails()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgService.ItemsSource = services;
            dgTransact.ItemsSource = stockOut;
            dgPaymentHistory.ItemsSource = payHist;
            dgPhotocopy.ItemsSource = photocopy;
            photocopy.Add(new PhotocopyDataModel
            {
                item = "Long",
                price = .80,
                qty = 5,
                totalPerItem = 5
            });
            photocopy.Add(new PhotocopyDataModel
            {
                item = "Short",
                price = .70,
                qty = 5,
                totalPerItem = 3.5
            });
            photocopy.Add(new PhotocopyDataModel
            {
                item = "A4",
                price = .90,
                qty = 5,
                totalPerItem = 4.5
            });

            stockOut.Add(new ItemDataModel
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

            stockOut.Add(new ItemDataModel
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

            stockOut.Add(new ItemDataModel
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

            payHist.Add(new PaymentHistoryDataModel
            {
                date = "11/3/2020",
                price = 1000
            });

            payHist.Add(new PaymentHistoryDataModel
            {
                date = "11/4/2020",
                price = 500
            });

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

            services.Add(new JobOrderDataModel
            {
                qty = 10,
                unit = "pcs",
                description = "Photocopy of Book Soft Bind (Color - Pink)",
                copy = "1",
                size = "SH",
                material = "PAPER",
                unitPrice = .70,
                amount = 254.50
            });

        }
    }
}
