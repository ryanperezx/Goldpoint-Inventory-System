using NLog;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for StockOut.xaml
    /// </summary>
    public partial class StockOut : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        ObservableCollection<PhotocopyDataModel> photocopy = new ObservableCollection<PhotocopyDataModel>();

        private static Logger Log = LogManager.GetCurrentClassLogger();
        string adminLevel;
        public StockOut(string fullName, string adminLevel)
        {
            InitializeComponent();
            txtIssuedBy.Text = fullName;
            stack.DataContext = new ExpanderListViewModel();
            dgStockOut.ItemsSource = items;
            //to avoid null error
            txtQty.TextChanged += TxtQty_TextChanged;
            txtDiscount.TextChanged += TxtDiscount_TextChanged;
            dgPhotocopy.ItemsSource = photocopy;
            getDRNo();
            rdUnpaid.IsChecked = true;
            this.adminLevel = adminLevel;
            checkAdminLevel();

        }

        private void checkAdminLevel()
        {
            if (adminLevel == "Administrator")
            {
                txtDiscount.IsEnabled = true;
            }
            else
            {
                txtDiscount.IsEnabled = false;
            }
        }

        private void radiobuttonPayment(object sender, RoutedEventArgs e)
        {
            RadioButton radiobtn = (RadioButton)sender;
            string value = radiobtn.Content.ToString();
            switch (value)
            {
                case "Unpaid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    txtDiscount.Value = 0;
                    checkAdminLevel();
                    break;
                case "Downpayment":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = true;
                    checkAdminLevel();
                     break;
                case "Paid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    checkAdminLevel();
                    break;
                case "Company Use":
                    break;
            }
        }

        private void checkboxService(object sender, RoutedEventArgs e)
        {
            if (chkCompany.IsChecked == true)
            {
                chkInv.IsChecked = false;
                chkOR.IsChecked = false;

                txtInv.Text = null;
                txtORNo.Text = null;

                rdPaid.IsChecked = true;
                rdUnpaid.IsEnabled = false;
                rdDownpayment.IsEnabled = false;
                txtDiscount.Value = 0;
                txtDiscount.IsEnabled = false;
                txtAddress.IsEnabled = false;
                txtAddress.Document.Blocks.Clear();
                txtContactNo.IsEnabled = false;
                txtContactNo.Text = null;

            }
            else
            {
                CheckBox chkbox = (CheckBox)sender;
                string value = chkbox.Content.ToString();

                getDRNo();
                chkDR.IsChecked = true;
                txtAddress.IsEnabled = true;
                txtContactNo.IsEnabled = true;
                rdUnpaid.IsEnabled = true;
                rdDownpayment.IsEnabled = true;
                checkAdminLevel();

                if (chkbox.IsChecked == true && value == "Original Receipt")
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
                if (chkbox.IsChecked == true && value == "Invoice")
                {
                    SqlConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 InvoiceNo from TransactionDetails WHERE invoiceNo is not null AND DATALENGTH(invoiceNo) > 0 ORDER BY InvoiceNo DESC", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                txtInv.Text = "1";
                            else
                            {
                                int invoiceNoIndex = reader.GetOrdinal("InvoiceNo");
                                int invoiceNo = Convert.ToInt32(reader.GetValue(invoiceNoIndex)) + 1;
                                txtInv.Text = invoiceNo.ToString();
                            }
                        }
                    }
                }

            }
        }

        private void unCheckBoxService(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            string value = chkbox.Content.ToString();

            if (chkbox.IsChecked == false && value == "Invoice")
            {
                txtInv.Text = null;
            }
            if (chkbox.IsChecked == false && value == "Delivery Receipt")
            {
                txtDRNo.Text = null;
            }
            if (chkbox.IsChecked == false && value == "Original Receipt")
            {
                txtORNo.Text = null;
            }
            if (chkCompany.IsChecked == false)
            {
                txtAddress.IsEnabled = true;
                txtContactNo.IsEnabled = true;
                rdUnpaid.IsEnabled = true;
                rdDownpayment.IsEnabled = true;
                checkAdminLevel();

            }
        }

        private void BtnAddtoList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text) || string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (txtQty.Value == 0)
            {
                MessageBox.Show("Please set quantity to any greater than zero");
                txtQty.Focus();
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {

                            reader.Read();
                            int descriptionIndex = reader.GetOrdinal("description");
                            int typeIndex = reader.GetOrdinal("type");
                            int brandIndex = reader.GetOrdinal("brand");
                            int sizeIndex = reader.GetOrdinal("size");
                            int qtyIndex = reader.GetOrdinal("qty");
                            if (Convert.ToInt32(reader.GetValue(qtyIndex)) < txtQty.Value)
                            {
                                MessageBox.Show("Quantity to be stock out is greater than the stock quantity");
                                return;
                            }

                            var found = items.FirstOrDefault(x => txtItemCode.Text == x.itemCode);
                            if (found != null)
                            {
                                items.Add(new ItemDataModel
                                {
                                    itemCode = txtItemCode.Text,
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    type = Convert.ToString(reader.GetValue(typeIndex)),
                                    brand = Convert.ToString(reader.GetValue(brandIndex)),
                                    size = Convert.ToString(reader.GetValue(sizeIndex)),
                                    qty = Convert.ToInt32(found.qty + txtQty.Value),
                                    totalPerItem = Convert.ToDouble(found.totalPerItem + txtTotalPerItem.Value),
                                    remarks = txtRemarks.Text
                                });

                                foreach (var item in items.Where(x => txtItemCode.Text == x.itemCode).ToList())
                                {
                                    items.Remove(item);
                                    break;
                                }
                            }
                            else
                            {
                                items.Add(new ItemDataModel
                                {
                                    itemCode = txtItemCode.Text,
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    type = Convert.ToString(reader.GetValue(typeIndex)),
                                    brand = Convert.ToString(reader.GetValue(brandIndex)),
                                    size = Convert.ToString(reader.GetValue(sizeIndex)),
                                    qty = (int)txtQty.Value,
                                    totalPerItem = (double)txtTotalPerItem.Value,
                                    remarks = txtRemarks.Text
                                });

                            }
                            txtTotal.Value += txtTotalPerItem.Value;
                            txtDownpayment.MaxValue = (double)txtTotal.Value - 1;
                            txtDiscount.MaxValue = (double)txtTotal.Value - 1;
                            txtQty.Value = 0;
                            txtItemPrice.Value = 0;
                            ckDealersPrice.IsChecked = false;

                            txtItemCode.Text = null;
                            txtDesc.Text = null;
                            txtCustName.Text = null;
                            txtBrand.Text = null;
                            txtQty.Value = 0;
                            txtSize.Text = null;
                            txtRemarks.Text = null;
                            txtItemPrice.Value = 0;
                            txtTotalPerItem.Value = 0;
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }
                }
            }
        }
        private void BtnSearchItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text))
            {
                MessageBox.Show("Please type the item code before searching");
                txtItemCode.Focus();
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {

                            reader.Read();
                            int descriptionIndex = reader.GetOrdinal("description");
                            txtDesc.Text = Convert.ToString(reader.GetValue(descriptionIndex));

                            int typeIndex = reader.GetOrdinal("type");
                            txtType.Text = Convert.ToString(reader.GetValue(typeIndex));

                            int brandIndex = reader.GetOrdinal("brand");
                            txtBrand.Text = Convert.ToString(reader.GetValue(brandIndex));

                            txtQty.Value = 0;

                            int sizeIndex = reader.GetOrdinal("size");
                            txtSize.Text = Convert.ToString(reader.GetValue(sizeIndex));

                            int msrpIndex = reader.GetOrdinal("MSRP");
                            txtItemPrice.Value = Convert.ToDouble(reader.GetValue(msrpIndex));

                            ckDealersPrice.IsChecked = false;
                            searched = true;
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }

                }
            }
        }
        private void TxtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtItemPrice.Value > 0 && txtQty.Value > 0)
            {
                txtTotalPerItem.Value = txtItemPrice.Value * txtQty.Value;
            }
            else
            {
                txtTotalPerItem.Value = 0;
            }
        }
        private void BtnReset_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            items.Clear();
            photocopy.Clear();
            emptyFields();
        }
        private void emptyFields()
        {
            txtItemCode.Text = null;
            txtDesc.Text = null;
            txtRemarks.Text = null;
            txtCustName.Text = null;
            txtAddress.Document.Blocks.Clear();
            txtContactNo.Text = null;
            txtDiscount.Value = 0;
            txtDownpayment.Value = 0;
            txtDownpayment.MaxValue = 0;
            txtDiscount.Value = 0;
            txtDiscount.MaxValue = 0;
            txtDRNo.Text = null;
            txtInv.Text = null;
            txtItemPrice.Value = 0;
            txtORNo.Text = null;
            txtBrand.Text = null;
            txtQty.Value = 0;
            txtSize.Text = null;
            txtTotal.Value = 0;
            txtTotalPerItem.Value = 0;
            txtTransactRemarks.Document.Blocks.Clear();
            txtType.Text = null;
            getDRNo();

            chkInv.IsChecked = false;
            chkOR.IsChecked = false;
            chkCompany.IsChecked = false;
            ckDealersPrice.IsChecked = false;
            searched = false;

            txtShort.Value = null;
            txtLong.Value = null;
            txtLegal.Value = null;
            txtA4.Value = null;
            txtA3.Value = null;
        }

        private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            string address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd).Text;
            string remarks = new TextRange(txtTransactRemarks.Document.ContentStart, txtTransactRemarks.Document.ContentEnd).Text;

            if (items.Count == 0 && photocopy.Count == 0)
            {
                MessageBox.Show("Item list is empty");
            }
            else if (string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtCustName.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (chkCompany.IsChecked == false && (string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(txtIssuedBy.Text)))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (rdDownpayment.IsChecked == true && txtDownpayment.Value == 0)
            {
                MessageBox.Show("Please set downpayment to anything greater than zero.");
            }
            else
            {
                string sMessageBoxText = "Confirming Transaction";
                string sCaption = "Confirm stock out of materials/supplies?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        bool success = false;
                        getDRNo();
                        if (chkInv.IsChecked == true)
                        {
                            getInvNo();
                        }
                        if (chkOR.IsChecked == true)
                        {
                            getORNo();
                        }
                        foreach (var item in photocopy)
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
                                    Log.Error(ex);
                                }
                            }
                            using (SqlCommand cmd = new SqlCommand("INSERT into SoldMaterials VALUES (@date, @desc, @qty, @total)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@desc", item.item);
                                cmd.Parameters.AddWithValue("@qty", item.qty);
                                cmd.Parameters.AddWithValue("@total", item.totalPerItem);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                    success = false;
                                }
                            }
                        }
                        foreach (var item in items)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into ReleasedMaterials VALUES (@DRNo, @itemCode, @desc, @type, @brand, @size, @qty, @totalPerItem, @remarks)", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                cmd.Parameters.AddWithValue("@desc", item.description);
                                cmd.Parameters.AddWithValue("@type", item.type);
                                cmd.Parameters.AddWithValue("@brand", item.brand);
                                cmd.Parameters.AddWithValue("@size", item.size);
                                cmd.Parameters.AddWithValue("@qty", item.qty);
                                cmd.Parameters.AddWithValue("@totalPerItem", item.totalPerItem);
                                cmd.Parameters.AddWithValue("@remarks", item.remarks);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    success = true;
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into SoldMaterials VALUES (@date, @desc, @qty, @totalPerItem)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@desc", item.description);
                                cmd.Parameters.AddWithValue("@qty", item.qty);
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
                                    Log.Error(ex);
                                }
                            }


                            using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems set qty = qty - @qty where itemCode = @itemCode", conn))
                            {
                                cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                cmd.Parameters.AddWithValue("@qty", item.qty);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    success = true;
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                }
                            }
                        }
                        if (chkCompany.IsChecked == false)
                        {

                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @paidAmt, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@desc", "DR[Stock Out]: " + txtDRNo.Text);
                                if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", txtTotal.Value);
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                }
                                if (rdUnpaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", 0);
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
                                }
                                if (rdDownpayment.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", txtDownpayment.Value);
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
                                }
                                cmd.Parameters.AddWithValue("@total", txtTotal.Value);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                    success = false;
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with D.R No: " + txtDRNo.Text + ", had a transaction amounting to Php " + txtTotal.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                    success = false;
                                }
                            }
                            using (SqlCommand cmd = new SqlCommand("INSERT into PaymentHist VALUES (@receiptNo, @service, @date, @paidAmt, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@receiptNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@service", "Stock Out");
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@total", txtTotal.Value - txtDiscount.Value);
                                if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", txtTotal.Value - txtDiscount.Value);
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                }
                                if (rdUnpaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", 0);
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
                                }
                                if (rdDownpayment.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", txtDownpayment.Value);
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
                                }
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                    success = false;
                                }
                            }
                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (DRNo, service, date, deadline, customerName, issuedBy, address, contactNo, remarks, ORNo, invoiceNo, status, claimed, inaccessible) VALUES (@DRNo, @service, @date, @deadline, @customerName, @issuedBy, @address, @contactNo, @remarks, @ORNo, @InvoiceNo, @status, @claimed, 1)", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@service", "Stock Out");
                                cmd.Parameters.AddWithValue("@deadline", "N\\A");
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                                cmd.Parameters.AddWithValue("@issuedBy", txtIssuedBy.Text);
                                cmd.Parameters.AddWithValue("@address", address);
                                cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                if (string.IsNullOrEmpty(txtORNo.Text))
                                {
                                    cmd.Parameters.AddWithValue("@ORNo", DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@ORNo", txtORNo.Text);
                                }
                                if (string.IsNullOrEmpty(txtInv.Text))
                                {
                                    cmd.Parameters.AddWithValue("@InvoiceNo", DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@InvoiceNo", txtInv.Text);
                                }
                                if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                    cmd.Parameters.AddWithValue("@claimed", "Claimed");
                                }
                                if (rdUnpaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
                                    cmd.Parameters.AddWithValue("@claimed", "Claimed");

                                }
                                if (rdDownpayment.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
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
                                    Log.Error(ex);
                                    success = false;

                                }
                            }
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (DRNo, service, date, deadline, customerName, issuedBy, address, contactNo, remarks, ORNo, invoiceNo, status, claimed, inaccessible) VALUES (@DRNo, @service, @date, @deadline, @customerName, @issuedBy, @address, @contactNo, @remarks, 0, 0, @status, @claimed, 1)", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@service", "Stock Out");
                                cmd.Parameters.AddWithValue("@deadline", "N\\A");
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                                cmd.Parameters.AddWithValue("@issuedBy", txtIssuedBy.Text);
                                cmd.Parameters.AddWithValue("@address", "N\\A");
                                cmd.Parameters.AddWithValue("@contactNo", "N\\A");
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                cmd.Parameters.AddWithValue("@status", "Paid");
                                cmd.Parameters.AddWithValue("@claimed", "Claimed");
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                    success = false;

                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@transaction", "Staff: " + txtCustName.Text + ", with DR No: " + txtDRNo.Text + ", stock out materials amounting to: " + txtTotal.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                    success = false;
                                }
                            }
                        }

                        promptPrintDR();
                        emptyFields();
                        items.Clear();
                        photocopy.Clear();
                        if (success)
                            MessageBox.Show("Transaction has been recorded!");
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
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
                        txtDRNo.Text = DateTime.Today.Year.ToString() + "0001";
                    else
                    {
                        int DRNoIndex = reader.GetOrdinal("DRNo");
                        int DRNo = 0;
                        if (Convert.ToInt32(Convert.ToString(reader.GetValue(DRNoIndex)).Substring(0, 4)) < DateTime.Today.Year)
                        {
                            txtDRNo.Text = DateTime.Today.Year.ToString() + "0001";
                        }
                        else
                        {
                            DRNo = Convert.ToInt32(reader.GetValue(DRNoIndex)) + 1;
                            txtDRNo.Text = DRNo.ToString();

                        }

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
                        txtInv.Text = "1";
                    else
                    {
                        int invoiceNoIndex = reader.GetOrdinal("InvoiceNo");
                        int invoiceNo = Convert.ToInt32(reader.GetValue(invoiceNoIndex)) + 1;
                        txtInv.Text = invoiceNo.ToString();
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
                txtTotal.Value -= last.totalPerItem;
                items.RemoveAt(items.Count - 1);
            }
        }

        private void TxtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtDiscount.Value > 0 && txtTotal.Value > 0)
            {
                txtDownpayment.MaxValue = Convert.ToDouble(txtTotal.Value - txtDiscount.Value) - 1;
            }
        }

        private void CkDealersPrice_Checked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemCode.Text) && !string.IsNullOrEmpty(txtDesc.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT dealersPrice from Inventoryitems where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                            double dealersPrice = Convert.ToDouble(reader.GetValue(dealersPriceIndex));
                            txtItemPrice.Value = dealersPrice;

                            txtTotalPerItem.Value = txtItemPrice.Value * txtQty.Value;
                        }
                    }
                }
            }
            else
            {
                txtItemPrice.Value = 0;
            }
        }

        private void CkDealersPrice_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemCode.Text) && !string.IsNullOrEmpty(txtDesc.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT msrp from Inventoryitems where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int msrpIndex = reader.GetOrdinal("msrp");
                            double msrp = Convert.ToDouble(reader.GetValue(msrpIndex));
                            txtItemPrice.Value = msrp;

                            txtTotalPerItem.Value = txtItemPrice.Value * txtQty.Value;
                        }
                    }
                }
            }
            else
            {
                txtItemPrice.Value = 0;
            }
        }

        bool searched = false;
        private void TxtItemCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searched == true)
            {
                txtDesc.Text = null;
                txtType.Text = null;
                txtBrand.Text = null;
                txtSize.Text = null;
                txtQty.Value = 0;
                txtItemPrice.Value = 0;
                txtTotalPerItem.Value = 0;
                searched = false;
                return;
            }
            if(txtItemCode.Text.Length > 8)
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {

                            reader.Read();
                            int descriptionIndex = reader.GetOrdinal("description");
                            txtDesc.Text = Convert.ToString(reader.GetValue(descriptionIndex));

                            int typeIndex = reader.GetOrdinal("type");
                            txtType.Text = Convert.ToString(reader.GetValue(typeIndex));

                            int brandIndex = reader.GetOrdinal("brand");
                            txtBrand.Text = Convert.ToString(reader.GetValue(brandIndex));

                            txtQty.Value = 0;

                            int sizeIndex = reader.GetOrdinal("size");
                            txtSize.Text = Convert.ToString(reader.GetValue(sizeIndex));

                            int msrpIndex = reader.GetOrdinal("MSRP");
                            txtItemPrice.Value = Convert.ToDouble(reader.GetValue(msrpIndex));

                            ckDealersPrice.IsChecked = false;
                            searched = true;
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }

                }
            }
        }

        private void promptPrintDR()
        {
            string sMessageBoxText = "Do you want to print the Delivery Receipt?";
            string sCaption = "Print Delivery Receipt Receipt";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            switch (dr)
            {
                case MessageBoxResult.Yes:
                    PrintCopies pc = new PrintCopies();
                    pc.ShowDialog();
                    int loop = pc.copies;
                    DocToPDFConverter converter = new DocToPDFConverter();
                    PdfDocument pdfDocument;
                    PdfViewerControl pdfViewer1 = new PdfViewerControl();
                    for (int print = 1; print <= loop; print++)
                    {

                        //should print 2 receipts, for customer and company
                        try
                        {
                            using (WordDocument document = new WordDocument("Templates/receipt-template.docx", FormatType.Docx))
                            {
                                Syncfusion.DocIO.DLS.TextSelection textSelection;
                                WTextRange textRange;

                                textSelection = document.Find("<dr no>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtDRNo.Text;
                                textSelection = document.Find("<full name>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtCustName.Text;
                                textSelection = document.Find("<printed name>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtCustName.Text.ToUpper();
                                textSelection = document.Find("<j.o no>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";
                                textSelection = document.Find("<date>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtDate.Text;
                                textSelection = document.Find("<address>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                TextRange address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                textRange.Text = address.Text;

                                //if item exceeds 13. create another file
                                //check if stock out, photocopy or what of the two job order is printing
                                WordDocument document2 = new WordDocument("Templates/receipt-template.docx", FormatType.Docx);
                                int counter = 1;
                                int counter2 = 1;
                                if (items.Count > 0)
                                {
                                    foreach (var item in items)
                                    {
                                        if (counter > 17)
                                        {
                                            textSelection = document2.Find("<dr no>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDRNo.Text;
                                            textSelection = document2.Find("<full name>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtCustName.Text;
                                            textSelection = document2.Find("<printed name>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtCustName.Text.ToUpper();
                                            textSelection = document2.Find("<j.o no>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = "";
                                            textSelection = document2.Find("<date>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDate.Text;
                                            textSelection = document2.Find("<address>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = address.Text;

                                            textSelection = document2.Find("<qty" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.qty.ToString();

                                            textSelection = document2.Find("<description" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.description;

                                            textSelection = document2.Find("<price" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = (item.totalPerItem / item.qty).ToString();

                                            textSelection = document2.Find("<amount" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.totalPerItem.ToString();
                                            counter2++;
                                        }
                                        else
                                        {
                                            textSelection = document.Find("<qty" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.qty.ToString();

                                            textSelection = document.Find("<description" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.description;

                                            textSelection = document.Find("<price" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = (item.totalPerItem / item.qty).ToString();

                                            textSelection = document.Find("<amount" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.totalPerItem.ToString();
                                            counter++;
                                        }

                                    }
                                }
                                if (photocopy.Count > 0)
                                {
                                    foreach (var item in photocopy)
                                    {
                                        if (counter > 17)
                                        {
                                            textSelection = document2.Find("<dr no>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDRNo.Text;
                                            textSelection = document2.Find("<full name>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtCustName.Text;
                                            textSelection = document2.Find("<printed name>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtCustName.Text.ToUpper();
                                            textSelection = document2.Find("<j.o no>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = "";
                                            textSelection = document2.Find("<date>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDate.Text;
                                            textSelection = document2.Find("<address>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = address.Text;

                                            textSelection = document2.Find("<qty" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.qty.ToString();

                                            textSelection = document2.Find("<description" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.item;

                                            textSelection = document2.Find("<price" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.price.ToString();

                                            textSelection = document2.Find("<amount" + counter2 + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.totalPerItem.ToString();
                                            counter2++;
                                        }
                                        else
                                        {
                                            textSelection = document.Find("<qty" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.qty.ToString();

                                            textSelection = document.Find("<description" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.item;

                                            textSelection = document.Find("<price" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.price.ToString();

                                            textSelection = document.Find("<amount" + counter + ">", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = item.totalPerItem.ToString();
                                            counter++;
                                        }
                                    }
                                }
                                //remove unused placeholder
                                for (int i = counter; i <= 17; i++)
                                {

                                    textSelection = document.Find("<qty" + i + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "";

                                    textSelection = document.Find("<description" + i + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "";

                                    textSelection = document.Find("<price" + i + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "";

                                    textSelection = document.Find("<amount" + i + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "";
                                }
                                if (counter2 > 1)
                                {
                                    for (int i = counter2; i <= 17; i++)
                                    {
                                        textSelection = document2.Find("<qty" + i + ">", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";

                                        textSelection = document2.Find("<description" + i + ">", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";

                                        textSelection = document2.Find("<price" + i + ">", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";

                                        textSelection = document2.Find("<amount" + i + ">", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";
                                    }
                                    pdfDocument = converter.ConvertToPDF(document2);
                                    pdfDocument.Save(Environment.CurrentDirectory + "/temp.pdf");
                                    pdfViewer1.Load(Environment.CurrentDirectory + "/temp.pdf");
                                    pdfViewer1.PrinterSettings.PageOrientation = PdfViewerPrintOrientation.Landscape;
                                    pdfViewer1.PrinterSettings.PageSize = PdfViewerPrintSize.ActualSize;
                                    pdfViewer1.Print();
                                    document2.Close();


                                }
                                pdfDocument = converter.ConvertToPDF(document);
                                pdfDocument.Save(Environment.CurrentDirectory + "/temp.pdf");
                                pdfViewer1.Load(Environment.CurrentDirectory + "/temp.pdf");
                                pdfViewer1.PrinterSettings.PageOrientation = PdfViewerPrintOrientation.Landscape;
                                pdfViewer1.PrinterSettings.PageSize = PdfViewerPrintSize.ActualSize;
                                pdfViewer1.Print();
                                File.Delete(Environment.CurrentDirectory + "/temp.pdf");

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error has been encountered! Log has been updated with the error");
                            Log = LogManager.GetLogger("*");
                            Log.Error(ex);
                            return;
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void BtnPhotocopyAddToList_Click(object sender, RoutedEventArgs e)
        {
            double shortPrice, longPrice, a4Price, a3Price, legalPrice;
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from PaperPrices", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    shortPrice = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("short")));
                    longPrice = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("long")));
                    a4Price = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("a4")));
                    a3Price = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("a3")));
                    legalPrice = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("legal")));
                }
            }
            bool isEmpty = true;
            //does not update display but does work
            if (!string.IsNullOrEmpty(txtShort.Text) && txtShort.Value != 0)
            {
                var found = photocopy.FirstOrDefault(x => x.item.Equals("Short") && x.qty > 0);
                if (found != null)
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "Short",
                        qty = Convert.ToInt32(found.qty + txtShort.Value),
                        price = (double)shortPrice,
                        totalPerItem = (double)((found.qty + txtShort.Value) * shortPrice)
                    });

                    foreach (var item in photocopy.Where(x => x.item.Equals("Short")).ToList())
                    {
                        photocopy.Remove(item);
                        break;
                    }
                }
                else
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "Short",
                        qty = Convert.ToInt32(txtShort.Value),
                        price = (double)shortPrice,
                        totalPerItem = (double)(Convert.ToInt64(txtShort.Value) * shortPrice)
                    });
                }
                txtTotal.Value += (double)(Convert.ToInt64(txtShort.Value) * shortPrice);
                isEmpty = false;
            }
            if (!string.IsNullOrEmpty(txtLong.Text) && txtLong.Value != 0)
            {
                var found = photocopy.FirstOrDefault(x => x.item.Equals("Long") && x.qty > 0);
                if (found != null)
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "Long",
                        qty = Convert.ToInt32(found.qty + txtLong.Value),
                        price = (double)longPrice,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtLong.Value) * longPrice)
                    });

                    foreach (var item in photocopy.Where(x => x.item.Equals("Long")).ToList())
                    {
                        photocopy.Remove(item);
                        break;
                    }
                }
                else
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "Long",
                        qty = Convert.ToInt32(txtLong.Value),
                        price = (double)longPrice,
                        totalPerItem = (double)(Convert.ToInt64(txtLong.Value) * longPrice)
                    });
                }

                txtTotal.Value += (double)(Convert.ToInt64(txtLong.Value) * longPrice);
                isEmpty = false;

            }
            if (!string.IsNullOrEmpty(txtLegal.Text) && txtLegal.Value != 0)
            {
                var found = photocopy.FirstOrDefault(x => x.item.Equals("Legal") && x.qty > 0);
                if (found != null)
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "Legal",
                        qty = Convert.ToInt32(found.qty + txtLegal.Value),
                        price = (double)legalPrice,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtLegal.Value) * legalPrice)
                    });

                    foreach (var item in photocopy.Where(x => x.item.Equals("Legal")).ToList())
                    {
                        photocopy.Remove(item);
                        break;
                    }
                }
                else
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "Legal",
                        qty = Convert.ToInt32(txtLegal.Value),
                        price = (double)legalPrice,
                        totalPerItem = (double)(Convert.ToInt64(txtLegal.Value) * legalPrice)
                    });
                }

                txtTotal.Value += (double)(Convert.ToInt64(txtLegal.Value) * legalPrice);
                isEmpty = false;
            }
            if (!string.IsNullOrEmpty(txtA4.Text) && txtA4.Value != 0)
            {
                var found = photocopy.FirstOrDefault(x => x.item.Equals("A4") && x.qty > 0);
                if (found != null)
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "A4",
                        qty = Convert.ToInt32(found.qty + txtA4.Value),
                        price = (double)a4Price,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtA4.Value) * a4Price)
                    });

                    foreach (var item in photocopy.Where(x => x.item.Equals("A4")).ToList())
                    {
                        photocopy.Remove(item);
                        break;
                    }
                }
                else
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "A4",
                        qty = Convert.ToInt32(txtA4.Value),
                        price = (double)a4Price,
                        totalPerItem = (double)(Convert.ToInt64(txtA4.Value) * a4Price)
                    });
                }
                txtTotal.Value += Convert.ToInt64(txtA4.Value) * 5.00;
                isEmpty = false;
            }
            if (!string.IsNullOrEmpty(txtA3.Text) && txtA3.Value != 0)
            {
                var found = photocopy.FirstOrDefault(x => x.item.Equals("A3") && x.qty > 0);
                if (found != null)
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "A3",
                        qty = Convert.ToInt32(found.qty + txtA3.Value),
                        price = (double)a3Price,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtA3.Value) * a3Price)
                    });

                    foreach (var item in photocopy.Where(x => x.item.Equals("A3")).ToList())
                    {
                        photocopy.Remove(item);
                        break;
                    }
                }
                else
                {
                    photocopy.Add(new PhotocopyDataModel
                    {
                        item = "A3",
                        qty = Convert.ToInt32(txtA3.Value),
                        price = (double)a3Price,
                        totalPerItem = (double)(Convert.ToInt64(txtA3.Value) * a3Price)
                    });
                }
                txtTotal.Value += (double)(Convert.ToInt64(txtA3.Value) * a3Price);
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

            }
        }

        private void BtnPhotocopyRemoveLastItem_Click(object sender, RoutedEventArgs e)
        {
            if (photocopy.Count == 0)
            {
                MessageBox.Show("Item list is empty");
            }
            else
            {
                //deduct to total
                var last = photocopy.Last();
                txtTotal.Value -= last.totalPerItem;
                photocopy.RemoveAt(photocopy.Count - 1);
            }
        }
    }
}
