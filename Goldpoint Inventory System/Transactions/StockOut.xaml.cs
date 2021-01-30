using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using NLog;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for StockOut.xaml
    /// </summary>
    public partial class StockOut : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public StockOut()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgStockOut.ItemsSource = items;
            //to avoid null error
            txtQty.TextChanged += TxtQty_TextChanged;
            txtDiscount.TextChanged += TxtDiscount_TextChanged;
            getDRNo();
            rdUnpaid.IsChecked = true;

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
                    txtDiscount.IsEnabled = false;
                    break;
                case "Downpayment":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = true;
                    txtDiscount.IsEnabled = true;
                    break;
                case "Paid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    txtDiscount.IsEnabled = true;
                    break;
                case "Company Use":
                    break;
            }
        }

        private void checkboxService(object sender, RoutedEventArgs e)
        {
            if (chkCompany.IsChecked == true)
            {
                chkDR.IsChecked = false;
                chkInv.IsChecked = false;
                chkOR.IsChecked = false;

                txtInv.Text = null;
                txtDRNo.Text = null;
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
                txtDiscount.IsEnabled = true;

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
                getDRNo();
                chkDR.IsChecked = true;
                txtAddress.IsEnabled = true;
                txtContactNo.IsEnabled = true;
                rdUnpaid.IsEnabled = true;
                rdDownpayment.IsEnabled = true;
                txtDiscount.IsEnabled = true;

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
                                    totalPerItem = Convert.ToDouble(found.totalPerItem  + txtTotalPerItem.Value)
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
                                    totalPerItem = (double)txtTotalPerItem.Value
                                });

                            }
                            txtTotal.Value += txtTotalPerItem.Value;
                            txtDownpayment.MaxValue = (double)txtTotal.Value - 1;
                            txtDiscount.MaxValue = (double)txtTotal.Value - 1;
                            txtQty.Value = 0;
                            txtItemPrice.Value = 0;
                            ckDealersPrice.IsChecked = false;

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
            emptyFields();
        }

        private void emptyFields()
        {
            txtItemCode.Text = null;
            txtDesc.Text = null;
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
        }

        private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            string address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd).Text;
            string remarks = new TextRange(txtTransactRemarks.Document.ContentStart, txtTransactRemarks.Document.ContentEnd).Text;

            if (items.Count == 0)
            {
                MessageBox.Show("Item list is empty");
            }
            else if (string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtCustName.Text))
            {
                if (chkCompany.IsChecked == false || string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(address))
                {
                    MessageBox.Show("One or more fields are empty!");
                }
                else
                {
                    MessageBox.Show("One or more fields are empty!");
                }
            }
            else if (rdDownpayment.IsChecked == true && string.IsNullOrEmpty(txtDownpayment.Text))
            {
                MessageBox.Show("Please input downpayment to be able to proceed");
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
                        foreach (var item in items)
                        {
                            if (chkCompany.IsChecked == false)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT into ReleasedMaterials VALUES (@DRNo, @itemCode, @desc, @type, @brand, @size, @qty, @totalPerItem)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                    cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                    cmd.Parameters.AddWithValue("@desc", item.description);
                                    cmd.Parameters.AddWithValue("@type", item.type);
                                    cmd.Parameters.AddWithValue("@brand", item.brand);
                                    cmd.Parameters.AddWithValue("@size", item.size);
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
                                        Log.Error(ex, "Query Error");
                                    }
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
                                    Log.Error(ex, "Query Error");
                                }
                            }
                        }
                        if (rdDownpayment.IsChecked == true && chkCompany.IsChecked == false)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @service, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@service", "Stock Out");
                                cmd.Parameters.AddWithValue("@total", txtDownpayment.Value);
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
                        else if (rdPaid.IsChecked == true && chkCompany.IsChecked == false)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @service, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@service", "Stock Out");
                                cmd.Parameters.AddWithValue("@total", txtTotal.Value);
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
                        if (chkCompany.IsChecked == false)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with D.R No: " + txtDRNo.Text + ", had a stock out transaction amounting to Php " + txtTotal.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
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
                                cmd.Parameters.AddWithValue("@service", "Stock Out");
                                cmd.Parameters.AddWithValue("@deadline", "N\\A");
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                                cmd.Parameters.AddWithValue("@address", address);
                                cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                cmd.Parameters.AddWithValue("@ORNo", txtORNo.Text);
                                cmd.Parameters.AddWithValue("@InvoiceNo", txtInv.Text);
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
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@transaction", "Staff: " + txtCustName.Text + ", stock out materials amounting to: " + txtTotal.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
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

                        if (success)
                            MessageBox.Show("Transaction has been recorded!");
                        emptyFields();
                        items.Clear();
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
            if(txtDiscount.Value > 0 && txtTotal.Value > 0)
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
            if(searched == true)
            {
                txtDesc.Text = null;
                txtType.Text = null;
                txtBrand.Text = null;
                txtSize.Text = null;
                txtQty.Value = 0;
                txtItemPrice.Value = 0;
                txtTotalPerItem.Value = 0;
                searched = false;
            }
        }
    }
}
