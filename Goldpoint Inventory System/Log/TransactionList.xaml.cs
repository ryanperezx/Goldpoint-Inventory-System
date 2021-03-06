﻿using System;
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
            try
            {
                InitializeComponent();
                dgTransactions.ItemsSource = customers;
                dgTransactions.RowHeight = 40;

                fillUpTransactions();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void TxtCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustomerName.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.contactNo, td.issuedBy, td.address, td.status, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on (ph.receiptNo = td.DRNo or (ph.receiptNo = td.jobOrderNo AND ph.service = td.service)) where td.customerName LIKE @custName and td.inaccessible = 1 ORDER BY td.customerName ASC", conn))
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
                                string drNo = null;
                                if (reader.GetValue(drNoindex) == DBNull.Value)
                                    drNo = "";
                                else
                                    drNo = Convert.ToString(reader.GetValue(drNoindex));
                                int totalIndex = reader.GetOrdinal("total");
                                int serviceIndex = reader.GetOrdinal("service");
                                int customerNameIndex = reader.GetOrdinal("customerName");
                                int issuedByIndex = reader.GetOrdinal("issuedBy");
                                int addressIndex = reader.GetOrdinal("address");
                                int contactNoIndex = reader.GetOrdinal("contactNo");
                                int statusIndex = reader.GetOrdinal("status");

                                customers.Add(new UserTransactionDataModel
                                {
                                    date = Convert.ToString(reader.GetValue(dateIndex)),
                                    deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                    drNo = drNo,
                                    service = Convert.ToString(reader.GetValue(serviceIndex)),
                                    customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                    issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                    total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                    address = Convert.ToString(reader.GetValue(addressIndex)),
                                    contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                    status = Convert.ToString(reader.GetValue(statusIndex)),
                                });
                            }
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
                        using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.contactNo, td.issuedBy, td.address, td.status, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on (ph.receiptNo = td.DRNo or (ph.receiptNo = td.jobOrderNo AND ph.service = td.service)) where td.drNo = @drNo and td.inaccessible = 1", conn))
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
                                        string drNo = null;
                                        if (reader.GetValue(drNoindex) == DBNull.Value)
                                            drNo = "";
                                        else
                                            drNo = Convert.ToString(reader.GetValue(drNoindex));
                                        int totalIndex = reader.GetOrdinal("total");
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int customerNameIndex = reader.GetOrdinal("customerName");
                                        int issuedByIndex = reader.GetOrdinal("issuedBy");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        customers.Add(new UserTransactionDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                            drNo = drNo,
                                            service = Convert.ToString(reader.GetValue(serviceIndex)),
                                            customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                            total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                            issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
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
                                }
                            }
                        }

                        break;
                    case "Original Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.contactNo, td.issuedBy, td.address, td.status, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on (ph.receiptNo = td.DRNo or (ph.receiptNo = td.jobOrderNo AND ph.service = td.service)) where td.orNo = @orNo and td.inaccessible = 1", conn))
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
                                        string drNo = null;
                                        if (reader.GetValue(drNoindex) == DBNull.Value)
                                            drNo = "";
                                        else
                                            drNo = Convert.ToString(reader.GetValue(drNoindex));
                                        int totalIndex = reader.GetOrdinal("total");
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int customerNameIndex = reader.GetOrdinal("customerName");
                                        int issuedByIndex = reader.GetOrdinal("issuedBy");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        customers.Add(new UserTransactionDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                            drNo = drNo,
                                            service = Convert.ToString(reader.GetValue(serviceIndex)),
                                            customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                            total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                            issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
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
                                }
                            }
                        }

                        break;
                    case "Invoice":
                        using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.contactNo, td.issuedBy, td.address, td.status, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on (ph.receiptNo = td.DRNo or (ph.receiptNo = td.jobOrderNo AND ph.service = td.service)) where td.invoiceNo = @invoiceNo and td.inaccessible = 1", conn))
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
                                        string drNo = null;
                                        if (reader.GetValue(drNoindex) == DBNull.Value)
                                            drNo = "";
                                        else
                                            drNo = Convert.ToString(reader.GetValue(drNoindex));
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int customerNameIndex = reader.GetOrdinal("customerName");
                                        int totalIndex = reader.GetOrdinal("total");
                                        int issuedByIndex = reader.GetOrdinal("issuedBy");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        customers.Add(new UserTransactionDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                            drNo = drNo,
                                            service = Convert.ToString(reader.GetValue(serviceIndex)),
                                            customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                            total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                            issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
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

            using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.contactNo, td.issuedBy, td.address, td.status, ph.total FROM TransactionDetails td JOIN(SELECT receiptNo, service, total from PaymentHist GROUP by receiptNo, service, total) ph on(td.DRNo = ph.receiptNo or(td.jobOrderNo = ph.receiptNo AND ph.service = td.service)) where td.inaccessible = 1 ORDER BY CAST(td.date as date) ASC", conn))
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
                            string drNo = null;
                            if (reader.GetValue(drNoindex) == DBNull.Value)
                                drNo = "";
                            else
                                drNo = Convert.ToString(reader.GetValue(drNoindex));
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int totalIndex = reader.GetOrdinal("total");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                drNo = drNo,
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                            });
                        }
                    }
                }
            }
        }

        private void ChkCompany_Checked(object sender, RoutedEventArgs e)
        {
            chkUnpaid.IsChecked = false;
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.issuedBy, td.contactNo, td.address, td.status, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on (ph.receiptNo = td.DRNo or (ph.receiptNo = td.jobOrderNo AND ph.service = td.service)) where td.address = 'N\\A' and td.contactNo = 'N\\A' and td.deadline = 'N\\A'", conn))
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
                            string drNo = null;
                            if (reader.GetValue(drNoindex) == DBNull.Value)
                                drNo = "";
                            else
                                drNo = Convert.ToString(reader.GetValue(drNoindex));
                            int totalIndex = reader.GetOrdinal("total");
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                drNo = drNo,
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                            });
                        }
                    }
                }
            }
        }

        private void ChkCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            fillUpTransactions();
        }

        private void ChkUnpaid_Checked(object sender, RoutedEventArgs e)
        {
            chkCompany.IsChecked = false;
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.contactNo, td.issuedBy, td.address, td.status, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on (ph.receiptNo = td.DRNo or (ph.receiptNo = td.jobOrderNo AND ph.service = td.service)) where td.inaccessible = 1 and td.status = 'Unpaid'", conn))
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
                            string drNo = null;
                            if (reader.GetValue(drNoindex) == DBNull.Value)
                                drNo = "";
                            else
                                drNo = Convert.ToString(reader.GetValue(drNoindex));
                            int totalIndex = reader.GetOrdinal("total");
                            int serviceIndex = reader.GetOrdinal("service");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int statusIndex = reader.GetOrdinal("status");

                            customers.Add(new UserTransactionDataModel
                            {
                                date = Convert.ToString(reader.GetValue(dateIndex)),
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                drNo = drNo,
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                customerName = Convert.ToString(reader.GetValue(customerNameIndex)),
                                total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex)),
                                address = Convert.ToString(reader.GetValue(addressIndex)),
                                contactNo = Convert.ToString(reader.GetValue(contactNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                            });
                        }
                    }
                }
            }
        }

        private void ChkUnpaid_Unchecked(object sender, RoutedEventArgs e)
        {
            fillUpTransactions();
        }
    }
}
