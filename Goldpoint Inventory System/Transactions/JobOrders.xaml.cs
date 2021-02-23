using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for JobOrders.xaml
    /// </summary>
    public partial class JobOrders : Window
    {

        ObservableCollection<UserTransactionDataModel> customers = new ObservableCollection<UserTransactionDataModel>();
        public JobOrders()
        {
            InitializeComponent();
            dgJobOrders.ItemsSource = customers;
            dgJobOrders.RowHeight = 40;
            fillUpJobOrders();
        }

        private void TxtCustName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustName.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo is not null AND DATALENGTH(jobOrderNo) > 0 and (service = 'Printing, Services, etc.' or service = 'Tarpaulin') and customerName LIKE @custName and inaccessible = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@custName", '%' + txtCustName.Text + '%');
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        customers.Clear();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int dateIndex = reader.GetOrdinal("date");
                                int deadlineIndex = reader.GetOrdinal("deadline");
                                int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                int serviceIndex = reader.GetOrdinal("service");
                                int customerNameIndex = reader.GetOrdinal("customerName");
                                int issuedByIndex = reader.GetOrdinal("issuedBy");
                                int addressIndex = reader.GetOrdinal("address");
                                int contactNoIndex = reader.GetOrdinal("contactNo");
                                int statusIndex = reader.GetOrdinal("status");
                                int claimedIndex = reader.GetOrdinal("claimed");

                                bool isDeadline = false;

                                if (DateTime.Compare(Convert.ToDateTime(reader.GetValue(deadlineIndex)), DateTime.Today) >= 0)
                                {
                                    if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed" && Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                                    {
                                        isDeadline = false;
                                    }
                                    else
                                    {
                                        isDeadline = true;
                                    }
                                }

                                customers.Add(new UserTransactionDataModel
                                {
                                    date = Convert.ToString(reader.GetValue(dateIndex)),
                                    deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                    isDeadline = isDeadline,
                                    jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)),
                                    service = Convert.ToString(reader.GetValue(serviceIndex)),
                                    customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                    issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                    address = Convert.ToString(reader.GetValue(addressIndex)),
                                    contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                    status = Convert.ToString(reader.GetValue(statusIndex)),
                                    claimed = Convert.ToString(reader.GetValue(claimedIndex)),
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                fillUpJobOrders();
            }
        }

        private void fillUpJobOrders()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where inaccessible = 1 and (service = 'Printing, Services, etc.' or service = 'Tarpaulin') ORDER BY TRY_CAST(deadline as date) ASC", conn))
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
                            int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");
                            int claimedIndex = reader.GetOrdinal("claimed");

                            bool isDeadline = false;

                            if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed" && Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                            {
                                isDeadline = false;
                            }
                            else
                            {
                                isDeadline = true;
                            }

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                isDeadline = isDeadline,
                                jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                                claimed = Convert.ToString(reader.GetValue(claimedIndex)),
                            });

                        }
                    }
                }
            }
        }

        private void BtnSearchJobOrder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtJobOrder.Text) || string.IsNullOrEmpty(cmbJobOrder.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo = @jobOrderNo and service = @service and inaccessible = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                    cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        customers.Clear();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int dateIndex = reader.GetOrdinal("date");
                                int deadlineIndex = reader.GetOrdinal("deadline");
                                int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                int serviceIndex = reader.GetOrdinal("service");
                                int customerNameIndex = reader.GetOrdinal("customerName");
                                int issuedByIndex = reader.GetOrdinal("issuedBy");
                                int addressIndex = reader.GetOrdinal("address");
                                int contactNoIndex = reader.GetOrdinal("contactNo");
                                int statusIndex = reader.GetOrdinal("status");
                                int claimedIndex = reader.GetOrdinal("claimed");

                                bool isDeadline = false;

                                if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed" && Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                                {
                                    isDeadline = false;
                                }
                                else
                                {
                                    isDeadline = true;
                                }

                                customers.Add(new UserTransactionDataModel
                                {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                isDeadline = isDeadline,
                                jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                                claimed = Convert.ToString(reader.GetValue(claimedIndex)),
                                });
                            }
                        }
                        else
                        {
                            MessageBox.Show("Job Order does not exist");
                            fillUpJobOrders();
                            return;
                        }
                    }
                }
            }
        }

        private void ChkCancelled_Checked(object sender, RoutedEventArgs e)
        {
            chkUnclaimed.IsChecked = false;
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where inaccessible = 0 and (service = 'Printing, Services, etc.' or service = 'Tarpaulin') ORDER BY customerName ASC", conn))
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
                            int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");
                            int claimedIndex = reader.GetOrdinal("claimed");

                            bool isDeadline = false;

                            if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed" && Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                            {
                                isDeadline = false;
                            }
                            else
                            {
                                isDeadline = true;
                            }

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                isDeadline = isDeadline,
                                jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                                claimed = Convert.ToString(reader.GetValue(claimedIndex)),
                            });

                        }
                    }
                }
            }
        }

        private void ChkCancelled_Unchecked(object sender, RoutedEventArgs e)
        {
            fillUpJobOrders();
        }

        private void ChkUnclaimed_Checked(object sender, RoutedEventArgs e)
        {
            chkCancelled.IsChecked = false;
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where inaccessible = 1 and (service = 'Printing, Services, etc.' or service = 'Tarpaulin') and claimed = 'Unclaimed'", conn))
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
                            int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");
                            int claimedIndex = reader.GetOrdinal("claimed");

                            bool isDeadline = false;

                            if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed" && Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                            {
                                isDeadline = false;
                            }
                            else
                            {
                                isDeadline = true;
                            }

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                isDeadline = isDeadline,
                                jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                                claimed = Convert.ToString(reader.GetValue(claimedIndex)),
                            });

                        }
                    }
                }
            }
        }

        private void ChkUnclaimed_Unchecked(object sender, RoutedEventArgs e)
        {
            fillUpJobOrders();
        }
    }
}
