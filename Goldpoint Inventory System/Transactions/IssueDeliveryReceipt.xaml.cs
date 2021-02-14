using NLog;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
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
        ObservableCollection<DeliveryReceiptDataModel> items = new ObservableCollection<DeliveryReceiptDataModel>();
        public IssueDeliveryReceipt(string fullName)
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            txtIssuedBy.Text = fullName;
            dgItems.ItemsSource = items;
            getDRNo();
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

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            items.Clear();
            getDRNo();
            emptyFields();
        }

        private void emptyFields()
        {
            txtDate.Text = DateTime.Today.ToShortDateString();
            txtDateDeadline.Text = DateTime.Today.ToShortDateString();
            txtCustName.Text = null;
            txtIssuedBy.Text = null;
            txtContactNo.Text = null;
            txtAddress.Document.Blocks.Clear();
            txtRemarks.Document.Blocks.Clear();
            rdUnpaid.IsChecked = true;
            txtDownpayment.Value = 0;
            txtTotal.Value = 0;
            txtDownpayment.MaxValue = 0;

            txtDesc.Text = null;
            txtQty.Value = 0;
            txtUnitPrice.Value = 0;

            chkInv.IsChecked = false;
            chkOR.IsChecked = false;
            txtInv.Text = null;
            txtORNo.Text = null;
            txtPOJO.Text = null;
        }

        private void BtnSaveDeliveryReceipt_Click(object sender, RoutedEventArgs e)
        {
            string address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd).Text;
            string remarks = new TextRange(txtRemarks.Document.ContentStart, txtRemarks.Document.ContentEnd).Text;
            if (items.Count == 0)
            {
                MessageBox.Show("Item list is empty!");
            }
            else if (string.IsNullOrEmpty(txtCustName.Text) || string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtDateDeadline.Text) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(txtIssuedBy.Text))
            {
                MessageBox.Show("One or more fields are empty");
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
                string sMessageBoxText = "Confirm addition of Transaction";
                string sCaption = "Confirm Transaction?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        MessageBox.Show(txtPOJO.Text);
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        //insert all and print
                        //transaction details
                        foreach (var item in items)
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into ManualTransaction VALUES (@drNo, @desc, @qty, @unitPrice, @amount)", conn))
                            {
                                cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@desc", item.description);
                                cmd.Parameters.AddWithValue("@qty", item.qty);
                                cmd.Parameters.AddWithValue("@unitPrice", item.unitPrice);
                                cmd.Parameters.AddWithValue("@amount", item.amount);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                }
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (DRNo, poJoNo, service, date, deadline, customerName, issuedBy, address, contactNo, remarks, ORNo, invoiceNo, status, claimed, inaccessible) VALUES (@DRNo, @pojo, @service, @date, @deadline, @customerName, @issuedBy, @address, @contactNo, @remarks, @ORNo, @InvoiceNo, @status, @claimed, 1)", conn))
                        {
                            cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@service", "Manual Transaction");
                            cmd.Parameters.AddWithValue("@deadline", txtDateDeadline.Text);
                            cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                            cmd.Parameters.AddWithValue("@issuedBy", txtIssuedBy.Text);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);
                            cmd.Parameters.AddWithValue("@remarks", remarks);
                            if (string.IsNullOrEmpty(txtPOJO.Text))
                            {
                                cmd.Parameters.AddWithValue("@pojo", DBNull.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@pojo", txtPOJO.Text);
                            }
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

                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @amount, @total, @status)", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@desc", "DR[Manual]: " + txtDRNo.Text);
                            if (rdPaid.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@amount", txtTotal.Value);
                                cmd.Parameters.AddWithValue("@status", "Paid");
                            }
                            if (rdUnpaid.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@amount", 0);
                                cmd.Parameters.AddWithValue("@status", "Unpaid");
                            }
                            if (rdDownpayment.IsChecked == true)
                            {
                                cmd.Parameters.AddWithValue("@amount", txtDownpayment.Value);
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
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("INSERT into PaymentHist VALUES (@DRNo, @date, @paidAmt, @total, @status)", conn))
                        {
                            cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@total", txtTotal.Value);
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
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex);
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with D.R No: " + txtDRNo.Text + ", had a manual transaction amounting to Php " + txtTotal.Text);
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
                            }
                        }
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

        private void checkboxService(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            string value = chkbox.Content.ToString();

            getDRNo();
            txtAddress.IsEnabled = true;
            txtContactNo.IsEnabled = true;
            rdUnpaid.IsEnabled = true;
            rdDownpayment.IsEnabled = true;

            if (chkbox.IsChecked == true && value == "Original Receipt")
            {
                getORNo();
            }
            if (chkbox.IsChecked == true && value == "Invoice")
            {
                getInvNo();
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
            if (chkbox.IsChecked == false && value == "Original Receipt")
            {
                txtORNo.Text = null;
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
                            textRange.Text = txtPOJO.Text;
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
                                    if (counter > 13)
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
                                        textRange.Text = txtPOJO.Text;
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
                                        textRange.Text = item.unitPrice.ToString();

                                        textSelection = document2.Find("<amount" + counter2 + ">", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = item.amount.ToString();
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
                                        textRange.Text = item.unitPrice.ToString();

                                        textSelection = document.Find("<amount" + counter + ">", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = item.amount.ToString();
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
                                for (int i = counter2; i <= 14; i++)
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
                        Log.Error(ex);
                        return;
                    }
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
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
                    break;
                case "Downpayment":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = true;
                    break;
                case "Paid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    break;
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(txtUnitPrice.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if(txtQty.Value == 0)
            {
                MessageBox.Show("Please change quantity to anything greater than zero");
            }
            else if(txtUnitPrice.Value == 0)
            {
                MessageBox.Show("Please change unit price to anything greater than zero");

            }
            else
            {
                items.Add(new DeliveryReceiptDataModel
                {
                    description = txtDesc.Text,
                    qty = (int) txtQty.Value,
                    unitPrice = (double) txtUnitPrice.Value,
                    amount = (double)(txtQty.Value * txtUnitPrice.Value)
                });

                txtTotal.Value += (double)(txtQty.Value * txtUnitPrice.Value);
                txtDownpayment.MaxValue = (double)(txtTotal.Value - 1);

                txtDesc.Text = null;
                txtQty.Value = 0;
                txtUnitPrice.Value = 0;
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
                double amount = items[items.Count - 1].amount;
                txtTotal.Value -= amount;
                items.RemoveAt(items.Count - 1);
            }
        }
    }
}
