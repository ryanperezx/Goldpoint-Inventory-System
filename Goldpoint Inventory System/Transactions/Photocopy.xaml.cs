﻿using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Linq;
using NLog;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for Photocopy.xaml
    /// </summary>
    public partial class Photocopy : UserControl
    {
        ObservableCollection<PhotocopyDataModel> items = new ObservableCollection<PhotocopyDataModel>();
        private static Logger Log = LogManager.GetCurrentClassLogger();

        double total = 0;
        public Photocopy()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgPhotocopy.ItemsSource = items;
            getDRNo();
            chkUnpaid.IsChecked = true;
        }

        private void getDRNo()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 DRNo from TransactionDetails WHERE DRNo is not null AND DATALENGTH(DRNo) > 0 ORDER BY DRNo DESC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        txtDRNo.Text = "1";
                    else
                    {
                        int DRNoIndex = reader.GetOrdinal("DRNo");
                        int DRNo = Convert.ToInt32(reader.GetValue(DRNoIndex)) + 1;
                        txtDRNo.Text = DRNo.ToString();

                    }
                }
            }
        }
        private void getORNo()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 ORNo from TransactionDetails WHERE ORNo is not null AND DATALENGTH(ORNo) > 0  ORDER BY ORNo DESC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        txtORNo.Text = "1";
                    else
                    {
                        int ORNoIndex = reader.GetOrdinal("ORNo");
                        int ORNo = Convert.ToInt32(reader.GetValue(ORNoIndex)) + 1;
                        txtORNo.Text = ORNo.ToString();

                    }
                }
            }
        }
        private void getInvNo()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 InvoiceNo from TransactionDetails WHERE invoiceNo is not null AND DATALENGTH(invoiceNo) > 0 ORDER BY InvoiceNo DESC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        txtInvoice.Text = "1";
                    else
                    {
                        int invoiceNoIndex = reader.GetOrdinal("InvoiceNo");
                        int invoiceNo = Convert.ToInt32(reader.GetValue(invoiceNoIndex)) + 1;
                        txtInvoice.Text = invoiceNo.ToString();
                    }
                }
            }
        }

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            items.Clear();
            getDRNo();
        }

        private void emptyFields()
        {
            txtShort.Text = null;
            txtLong.Text = null;
            txtLegal.Text = null;
            txtA4.Text = null;
            txtA3.Text = null;
            txtItemTotal.Text = null;

            txtCustName.Text = null;
            txtCustTotal.Text = null;
            txtAddress.Document.Blocks.Clear();
            txtContactNo.Text = null;
            txtDownpayment.Text = null;
            txtDate.Text = DateTime.Today.ToShortDateString();
            txtRemarks.Text = null;

            chkInv.IsChecked = false;
            chkOR.IsChecked = false;
            total = 0;
        }

        private void chkServ_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            string value = chkBox.Content.ToString();
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            switch (value)
            {
                case "Official Receipt":
                    getORNo();
                    break;
                case "Invoice":
                    getInvNo();
                    break;
            }

        }

        private void chkServ_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            string value = chkBox.Content.ToString(); switch (value)
            {
                case "Delivery Receipt":
                    txtDRNo.Text = null;
                    break;
                case "Official Receipt":
                    txtORNo.Text = null;
                    break;
                case "Invoice":
                    txtInvoice.Text = null;
                    break;
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

        private void BtnAddtoList_Click(object sender, RoutedEventArgs e)
        {
            bool isEmpty = true;
            //does not update display but does work
            if (!string.IsNullOrEmpty(txtShort.Text) && txtShort.Value != 0)
            {
                var found = items.FirstOrDefault(x =>  x.item.Equals("Short") && x.qty > 0);
                if(found != null)
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Short",
                        qty = Convert.ToInt32(found.qty + txtShort.Value),
                        price = 0.70,
                        totalPerItem = Convert.ToInt64(found.qty + txtShort.Value) * 0.70
                    });

                    foreach (var item in items.Where(x => x.item.Equals("Short")).ToList())
                    {
                        items.Remove(item);
                        break;
                    }
                }
                else
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Short",
                        qty = Convert.ToInt32(txtShort.Value),
                        price = 0.70,
                        totalPerItem = Convert.ToInt64(txtShort.Value) * 0.70
                    });
                }
                total += Convert.ToInt64(txtShort.Value) * 0.70;
                isEmpty = false;
            }
            if (!string.IsNullOrEmpty(txtLong.Text) && txtLong.Value != 0)
            {
                var found = items.FirstOrDefault(x => x.item.Equals("Long") && x.qty > 0);
                if (found != null)
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Long",
                        qty = Convert.ToInt32(found.qty + txtLong.Value),
                        price = 0.80,
                        totalPerItem = Convert.ToInt64(found.qty + txtLong.Value) * 0.80
                    });
                }
                else
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Long",
                        qty = Convert.ToInt32(txtLong.Value),
                        price = 0.80,
                        totalPerItem = Convert.ToInt64(txtLong.Value) * 0.80
                    });
                }

                total += Convert.ToInt64(txtLong.Value) * 0.80;
                isEmpty = false;

            }
            if (!string.IsNullOrEmpty(txtLegal.Text) && txtLegal.Value != 0)
            {
                var found = items.FirstOrDefault(x => x.item.Equals("Legal") && x.qty > 0);
                if (found != null)
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Legal",
                        qty = Convert.ToInt32(found.qty + txtLegal.Value),
                        price = 1.50,
                        totalPerItem = Convert.ToInt64(found.qty + txtLegal.Value) * 1.50
                    });

                    foreach (var item in items.Where(x => x.item.Equals("Legal")).ToList())
                    {
                        items.Remove(item);
                        break;
                    }
                }
                else
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Legal",
                        qty = Convert.ToInt32(txtLegal.Value),
                        price = 1.50,
                        totalPerItem = Convert.ToInt64(txtLegal.Value) * 1.50
                    });
                }

                total += Convert.ToInt64(txtLegal.Value) * 1.50;
                isEmpty = false;
            }
            if (!string.IsNullOrEmpty(txtA4.Text) && txtA4.Value != 0)
            {
                var found = items.FirstOrDefault(x => x.item.Equals("A4") && x.qty > 0);
                if (found != null)
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "A4",
                        qty = Convert.ToInt32(found.qty + txtA4.Value),
                        price = 0.90,
                        totalPerItem = Convert.ToInt64(found.qty + txtLegal.Value) * 0.90
                    });

                    foreach (var item in items.Where(x => x.item.Equals("A4")).ToList())
                    {
                        items.Remove(item);
                        break;
                    }
                }
                else
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "A4",
                        qty = Convert.ToInt32(txtA4.Value),
                        price = 0.90,
                        totalPerItem = Convert.ToInt64(txtLegal.Value) * 0.90
                    });
                }
                total += Convert.ToInt64(txtA4.Value) * 0.90;
                isEmpty = false;
            }
            if (!string.IsNullOrEmpty(txtA3.Text) && txtA3.Value != 0)
            {
                var found = items.FirstOrDefault(x => x.item.Equals("A3") && x.qty > 0);
                if (found != null)
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "A3",
                        qty = Convert.ToInt32(found.qty + txtA3.Value),
                        price = 0.90,
                        totalPerItem = Convert.ToInt64(found.qty + txtA3.Value) * 5.00
                    });

                    foreach (var item in items.Where(x => x.item.Equals("A3")).ToList())
                    {
                        items.Remove(item);
                        break;
                    }
                }
                else
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "A3",
                        qty = Convert.ToInt32(txtA3.Value),
                        price = 0.90,
                        totalPerItem = Convert.ToInt64(txtA3.Value) * 5.00
                    });
                }
                total += Convert.ToInt64(txtA3.Value) * 5.00;
                isEmpty = false;
            }
            if (isEmpty)
            {
                MessageBox.Show("There is nothing to add to list");
            }
            else
            {
                txtShort.Value = 0;
                txtLong.Value = 0;
                txtLegal.Value = 0;
                txtA4.Value = 0;
                txtA3.Value = 0;
                txtCustTotal.Value = total;
                txtItemTotal.Value = total;
            }

        }

        private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            string address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd).Text;

            if (items.Count == 0)
            {
                MessageBox.Show("The list of item to be photocopied is empty!");
            }
            else if (string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtCustName.Text) || string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(address))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (chkDownpayment.IsChecked == true && string.IsNullOrEmpty(txtDownpayment.Text))
            {
                MessageBox.Show("Please input downpayment to be able to proceed");
            }
            else if (chkDownpayment.IsChecked == true && txtDownpayment.Value == 0)
            {
                MessageBox.Show("Please set downpayment to anything greater than zero.");
            }
            else
            {
                string sMessageBoxText = "Confirming Photocopy transaction";
                string sCaption = "Confirm Transaction?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        bool success = false;
                        //update DRNo in case Stock out or Job order updated it
                        getDRNo();
                        if(chkInv.IsChecked == true)
                        {
                            getInvNo();
                        }
                        if(chkOR.IsChecked == true)
                        {
                            getORNo();
                        }
                        foreach (var item in items)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into PhotocopyDetails VALUES (@DRNo, @item, @price, @qty, @totalPerItem)", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@item", item.item);
                                cmd.Parameters.AddWithValue("@qty", item.qty);
                                cmd.Parameters.AddWithValue("@price", item.price);
                                cmd.Parameters.AddWithValue("@totalPerItem", item.totalPerItem);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    success = true;
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                }
                            }
                        }
                        if (chkDownpayment.IsChecked == true)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @service, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@service", "Photocopy");
                                cmd.Parameters.AddWithValue("@total", txtDownpayment.Value);
                                cmd.Parameters.AddWithValue("@status", "Downpayment");
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                    success = false;
                                }
                            }
                        }
                        else if(chkPaid.IsChecked == true)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @service, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@service", "Photocopy");
                                cmd.Parameters.AddWithValue("@total", txtCustTotal.Value);
                                cmd.Parameters.AddWithValue("@status", "Paid");
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
                                    success = false;
                                }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with D.R No: " + txtDRNo.Text + ", had a photocopy transaction amounting to Php " + txtCustTotal.Text);
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex, "Query Error");
                                success = false;
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT into PaymentHist VALUES (@DRNo, @date, @paidAmt, @total, @status)", conn))
                        {
                            cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@total", txtCustTotal.Value);
                            if (chkPaid.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@paidAmt", txtCustTotal.Value);
                                cmd.Parameters.AddWithValue("@status", "Paid");
                            }
                            if (chkUnpaid.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@paidAmt", 0);
                                cmd.Parameters.AddWithValue("@status", "Unpaid");
                            }
                            if (chkDownpayment.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@paidAmt", txtDownpayment.Value);
                                cmd.Parameters.AddWithValue("@status", "Downpayment");
                            }
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex, "Query Error");
                                success = false;
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (DRNo, service, date, deadline, customerName, address, contactNo, remarks, ORNo, invoiceNo, status, claimed, inaccessible) VALUES (@DRNo, @service, @date, @deadline, @customerName, @address, @contactNo, @remarks, @ORNo, @InvoiceNo, @status, @claimed, 1)", conn))
                        {
                            cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@service", "Photocopy");
                            cmd.Parameters.AddWithValue("@deadline", "N\\A");
                            cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@ORNo", txtORNo.Text);
                            cmd.Parameters.AddWithValue("@InvoiceNo", txtInvoice.Text);

                            if (chkPaid.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@status", "Paid");
                                cmd.Parameters.AddWithValue("@claimed", "Claimed");

                            }
                            if (chkUnpaid.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@status", "Unpaid");
                                cmd.Parameters.AddWithValue("@claimed", "Claimed");

                            }
                            if (chkDownpayment.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@status", "Downpayment");
                                cmd.Parameters.AddWithValue("@claimed", "Claimed");

                            }
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex, "Query Error");
                                success = false;

                            }
                        }
                        if (success)
                            MessageBox.Show("Transaction has been recorded!");
                        emptyFields();
                        items.Clear();
                        getDRNo();
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void TxtDownpayment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDownpayment.Text) && !string.IsNullOrEmpty(txtCustTotal.Text))
            {
                double downpayment = (double) txtDownpayment.Value;
                double total = (double) txtCustTotal.Value;
                if (downpayment > total)
                {
                    txtDownpayment.Text = total.ToString();
                }
            }

        }

        private void BtnRemoveLastItem_Click(object sender, RoutedEventArgs e)
        {
            if (items.Count == 0)
            {
                MessageBox.Show("Item list is empty");
            }
            else
            {
                //deduct to total
                var last = items.Last();
                txtCustTotal.Value -= last.totalPerItem;
                txtItemTotal.Value -= last.totalPerItem;
                items.RemoveAt(items.Count - 1);
            }
        }
    }
}
