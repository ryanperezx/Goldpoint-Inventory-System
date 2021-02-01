using NLog;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

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
            fillPaperPrice();
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
                var found = items.FirstOrDefault(x => x.item.Equals("Short") && x.qty > 0);
                if (found != null)
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Short",
                        qty = Convert.ToInt32(found.qty + txtShort.Value),
                        price = (double)txtShortPrice.Value,
                        totalPerItem = (double)((found.qty + txtShort.Value) * txtShortPrice.Value)
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
                        price = (double)txtShortPrice.Value,
                        totalPerItem = (double)(Convert.ToInt64(txtShort.Value) * txtShortPrice.Value)
                    });
                }
                total += (double)(Convert.ToInt64(txtShort.Value) * txtShortPrice.Value);
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
                        price = (double)txtLongPrice.Value,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtLong.Value) * txtLongPrice.Value)
                    });

                    foreach (var item in items.Where(x => x.item.Equals("Long")).ToList())
                    {
                        items.Remove(item);
                        break;
                    }
                }
                else
                {
                    items.Add(new PhotocopyDataModel
                    {
                        item = "Long",
                        qty = Convert.ToInt32(txtLong.Value),
                        price = (double)txtLongPrice.Value,
                        totalPerItem = (double)(Convert.ToInt64(txtLong.Value) * txtLongPrice.Value)
                    });
                }

                total += (double)(Convert.ToInt64(txtLong.Value) * txtLongPrice.Value);
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
                        price = (double)txtLegalPrice.Value,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtLegal.Value) * txtLegalPrice.Value)
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
                        price = (double)txtLegalPrice.Value,
                        totalPerItem = (double)(Convert.ToInt64(txtLegal.Value) * txtLegalPrice.Value)
                    });
                }

                total += (double)(Convert.ToInt64(txtLegal.Value) * txtLegalPrice.Value);
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
                        price = (double)txtA4Price.Value,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtA4.Value) * txtA4Price.Value)
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
                        price = (double)txtA4Price.Value,
                        totalPerItem = (double)(Convert.ToInt64(txtA4.Value) * txtA4Price.Value)
                    });
                }
                total += Convert.ToInt64(txtA4.Value) * 5.00;
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
                        price = (double)txtA3Price.Value,
                        totalPerItem = (double)(Convert.ToInt64(found.qty + txtA3.Value) * txtA3Price.Value)
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
                        price = (double)txtA3Price.Value,
                        totalPerItem = (double)(Convert.ToInt64(txtA3.Value) * txtA3Price.Value)
                    });
                }
                total += (double)(Convert.ToInt64(txtA3.Value) * txtA3Price.Value);
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
                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @qty, @total)", conn))
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
                        promptPrintDR();
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
                double downpayment = (double)txtDownpayment.Value;
                double total = (double)txtCustTotal.Value;
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

        private void BtnUpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Confirming Updating Photocopy Price(s)?";
            string sCaption = "Confirm Update?";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            switch (dr)
            {
                case MessageBoxResult.Yes:
                    SqlConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE PaperPrices set short = @short, long = @long, a3 = @a3, legal = @legal, a4 = @a4", conn))
                    {
                        cmd.Parameters.AddWithValue("@short", txtShortPrice.Value);
                        cmd.Parameters.AddWithValue("@long", txtLongPrice.Value);
                        cmd.Parameters.AddWithValue("@a3", txtA3Price.Value);
                        cmd.Parameters.AddWithValue("@legal", txtLegalPrice.Value);
                        cmd.Parameters.AddWithValue("@a4", txtA4Price.Value);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Price(s) has been updated");
                            fillPaperPrice();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("An error has been encountered! Log has been updated with the error");
                            Log = LogManager.GetLogger("*");
                            Log.Error(ex, "Query Error");
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }

        }

        private void fillPaperPrice()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from PaperPrices", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        txtShortPrice.Value = 0;
                        txtLongPrice.Value = 0;
                        txtA3Price.Value = 0;
                        txtA4Price.Value = 0;
                        txtLegalPrice.Value = 0;
                    }
                    else
                    {
                        txtShortPrice.Value = (double)(reader.GetValue(reader.GetOrdinal("short")));
                        txtLongPrice.Value = (double)(reader.GetValue(reader.GetOrdinal("long")));
                        txtA3Price.Value = (double)(reader.GetValue(reader.GetOrdinal("a3")));
                        txtA4Price.Value = (double)(reader.GetValue(reader.GetOrdinal("a4")));
                        txtLegalPrice.Value = (double)(reader.GetValue(reader.GetOrdinal("legal")));
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
                    DocToPDFConverter converter = new DocToPDFConverter();
                    PdfDocument pdfDocument;
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
                                    if (counter > 12)
                                    {
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
                            for (int i = counter; i <= 13; i++)
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
                                for (int i = counter2; i <= 13; i++)
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
                                pdfDocument.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Sample-2.pdf");
                                document2.Close();

                            }
                            pdfDocument = converter.ConvertToPDF(document);
                            pdfDocument.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Sample.pdf");
                            pdfDocument.Close(true);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                        Log = LogManager.GetLogger("*");
                        Log.Error(ex, "Query Error");
                        return;
                    }
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

    }
}
