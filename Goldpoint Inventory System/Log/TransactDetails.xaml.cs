using Goldpoint_Inventory_System.Transactions;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

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
        bool exist = false;

        public TransactDetails()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgService.ItemsSource = services;
            dgTransact.ItemsSource = stockOut;
            dgPaymentHistory.ItemsSource = payHist;
            dgPhotocopy.ItemsSource = photocopy;

        }

        private void BtnSearchService_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServiceNo.Text) || string.IsNullOrEmpty(cmbService.Text))
            {
                MessageBox.Show("Please fill up the fields before searching for the service");
            }
            else
            {
                //claim is missing?
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                string service = "";
                int drNo = 0;
                double unpaidBalance = 0;
                exPhotocopy.IsEnabled = false;
                exStockOut.IsEnabled = false;
                exJobOrder.IsEnabled = false;
                exJobOrderTarp.IsEnabled = false;
                switch (cmbService.Text)
                {
                    case "Official Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails WHERE ORNo = @serviceNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@serviceNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int deadlineIndex = reader.GetOrdinal("deadline");
                                        int custNameIndex = reader.GetOrdinal("customerName");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        int invoiceNoIndex = reader.GetOrdinal("invoiceNo");
                                        int drNoIndex = reader.GetOrdinal("DRNo");
                                        int statusIndex = reader.GetOrdinal("status");
                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        txtAddress.Document.Blocks.Add(new Paragraph(new Run(Convert.ToString(reader.GetValue(addressIndex)))));
                                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(invoiceNoIndex))))
                                        {
                                            chkInv.IsChecked = true;
                                            txtInvoiceNo.Text = Convert.ToString(reader.GetValue(invoiceNoIndex));
                                        }
                                        chkOR.IsChecked = true;
                                        chkDR.IsChecked = true;
                                        txtORNo.Text = txtServiceNo.Text;
                                        txtDRNo.Text = Convert.ToString(reader.GetValue(drNoIndex));

                                        if (Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                                        {
                                            rdPaid.IsChecked = true;
                                        }
                                        else
                                        {
                                            rdUnpaid.IsChecked = true;
                                        }


                                        service = Convert.ToString(reader.GetValue(serviceIndex));
                                        drNo = Convert.ToInt32(reader.GetValue(drNoIndex));
                                        exist = true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    return;
                                }

                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("SELECT * from PaymentHist where DRNo = @serviceNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@serviceNo", drNo);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                payHist.Clear();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int dateIndex = reader.GetOrdinal("date");
                                        int paidAmtIndex = reader.GetOrdinal("paidAmount");
                                        int totalIndex = reader.GetOrdinal("total");
                                        int statusIndex = reader.GetOrdinal("status");

                                        payHist.Add(new PaymentHistoryDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }
                        txtUnpaidBalance.Value = txtTotal.Value - unpaidBalance;
                        txtUnpaidBalancePayment.Value = txtUnpaidBalance.Value;

                        if (service == "Photocopy")
                        {
                            exPhotocopy.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from PhotocopyDetails WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    photocopy.Clear();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int itemIndex = reader.GetOrdinal("item");
                                            int priceIndex = reader.GetOrdinal("price");
                                            int qtyIndex = reader.GetOrdinal("qty");
                                            int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                            photocopy.Add(new PhotocopyDataModel
                                            {
                                                item = Convert.ToString(reader.GetValue(itemIndex)),
                                                price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Stock Out")
                        {
                            exStockOut.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ReleasedMaterials WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    stockOut.Clear();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int itemCodeIndex = reader.GetOrdinal("itemCode");
                                            int descriptionIndex = reader.GetOrdinal("description");
                                            int typeIndex = reader.GetOrdinal("type");
                                            int brandIndex = reader.GetOrdinal("brand");
                                            int sizeIndex = reader.GetOrdinal("size");
                                            int qtyIndex = reader.GetOrdinal("qty");
                                            int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                            stockOut.Add(new ItemDataModel
                                            {
                                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Job Order")
                        {
                            exJobOrder.IsEnabled = true;
                        }
                        else if (service == "Job Order (Tarpaulin)")
                        {
                            exJobOrderTarp.IsEnabled = true;
                        }

                        break;
                    case "Delivery Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails WHERE DRNo = @serviceNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@serviceNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int deadlineIndex = reader.GetOrdinal("deadline");
                                        int custNameIndex = reader.GetOrdinal("customerName");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNo = reader.GetOrdinal("contactNo");
                                        int remarks = reader.GetOrdinal("remarks");
                                        int invoiceNoIndex = reader.GetOrdinal("invoiceNo");
                                        int orNoIndex = reader.GetOrdinal("ORNo");
                                        int statusIndex = reader.GetOrdinal("status");
                                        int claimedIndex = reader.GetOrdinal("claimed");

                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                        textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(invoiceNoIndex))))
                                        {
                                            chkInv.IsChecked = true;
                                            txtInvoiceNo.Text = Convert.ToString(reader.GetValue(invoiceNoIndex));
                                        }
                                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(orNoIndex))))
                                        {
                                            chkOR.IsChecked = true;
                                            txtORNo.Text = Convert.ToString(reader.GetValue(orNoIndex));
                                        }
                                        chkDR.IsChecked = true;
                                        txtDRNo.Text = txtServiceNo.Text;

                                        if (Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                                        {
                                            rdPaid.IsChecked = true;
                                        }
                                        else
                                        {
                                            rdUnpaid.IsChecked = true;
                                        }

                                        if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed")
                                        {
                                            chkClaimed.IsChecked = true;
                                        }
                                        else
                                        {
                                            chkClaimed.IsChecked = false;
                                        }

                                        service = Convert.ToString(reader.GetValue(serviceIndex));
                                        drNo = Convert.ToInt32(txtServiceNo.Text);
                                        exist = true;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    return;
                                }
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("SELECT * from PaymentHist WHERE DRNo = @serviceNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@serviceNo", drNo);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                payHist.Clear();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int dateIndex = reader.GetOrdinal("date");
                                        int paidAmtIndex = reader.GetOrdinal("paidAmount");
                                        int totalIndex = reader.GetOrdinal("total");
                                        int statusIndex = reader.GetOrdinal("status");

                                        payHist.Add(new PaymentHistoryDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }
                        txtUnpaidBalance.Value = txtTotal.Value - unpaidBalance;
                        txtUnpaidBalancePayment.Value = txtUnpaidBalance.Value;

                        if (service == "Photocopy")
                        {
                            exPhotocopy.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from PhotocopyDetails WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    photocopy.Clear();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int itemIndex = reader.GetOrdinal("item");
                                            int priceIndex = reader.GetOrdinal("price");
                                            int qtyIndex = reader.GetOrdinal("qty");
                                            int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                            photocopy.Add(new PhotocopyDataModel
                                            {
                                                item = Convert.ToString(reader.GetValue(itemIndex)),
                                                price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Stock Out")
                        {
                            exStockOut.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ReleasedMaterials WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    stockOut.Clear();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int itemCodeIndex = reader.GetOrdinal("itemCode");
                                            int descriptionIndex = reader.GetOrdinal("description");
                                            int typeIndex = reader.GetOrdinal("type");
                                            int brandIndex = reader.GetOrdinal("brand");
                                            int sizeIndex = reader.GetOrdinal("size");
                                            int qtyIndex = reader.GetOrdinal("qty");
                                            int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                            stockOut.Add(new ItemDataModel
                                            {
                                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Job Order")
                        {
                            exJobOrder.IsEnabled = true;
                        }
                        else if (service == "Job Order (Tarpaulin)")
                        {
                            exJobOrderTarp.IsEnabled = true;
                        }
                        break;
                    case "Invoice":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails WHERE invoiceNo = @serviceNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@serviceNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int serviceIndex = reader.GetOrdinal("service");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int deadlineIndex = reader.GetOrdinal("deadline");
                                        int custNameIndex = reader.GetOrdinal("customerName");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNo = reader.GetOrdinal("contactNo");
                                        int remarks = reader.GetOrdinal("remarks");
                                        int orNoIndex = reader.GetOrdinal("ORNo");
                                        int drNoIndex = reader.GetOrdinal("DRNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        txtAddress.Document.Blocks.Add(new Paragraph(new Run(Convert.ToString(reader.GetValue(addressIndex)))));
                                        txtDRNo.Text = Convert.ToString(reader.GetValue(drNoIndex));
                                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(orNoIndex))))
                                        {
                                            chkOR.IsChecked = true;
                                            txtORNo.Text = Convert.ToString(reader.GetValue(orNoIndex));
                                        }
                                        chkDR.IsChecked = true;
                                        txtInvoiceNo.Text = txtServiceNo.Text;

                                        if (Convert.ToString(reader.GetValue(statusIndex)) == "Paid")
                                        {
                                            rdPaid.IsChecked = true;
                                        }
                                        else
                                        {
                                            rdUnpaid.IsChecked = true;
                                        }

                                        service = Convert.ToString(reader.GetValue(serviceIndex));
                                        drNo = Convert.ToInt32(reader.GetValue(drNoIndex));
                                        exist = true;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    return;
                                }
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("SELECT * from PaymentHist WHERE DRNo = @serviceNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@serviceNo", drNo);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                payHist.Clear();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int dateIndex = reader.GetOrdinal("date");
                                        int paidAmtIndex = reader.GetOrdinal("paidAmount");
                                        int totalIndex = reader.GetOrdinal("total");
                                        int statusIndex = reader.GetOrdinal("status");

                                        payHist.Add(new PaymentHistoryDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }
                        txtUnpaidBalance.Value = txtTotal.Value - unpaidBalance;
                        txtUnpaidBalancePayment.Value = txtUnpaidBalance.Value;

                        if (service == "Photocopy")
                        {
                            exPhotocopy.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from PhotocopyDetails WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    photocopy.Clear();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int itemIndex = reader.GetOrdinal("item");
                                            int priceIndex = reader.GetOrdinal("price");
                                            int qtyIndex = reader.GetOrdinal("qty");
                                            int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                            photocopy.Add(new PhotocopyDataModel
                                            {
                                                item = Convert.ToString(reader.GetValue(itemIndex)),
                                                price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Stock Out")
                        {
                            exStockOut.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ReleasedMaterials WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    stockOut.Clear();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            int itemCodeIndex = reader.GetOrdinal("itemCode");
                                            int descriptionIndex = reader.GetOrdinal("description");
                                            int typeIndex = reader.GetOrdinal("type");
                                            int brandIndex = reader.GetOrdinal("brand");
                                            int sizeIndex = reader.GetOrdinal("size");
                                            int qtyIndex = reader.GetOrdinal("qty");
                                            int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                            stockOut.Add(new ItemDataModel
                                            {
                                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Job Order")
                        {
                            exJobOrder.IsEnabled = true;
                        }
                        else if (service == "Job Order (Tarpaulin)")
                        {
                            exJobOrderTarp.IsEnabled = true;
                        }

                        break;
                    case "Job Order (Tarpaulin)":

                        break;
                    case "Job Order (Printing, Services, etc.)":

                        break;
                }

            }
        }
        private void emptyFields()
        {
            txtAddress.Document.Blocks.Clear();
            txtAmount.Value = 0;
            txtCustName.Text = null;
            txtDate.Text = DateTime.Today.ToShortDateString();
            txtDatePayment.Text = DateTime.Today.ToShortDateString();
            txtDRNo.Text = null;
            txtInvoiceNo.Text = null;
            txtORNo.Text = null;
            txtServiceNo.Text = null;
            txtTotal.Value = 0;
            txtUnpaidBalance.Value = 0;
            txtUnpaidBalancePayment.Value = 0;
            chkClaimed.IsChecked = false;
            chkDR.IsChecked = false;
            chkInv.IsChecked = false;
            chkOR.IsChecked = false;
        }

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            photocopy.Clear();
            stockOut.Clear();
            payHist.Clear();
            services.Clear();
            exPhotocopy.IsEnabled = false;
            exStockOut.IsEnabled = false;
            exJobOrder.IsEnabled = false;
            exJobOrderTarp.IsEnabled = false;
            exist = false;
        }

        private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAmount.Value > txtUnpaidBalancePayment.Value)
            {
                txtAmount.Value = txtUnpaidBalancePayment.Value;
            }
        }

        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            if (exist == true)
            {
                if (txtAmount.Value == 0 || string.IsNullOrEmpty(txtAmount.Text))
                {
                    MessageBox.Show("Please enter amount greater than 0 to process payment");
                }
                else
                {
                    //if already fully paid, should disable the button
                    string sMessageBoxText = "Confirming payment";
                    string sCaption = "Update payment history?";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            SqlConnection conn = DBUtils.GetDBConnection();
                            conn.Open();
                            //if fully paid or not, update all if fully paid, if not, normal add
                            using (SqlCommand cmd = new SqlCommand("INSERT into PaymentHist (@DRNo, @date, @paidAmount, @total)", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@date", txtDatePayment.Text);
                                cmd.Parameters.AddWithValue("@paidAmount", txtUnpaidBalancePayment.Value);
                                cmd.Parameters.AddWithValue("@total", txtTotal.Value);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Payment history updated");
                                    payHist.Add(new PaymentHistoryDataModel
                                    {
                                        date = txtDatePayment.Text,
                                        amount = (double) txtUnpaidBalancePayment.Value
                                    });
                                    txtUnpaidBalance.Value -= txtUnpaidBalancePayment.Value;
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! " + ex);
                                }

                            }
                            break;
                        case MessageBoxResult.No:
                            return;
                        case MessageBoxResult.Cancel:
                            return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please search for the transaction first.");
            }

        }

        private void BtnClaiming_Click(object sender, RoutedEventArgs e)
        {
            if (exist == true)
            {
                //if already claimed, should disable the button
                string sMessageBoxText = "Confirming claiming of materials/supplies";
                string sCaption = "Update transaction?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE TransactionDetails set claimed = 'Claimed' where DRNo = @DRNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Transaction updated");
                                chkClaimed.IsChecked = true;
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! " + ex);
                            }
                        }
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else
            {
                MessageBox.Show("Please search for the transaction first.");
            }
        }

    }
}
