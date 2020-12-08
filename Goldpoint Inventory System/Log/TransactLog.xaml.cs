using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Input;

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
        }

        private void LblSearchTransact_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT date, [transaction], remarks from TransactionLogs WHERE CAST(date AS date) between @dateFrom and @dateTo", conn))
            {
                cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    log.Clear();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int dateIndex = reader.GetOrdinal("date");
                            int transactionIndex = reader.GetOrdinal("transaction");
                            int remarksIndex = reader.GetOrdinal("remarks");
                            log.Add(new TransactionLogDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                transaction = Convert.ToString(reader.GetValue(transactionIndex)),
                                remarks = Convert.ToString(reader.GetValue(remarksIndex))
                            });
                        }
                    }
                }
            }
        }
    }
}
