using NLog;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for IssueDeliveryReceipt.xaml
    /// </summary>
    public partial class IssueDeliveryReceipt : UserControl
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public IssueDeliveryReceipt()
        {
            InitializeComponent();
        }

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
