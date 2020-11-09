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
    /// Interaction logic for TransactLog.xaml
    /// </summary>
    public partial class TransactLog : UserControl
    {

        ObservableCollection<TransactionLogDataModel> log = new ObservableCollection<TransactionLogDataModel>();

        public TransactLog()
        {
            InitializeComponent();
            dgTransaction.ItemsSource = log;
            log.Add(new TransactionLogDataModel
            {
                date = "11/3/2020",
                transaction = "Service: Job Order (Printing, Services, Etc.), Service No: 00001; Customer: Juan Dela Cruz; Downpayment; Issued By: Jose",
                remarks = ""
            });
            log.Add(new TransactionLogDataModel
            {
                date = "11/3/2020",
                transaction = "Service: Photocopy, Service No: 00001; Customer: Juan Dela Cruz; Fully paid; Issued By: Josie",
                remarks = ""
            });

        }
    }
}
