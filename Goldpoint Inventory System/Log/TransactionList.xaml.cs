using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Goldpoint_Inventory_System.Log
{
    /// <summary>
    /// Interaction logic for Transactions.xaml
    /// </summary>
    public partial class TransactionList : Window
    {
        ObservableCollection<UserTransactionDataModel> customers = new ObservableCollection<UserTransactionDataModel>();
        public TransactionList()
        {
            InitializeComponent();
            dgTransactions.ItemsSource = customers;
            dgTransactions.RowHeight = 40;

            fillUpTransactions();
        }

        private void TxtCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustomerName.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where customerName LIKE @custName and inaccessible = 1 ORDER BY customerName ASC", conn))
                {
                    cmd.Parameters.AddWithValue("@custName", '%' + txtCustomerName.Text + '%');
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        customers.Clear();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int dateIndex = reader.GetOrdinal("date");
                                int deadlineIndex = reader.GetOrdinal("deadline");
                                int drNoindex = reader.GetOrdinal("drNo");
                                int serviceIndex = reader.GetOrdinal("service");
                                int customerNameIndex = reader.GetOrdinal("customerName");
                                int addressIndex = reader.GetOrdinal("address");
                                int contactNoIndex = reader.GetOrdinal("contactNo");
                                int statusIndex = reader.GetOrdinal("status");

                                customers.Add(new UserTransactionDataModel
                                {
                                    date = Convert.ToString(reader.GetValue(dateIndex)),
                                    deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                    drNo = Convert.ToInt32(reader.GetValue(drNoindex)),
                                    service = Convert.ToString(reader.GetValue(serviceIndex)),
                                    customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                    address = Convert.ToString(reader.GetValue(addressIndex)),
                                    contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                    status = Convert.ToString(reader.GetValue(statusIndex)),
                                });
                            }
                        }
                        else
                        {
                            fillUpTransactions();
                        }
                    }
                }
            }
            else
            {
                fillUpTransactions();
            }

        }

        private void LblSearchTransact_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServiceNo.Text) || string.IsNullOrEmpty(cmbService.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                switch (cmbService.Text)
                {
                    case "Delivery Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where drNo = @drNo and inaccessible = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@drNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                customers.Clear();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int dateIndex = reader.GetOrdinal("date");
                                        int deadlineIndex = reader.GetOrdinal("deadline");
                                        int drNoindex = reader.GetOrdinal("drNo");
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int customerNameIndex = reader.GetOrdinal("customerName");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        customers.Add(new UserTransactionDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                            drNo = Convert.ToInt32(reader.GetValue(drNoindex)),
                                            service = Convert.ToString(reader.GetValue(serviceIndex)),
                                            customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                            address = Convert.ToString(reader.GetValue(addressIndex)),
                                            contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                            status = Convert.ToString(reader.GetValue(statusIndex)),
                                        });
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    fillUpTransactions();
                                    return;
                                }
                            }
                        }

                        break;
                    case "Official Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where orNo = @orNo and inaccessible = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@orNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                customers.Clear();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int dateIndex = reader.GetOrdinal("date");
                                        int deadlineIndex = reader.GetOrdinal("deadline");
                                        int drNoindex = reader.GetOrdinal("drNo");
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int customerNameIndex = reader.GetOrdinal("customerName");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        customers.Add(new UserTransactionDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                            drNo = Convert.ToInt32(reader.GetValue(drNoindex)),
                                            service = Convert.ToString(reader.GetValue(serviceIndex)),
                                            customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                            address = Convert.ToString(reader.GetValue(addressIndex)),
                                            contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                            status = Convert.ToString(reader.GetValue(statusIndex)),
                                        });
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    fillUpTransactions();
                                    return;
                                }
                            }
                        }

                        break;
                    case "Invoice":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where invoiceNo = @invoiceNo and inaccessible = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@invoiceNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                customers.Clear();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int dateIndex = reader.GetOrdinal("date");
                                        int deadlineIndex = reader.GetOrdinal("deadline");
                                        int drNoindex = reader.GetOrdinal("drNo");
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int customerNameIndex = reader.GetOrdinal("customerName");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        customers.Add(new UserTransactionDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                            drNo = Convert.ToInt32(reader.GetValue(drNoindex)),
                                            service = Convert.ToString(reader.GetValue(serviceIndex)),
                                            customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                            address = Convert.ToString(reader.GetValue(addressIndex)),
                                            contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                            status = Convert.ToString(reader.GetValue(statusIndex)),
                                        });
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    fillUpTransactions();
                                    return;
                                }
                            }
                        }

                        break;
                    default:
                        fillUpTransactions();
                        break;
                }
            }
        }

        private void fillUpTransactions()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where inaccessible = 1 ORDER BY customerName ASC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    customers.Clear();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int dateIndex = reader.GetOrdinal("date");
                            int deadlineIndex = reader.GetOrdinal("deadline");
                            int drNoindex = reader.GetOrdinal("drNo");
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                drNo = Convert.ToInt32(reader.GetValue(drNoindex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                            });
                        }
                    }
                }
            }
        }
    }
}
