using Goldpoint_Inventory_System.Transactions;
using NLog;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
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
        ObservableCollection<JobOrderDataModel> tarp = new ObservableCollection<JobOrderDataModel>();
        ObservableCollection<DeliveryReceiptDataModel> items = new ObservableCollection<DeliveryReceiptDataModel>();

        private static Logger Log = LogManager.GetCurrentClassLogger();

        //for payment and claiming button
        bool exist = false;
        public TransactDetails()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgService.ItemsSource = services;
            dgTransact.ItemsSource = stockOut;
            dgPaymentHistory.ItemsSource = payHist;
            dgPhotocopy.ItemsSource = photocopy;
            dgTarpaulin.ItemsSource = tarp;
            dgItems.ItemsSource = items;

        }


        private void BtnSearchService_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServiceNo.Text) || string.IsNullOrEmpty(cmbService.Text))
            {
                MessageBox.Show("Please fill up the fields before searching for the service");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                string service = "";
                string jobOrderNo = "";
                int drNo = 0;
                double unpaidBalance = 0;
                exPhotocopy.IsEnabled = false;
                exStockOut.IsEnabled = false;
                exJobOrder.IsEnabled = false;
                exJobOrderTarp.IsEnabled = false;
                exIssueDR.IsEnabled = false;
                btnPrintJobOrder.IsEnabled = false;
                btnIssueDR.IsEnabled = false;
                emptyFields();
                clearServices();
                switch (cmbService.Text)
                {
                    case "Official Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails WHERE ORNo = @serviceNo and inaccessible = 1", conn))
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
                                        int issuedByIndex = reader.GetOrdinal("issuedBy");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        int invoiceNoIndex = reader.GetOrdinal("invoiceNo");
                                        int drNoIndex = reader.GetOrdinal("DRNo");
                                        int statusIndex = reader.GetOrdinal("status");
                                        int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                        int claimedIndex = reader.GetOrdinal("claimed");
                                        int pojoNoIndex = reader.GetOrdinal("poJoNo");


                                        if (reader.GetValue(jobOrderNoIndex) != DBNull.Value)
                                        {
                                            jobOrderNo = Convert.ToString(reader.GetValue(jobOrderNoIndex));
                                        }
                                        else if (reader.GetValue(pojoNoIndex) != DBNull.Value)
                                        {
                                            jobOrderNo = Convert.ToString(reader.GetValue(pojoNoIndex));
                                        }
                                        if (reader.GetValue(issuedByIndex) != DBNull.Value)
                                        {
                                            txtIssuedBy.Text = Convert.ToString(reader.GetValue(issuedByIndex));
                                        }
                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                        textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                        txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
                                        txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));

                                        if (!string.IsNullOrEmpty(Convert.ToString(reader.GetValue(invoiceNoIndex))))
                                        {
                                            chkInv.IsChecked = true;
                                            txtInvoiceNo.Text = Convert.ToString(reader.GetValue(invoiceNoIndex));
                                        }
                                        chkOR.IsChecked = true;
                                        chkDR.IsChecked = true;
                                        txtJobOrderNo.Text = Convert.ToString(jobOrderNo);
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

                                        if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed")
                                        {
                                            chkClaimed.IsChecked = true;
                                            btnClaiming.IsEnabled = false;
                                        }
                                        else
                                        {
                                            chkClaimed.IsChecked = false;
                                            btnClaiming.IsEnabled = true;

                                        }

                                        btnIssueOR.IsEnabled = true;
                                        btnIssueInvoice.IsEnabled = true;

                                        service = Convert.ToString(reader.GetValue(serviceIndex));
                                        drNo = Convert.ToInt32(reader.GetValue(drNoIndex));
                                        exist = true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    btnIssueOR.IsEnabled = false;
                                    btnIssueInvoice.IsEnabled = false;
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
                                            amount = Convert.ToDouble(reader.GetValue(paidAmtIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }
                        txtUnpaidBalancePayment.Value = Math.Abs(Convert.ToDouble(txtTotal.Value - unpaidBalance));
                        txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                        txtAmount.Value = (double)txtUnpaidBalancePayment.Value;

                        if (service == "Stock Out")
                        {
                            txtJobOrderNo.Text = null;
                            exPhotocopy.IsEnabled = true;
                            exStockOut.IsEnabled = true;
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
                                            int remarksIndex = reader.GetOrdinal("remarks");

                                            stockOut.Add(new ItemDataModel
                                            {
                                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                                remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Printing, Services, etc.")
                        {
                            exJobOrder.IsEnabled = true;
                            btnPrintJobOrder.IsEnabled = true;
                            if (string.IsNullOrEmpty(txtDRNo.Text))
                                btnIssueDR.IsEnabled = true;
                            txtJobOrder.Text = Convert.ToString(service);
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial where JobOrderNo = @jobOrderNo and service = 'Printing, Services, etc.'", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", jobOrderNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    services.Clear();
                                    while (reader.Read())
                                    {
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int unitIndex = reader.GetOrdinal("unit");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int materialIndex = reader.GetOrdinal("material");
                                        int copyIndex = reader.GetOrdinal("copy");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                        services.Add(new JobOrderDataModel
                                        {
                                            Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            unit = Convert.ToString(reader.GetValue(unitIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            material = Convert.ToString(reader.GetValue(materialIndex)),
                                            copy = Convert.ToString(reader.GetValue(copyIndex)),
                                            size = Convert.ToString(reader.GetValue(sizeIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Tarpaulin")
                        {
                            exJobOrderTarp.IsEnabled = true;
                            btnPrintJobOrder.IsEnabled = true;
                            if (string.IsNullOrEmpty(txtDRNo.Text))
                                btnIssueDR.IsEnabled = true;
                            txtJobOrder.Text = Convert.ToString(service);
                            using (SqlCommand cmd = new SqlCommand("SELECT * from TarpMaterial where JobOrderNo = @jobOrderNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", jobOrderNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    tarp.Clear();
                                    while (reader.Read())
                                    {
                                        int fileNameIndex = reader.GetOrdinal("fileName");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int mediaIndex = reader.GetOrdinal("media");
                                        int borderIndex = reader.GetOrdinal("border");
                                        int iLETIndex = reader.GetOrdinal("ILET");
                                        int unitPriceIndex = reader.GetOrdinal("unitPrice");



                                        tarp.Add(new JobOrderDataModel
                                        {
                                            fileName = Convert.ToString(reader.GetValue(fileNameIndex)),
                                            tarpQty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            tarpSize = Convert.ToString(reader.GetValue(sizeIndex)),
                                            media = Convert.ToString(reader.GetValue(mediaIndex)),
                                            border = Convert.ToString(reader.GetValue(borderIndex)),
                                            ILET = Convert.ToString(reader.GetValue(iLETIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                            amount = (double)(Convert.ToDouble(reader.GetValue(unitPriceIndex)) * Convert.ToInt32(reader.GetValue(qtyIndex)))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Manual Transaction")
                        {
                            exIssueDR.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ManualTransaction WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    items.Clear();
                                    while (reader.Read())
                                    {
                                        int descIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int unitPriceIndex = reader.GetOrdinal("unitPrice");
                                        int amountIndex = reader.GetOrdinal("amount");

                                        items.Add(new DeliveryReceiptDataModel
                                        {
                                            description = Convert.ToString(reader.GetValue(descIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(amountIndex))
                                        });
                                    }

                                }
                            }
                        }

                        break;
                    case "Delivery Receipt":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails WHERE DRNo = @serviceNo and inaccessible = 1", conn))
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
                                        int issuedByIndex = reader.GetOrdinal("issuedBy");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        int invoiceNoIndex = reader.GetOrdinal("invoiceNo");
                                        int orNoIndex = reader.GetOrdinal("ORNo");
                                        int statusIndex = reader.GetOrdinal("status");
                                        int claimedIndex = reader.GetOrdinal("claimed");
                                        int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                        int pojoNoIndex = reader.GetOrdinal("poJoNo");

                                        if (reader.GetValue(jobOrderNoIndex) != DBNull.Value)
                                        {
                                            jobOrderNo = Convert.ToString(reader.GetValue(jobOrderNoIndex));
                                        }
                                        else if (reader.GetValue(pojoNoIndex) != DBNull.Value)
                                        {
                                            jobOrderNo = Convert.ToString(reader.GetValue(pojoNoIndex));
                                        }
                                        if (reader.GetValue(issuedByIndex) != DBNull.Value)
                                        {
                                            txtIssuedBy.Text = Convert.ToString(reader.GetValue(issuedByIndex));
                                        }
                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                        textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                        txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
                                        txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));
                                        txtJobOrderNo.Text = Convert.ToString(jobOrderNo);

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
                                            btnClaiming.IsEnabled = false;
                                        }
                                        else
                                        {
                                            chkClaimed.IsChecked = false;
                                            btnClaiming.IsEnabled = true;

                                        }

                                        btnIssueOR.IsEnabled = true;
                                        btnIssueInvoice.IsEnabled = true;

                                        service = Convert.ToString(reader.GetValue(serviceIndex));
                                        drNo = Convert.ToInt32(txtServiceNo.Text);
                                        exist = true;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    btnIssueOR.IsEnabled = false;
                                    btnIssueInvoice.IsEnabled = false;
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
                                            amount = Convert.ToDouble(reader.GetValue(paidAmtIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));


                                    }
                                }
                            }
                        }

                        txtUnpaidBalancePayment.Value = txtTotal.Value - unpaidBalance;
                        txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                        txtAmount.Value = (double)txtUnpaidBalancePayment.Value;

                        if (service == "Stock Out")
                        {
                            txtJobOrderNo.Text = null;
                            exPhotocopy.IsEnabled = true;
                            exStockOut.IsEnabled = true;
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
                                            int remarksIndex = reader.GetOrdinal("remarks");

                                            stockOut.Add(new ItemDataModel
                                            {
                                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                                remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Printing, Services, etc.")
                        {
                            exJobOrder.IsEnabled = true;
                            btnPrintJobOrder.IsEnabled = true;
                            if (string.IsNullOrEmpty(txtDRNo.Text))
                                btnIssueDR.IsEnabled = true;
                            txtJobOrder.Text = Convert.ToString(service);
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial where JobOrderNo = @jobOrderNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", jobOrderNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    services.Clear();
                                    while (reader.Read())
                                    {
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int unitIndex = reader.GetOrdinal("unit");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int materialIndex = reader.GetOrdinal("material");
                                        int copyIndex = reader.GetOrdinal("copy");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                        services.Add(new JobOrderDataModel
                                        {
                                            Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            unit = Convert.ToString(reader.GetValue(unitIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            material = Convert.ToString(reader.GetValue(materialIndex)),
                                            copy = Convert.ToString(reader.GetValue(copyIndex)),
                                            size = Convert.ToString(reader.GetValue(sizeIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Tarpaulin")
                        {
                            exJobOrderTarp.IsEnabled = true;
                            btnPrintJobOrder.IsEnabled = true;
                            if (string.IsNullOrEmpty(txtDRNo.Text))
                                btnIssueDR.IsEnabled = true;
                            txtJobOrder.Text = Convert.ToString(service);
                            using (SqlCommand cmd = new SqlCommand("SELECT * from TarpMaterial where JobOrderNo = @jobOrderNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", jobOrderNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    tarp.Clear();
                                    while (reader.Read())
                                    {
                                        int fileNameIndex = reader.GetOrdinal("fileName");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int mediaIndex = reader.GetOrdinal("media");
                                        int borderIndex = reader.GetOrdinal("border");
                                        int iLETIndex = reader.GetOrdinal("ILET");
                                        int unitPriceIndex = reader.GetOrdinal("unitPrice");

                                        tarp.Add(new JobOrderDataModel
                                        {
                                            fileName = Convert.ToString(reader.GetValue(fileNameIndex)),
                                            tarpQty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            tarpSize = Convert.ToString(reader.GetValue(sizeIndex)),
                                            media = Convert.ToString(reader.GetValue(mediaIndex)),
                                            border = Convert.ToString(reader.GetValue(borderIndex)),
                                            ILET = Convert.ToString(reader.GetValue(iLETIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                            amount = (double)(Convert.ToDouble(reader.GetValue(unitPriceIndex)) * Convert.ToInt32(reader.GetValue(qtyIndex)))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Manual Transaction")
                        {
                            exIssueDR.IsEnabled = true;

                            using (SqlCommand cmd = new SqlCommand("SELECT * from ManualTransaction WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    items.Clear();
                                    while (reader.Read())
                                    {
                                        int descIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int unitPriceIndex = reader.GetOrdinal("unitPrice");
                                        int amountIndex = reader.GetOrdinal("amount");

                                        items.Add(new DeliveryReceiptDataModel
                                        {
                                            description = Convert.ToString(reader.GetValue(descIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(amountIndex))
                                        });
                                    }

                                }
                            }
                        }
                        break;
                    case "Invoice":
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails WHERE invoiceNo = @serviceNo and inaccessible = 1", conn))
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
                                        int issuedByIndex = reader.GetOrdinal("issuedBy");
                                        int addressIndex = reader.GetOrdinal("address");
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        int orNoIndex = reader.GetOrdinal("ORNo");
                                        int drNoIndex = reader.GetOrdinal("DRNo");
                                        int statusIndex = reader.GetOrdinal("status");
                                        int claimedIndex = reader.GetOrdinal("claimed");
                                        int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                        int pojoNoIndex = reader.GetOrdinal("pojoNo");

                                        if (reader.GetValue(jobOrderNoIndex) != DBNull.Value)
                                        {
                                            jobOrderNo = Convert.ToString(reader.GetValue(jobOrderNoIndex));
                                        }
                                        else if (reader.GetValue(pojoNoIndex) != DBNull.Value)
                                        {
                                            jobOrderNo = Convert.ToString(reader.GetValue(pojoNoIndex));
                                        }
                                        if (reader.GetValue(issuedByIndex) != DBNull.Value)
                                        {
                                            txtIssuedBy.Text = Convert.ToString(reader.GetValue(issuedByIndex));
                                        }
                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                        textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                        if (reader.GetValue(drNoIndex) != DBNull.Value)
                                            txtDRNo.Text = Convert.ToString(reader.GetValue(drNoIndex));
                                        txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
                                        txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));
                                        txtJobOrderNo.Text = Convert.ToString(jobOrderNo);

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

                                        if (Convert.ToString(reader.GetValue(claimedIndex)) == "Claimed")
                                        {
                                            chkClaimed.IsChecked = true;
                                            btnClaiming.IsEnabled = false;
                                        }
                                        else
                                        {
                                            chkClaimed.IsChecked = false;
                                            btnClaiming.IsEnabled = true;
                                        }

                                        btnIssueOR.IsEnabled = true;
                                        btnIssueInvoice.IsEnabled = true;

                                        service = Convert.ToString(reader.GetValue(serviceIndex));
                                        if (reader.GetValue(drNoIndex) != DBNull.Value)
                                            drNo = Convert.ToInt32(reader.GetValue(drNoIndex));
                                        exist = true;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Transaction does not exist");
                                    btnIssueOR.IsEnabled = false;
                                    btnIssueInvoice.IsEnabled = false;
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
                                            amount = Convert.ToDouble(reader.GetValue(paidAmtIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }

                        txtUnpaidBalancePayment.Value = Math.Abs(Convert.ToDouble(txtTotal.Value - unpaidBalance));
                        txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                        txtAmount.Value = (double)txtUnpaidBalancePayment.Value;

                        if (service == "Stock Out")
                        {
                            txtJobOrderNo.Text = null;
                            exPhotocopy.IsEnabled = true;
                            exStockOut.IsEnabled = true;
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
                                            int remarksIndex = reader.GetOrdinal("remarks");

                                            stockOut.Add(new ItemDataModel
                                            {
                                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                                totalPerItem = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                                remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else if (service == "Printing, Services, etc.")
                        {
                            exJobOrder.IsEnabled = true;
                            btnPrintJobOrder.IsEnabled = true;
                            if(string.IsNullOrEmpty(txtDRNo.Text))
                                btnIssueDR.IsEnabled = true;
                            txtJobOrder.Text = Convert.ToString(service);
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial where JobOrderNo = @jobOrderNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", jobOrderNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    services.Clear();
                                    while (reader.Read())
                                    {
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int unitIndex = reader.GetOrdinal("unit");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int materialIndex = reader.GetOrdinal("material");
                                        int copyIndex = reader.GetOrdinal("copy");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                        services.Add(new JobOrderDataModel
                                        {
                                            Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            unit = Convert.ToString(reader.GetValue(unitIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            material = Convert.ToString(reader.GetValue(materialIndex)),
                                            copy = Convert.ToString(reader.GetValue(copyIndex)),
                                            size = Convert.ToString(reader.GetValue(sizeIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Tarpaulin")
                        {
                            exJobOrderTarp.IsEnabled = true;
                            btnPrintJobOrder.IsEnabled = true;
                            if (string.IsNullOrEmpty(txtDRNo.Text))
                                btnIssueDR.IsEnabled = true;
                            txtJobOrder.Text = Convert.ToString(service);
                            using (SqlCommand cmd = new SqlCommand("SELECT * from TarpMaterial where JobOrderNo = @jobOrderNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", jobOrderNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    tarp.Clear();
                                    while (reader.Read())
                                    {
                                        int fileNameIndex = reader.GetOrdinal("fileName");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int mediaIndex = reader.GetOrdinal("media");
                                        int borderIndex = reader.GetOrdinal("border");
                                        int iLETIndex = reader.GetOrdinal("ILET");
                                        int unitPriceIndex = reader.GetOrdinal("unitPrice");

                                        tarp.Add(new JobOrderDataModel
                                        {
                                            fileName = Convert.ToString(reader.GetValue(fileNameIndex)),
                                            tarpQty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            tarpSize = Convert.ToString(reader.GetValue(sizeIndex)),
                                            media = Convert.ToString(reader.GetValue(mediaIndex)),
                                            border = Convert.ToString(reader.GetValue(borderIndex)),
                                            ILET = Convert.ToString(reader.GetValue(iLETIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                            amount = (double)(Convert.ToDouble(reader.GetValue(unitPriceIndex)) * Convert.ToInt32(reader.GetValue(qtyIndex)))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Manual Transaction")
                        {
                            exIssueDR.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ManualTransaction WHERE DRNo = @serviceNo", conn))
                            {
                                cmd.Parameters.AddWithValue("@serviceNo", drNo);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    items.Clear();
                                    while (reader.Read())
                                    {
                                        int descIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int unitPriceIndex = reader.GetOrdinal("unitPrice");
                                        int amountIndex = reader.GetOrdinal("amount");

                                        items.Add(new DeliveryReceiptDataModel
                                        {
                                            description = Convert.ToString(reader.GetValue(descIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(amountIndex))
                                        });
                                    }

                                }
                            }
                        }
                        break;
                    case "Job Order (Tarpaulin)":
                        //should be able to issue or and dr here
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo = @jobOrderNo and service = 'Tarpaulin' and inaccessible = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int drNoIndex = reader.GetOrdinal("DRNo");
                                    int orNoIndex = reader.GetOrdinal("ORNo");
                                    int invoiceNoIndex = reader.GetOrdinal("invoiceNo");
                                    int serviceIndex = reader.GetOrdinal("service");
                                    int dateIndex = reader.GetOrdinal("date");
                                    int deadlineIndex = reader.GetOrdinal("deadline");
                                    int customerNameIndex = reader.GetOrdinal("customerName");
                                    int issuedByIndex = reader.GetOrdinal("issuedBy");
                                    int addressIndex = reader.GetOrdinal("address");
                                    int contactNoIndex = reader.GetOrdinal("contactNo");
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    int statusIndex = reader.GetOrdinal("status");
                                    int claimedIndex = reader.GetOrdinal("Claimed");

                                    txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                    txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                    txtCustName.Text = Convert.ToString(reader.GetValue(customerNameIndex));
                                    TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                    textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                    txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
                                    txtDRNo.Text = Convert.ToString(reader.GetValue(drNoIndex));
                                    txtORNo.Text = Convert.ToString(reader.GetValue(orNoIndex));
                                    txtInvoiceNo.Text = Convert.ToString(reader.GetValue(invoiceNoIndex));
                                    txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));
                                    txtJobOrderNo.Text = txtServiceNo.Text;

                                    if (reader.GetValue(issuedByIndex) != DBNull.Value)
                                    {
                                        txtIssuedBy.Text = Convert.ToString(reader.GetValue(issuedByIndex));
                                    }

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
                                        btnClaiming.IsEnabled = false;
                                    }
                                    else
                                    {
                                        chkClaimed.IsChecked = false;
                                        btnClaiming.IsEnabled = true;
                                    }
                                    service = Convert.ToString(reader.GetValue(serviceIndex));
                                    drNo = Convert.ToInt32(reader.GetValue(drNoIndex));
                                    exist = true;
                                    exJobOrderTarp.IsEnabled = true;
                                    btnPrintJobOrder.IsEnabled = true;

                                    txtJobOrder.Text = Convert.ToString(service);

                                    btnIssueOR.IsEnabled = true;
                                    btnIssueInvoice.IsEnabled = true;

                                }
                                else
                                {
                                    MessageBox.Show("Job order does not exist.");
                                    exJobOrderTarp.IsEnabled = false;
                                    btnIssueOR.IsEnabled = false;
                                    btnIssueInvoice.IsEnabled = false;
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
                                            amount = Convert.ToDouble(reader.GetValue(paidAmtIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }

                        txtUnpaidBalancePayment.Value = Math.Abs(Convert.ToDouble(txtTotal.Value - unpaidBalance));
                        txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                        txtAmount.Value = (double)txtUnpaidBalancePayment.Value;
                        if (string.IsNullOrEmpty(txtDRNo.Text))
                            btnIssueDR.IsEnabled = true;

                        using (SqlCommand cmd = new SqlCommand("SELECT * from TarpMaterial where JobOrderNo = @jobOrderNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                tarp.Clear();
                                while (reader.Read())
                                {
                                    int fileNameIndex = reader.GetOrdinal("fileName");
                                    int qtyIndex = reader.GetOrdinal("qty");
                                    int sizeIndex = reader.GetOrdinal("size");
                                    int mediaIndex = reader.GetOrdinal("media");
                                    int borderIndex = reader.GetOrdinal("border");
                                    int iLETIndex = reader.GetOrdinal("ILET");
                                    int unitPriceIndex = reader.GetOrdinal("unitPrice");

                                    tarp.Add(new JobOrderDataModel
                                    {
                                        fileName = Convert.ToString(reader.GetValue(fileNameIndex)),
                                        tarpQty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                        tarpSize = Convert.ToString(reader.GetValue(sizeIndex)),
                                        media = Convert.ToString(reader.GetValue(mediaIndex)),
                                        border = Convert.ToString(reader.GetValue(borderIndex)),
                                        ILET = Convert.ToString(reader.GetValue(iLETIndex)),
                                        unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                        amount = (double)(Convert.ToDouble(reader.GetValue(unitPriceIndex)) * Convert.ToInt32(reader.GetValue(qtyIndex)))
                                    });
                                }
                            }
                        }
                        break;
                    case "Job Order (Printing, Services, etc.)":
                        //should be able to issue or and dr here
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo = @jobOrderNo and service = 'Printing, Services, etc.' and inaccessible = 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int drNoIndex = reader.GetOrdinal("DRNo");
                                    int orNoIndex = reader.GetOrdinal("ORNo");
                                    int invoiceNoIndex = reader.GetOrdinal("invoiceNo");
                                    int serviceIndex = reader.GetOrdinal("service");
                                    int dateIndex = reader.GetOrdinal("date");
                                    int deadlineIndex = reader.GetOrdinal("deadline");
                                    int customerNameIndex = reader.GetOrdinal("customerName");
                                    int issuedByIndex = reader.GetOrdinal("issuedBy");
                                    int addressIndex = reader.GetOrdinal("address");
                                    int contactNoIndex = reader.GetOrdinal("contactNo");
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    int statusIndex = reader.GetOrdinal("status");
                                    int claimedIndex = reader.GetOrdinal("Claimed");

                                    txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                    txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                    txtCustName.Text = Convert.ToString(reader.GetValue(customerNameIndex));
                                    TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                    textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                    txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
                                    txtDRNo.Text = Convert.ToString(reader.GetValue(drNoIndex));
                                    txtORNo.Text = Convert.ToString(reader.GetValue(orNoIndex));
                                    txtInvoiceNo.Text = Convert.ToString(reader.GetValue(invoiceNoIndex));
                                    txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));
                                    txtJobOrderNo.Text = txtServiceNo.Text;

                                    if (reader.GetValue(issuedByIndex) != DBNull.Value)
                                    {
                                        txtIssuedBy.Text = Convert.ToString(reader.GetValue(issuedByIndex));
                                    }

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
                                        btnClaiming.IsEnabled = false;
                                    }
                                    else
                                    {
                                        chkClaimed.IsChecked = false;
                                        btnClaiming.IsEnabled = true;
                                    }

                                    service = Convert.ToString(reader.GetValue(serviceIndex));
                                    if(reader.GetValue(drNoIndex) != DBNull.Value)
                                        drNo = Convert.ToInt32(reader.GetValue(drNoIndex));
                                    exist = true;
                                    exJobOrder.IsEnabled = true;
                                    btnPrintJobOrder.IsEnabled = true;
                                    txtJobOrder.Text = Convert.ToString(service);

                                    btnIssueOR.IsEnabled = true;
                                    btnIssueInvoice.IsEnabled = true;
                                }
                                else
                                {
                                    MessageBox.Show("Job order does not exist.");

                                    btnIssueOR.IsEnabled = false;
                                    btnIssueInvoice.IsEnabled = false;
                                    exJobOrder.IsEnabled = false;
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
                                            amount = Convert.ToDouble(reader.GetValue(paidAmtIndex))
                                        });

                                        unpaidBalance += Convert.ToDouble(reader.GetValue(paidAmtIndex));
                                        txtTotal.Value = Convert.ToDouble(reader.GetValue(totalIndex));
                                    }
                                }
                            }
                        }

                        txtUnpaidBalancePayment.Value = Math.Abs(Convert.ToDouble(txtTotal.Value - unpaidBalance));
                        txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                        txtAmount.Value = (double)txtUnpaidBalancePayment.Value;
                        if (string.IsNullOrEmpty(txtDRNo.Text))
                            btnIssueDR.IsEnabled = true;

                        using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial where JobOrderNo = @jobOrderNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtServiceNo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                services.Clear();
                                while (reader.Read())
                                {
                                    int descriptionIndex = reader.GetOrdinal("description");
                                    int unitIndex = reader.GetOrdinal("unit");
                                    int qtyIndex = reader.GetOrdinal("qty");
                                    int materialIndex = reader.GetOrdinal("material");
                                    int copyIndex = reader.GetOrdinal("copy");
                                    int sizeIndex = reader.GetOrdinal("size");
                                    int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                    services.Add(new JobOrderDataModel
                                    {
                                        Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                        unit = Convert.ToString(reader.GetValue(unitIndex)),
                                        qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                        material = Convert.ToString(reader.GetValue(materialIndex)),
                                        copy = Convert.ToString(reader.GetValue(copyIndex)),
                                        size = Convert.ToString(reader.GetValue(sizeIndex)),
                                        unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(qtyIndex)),
                                        amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                    });
                                }
                            }
                        }
                        break;
                }
                if (txtUnpaidBalancePayment.Value <= 0)
                {
                    btnPayment.IsEnabled = false;
                }
                else
                {
                    btnPayment.IsEnabled = true;
                }
            }
        }

        private void clearServices()
        {
            photocopy.Clear();
            stockOut.Clear();
            payHist.Clear();
            services.Clear();
            tarp.Clear();
            items.Clear();
        }

        private void emptyFields()
        {
            txtAmount.Value = 0;
            txtAmount.MaxValue = 0;
            txtCustName.Text = null;
            txtDate.Text = DateTime.Today.ToShortDateString();
            txtDatePayment.Text = DateTime.Today.ToShortDateString();
            txtDRNo.Text = null;
            txtInvoiceNo.Text = null;
            txtORNo.Text = null;
            txtContactNo.Text = null;
            txtTotal.Value = 0;
            txtUnpaidBalancePayment.Value = 0;
            chkClaimed.IsChecked = false;
            chkDR.IsChecked = false;
            chkInv.IsChecked = false;
            chkOR.IsChecked = false;
            txtJobOrder.Text = null;
            txtJobOrderNo.Text = null;
            txtRemarks.Text = null;
            txtAddress.Document.Blocks.Clear();
        }

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            txtServiceNo.Text = null;
            photocopy.Clear();
            stockOut.Clear();
            payHist.Clear();
            services.Clear();
            tarp.Clear();
            exPhotocopy.IsEnabled = false;
            exStockOut.IsEnabled = false;
            exJobOrder.IsEnabled = false;
            exJobOrderTarp.IsEnabled = false;
            exist = false;
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
                    //if already fully paid, should disable the button and update transactiondetails status
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
                            bool fullyPaid = false;
                            int count = 0;
                            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) from TransactionDetails WHERE drNo = @DRNo and date = @date", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@date", txtDatePayment.Text);
                                try
                                {
                                    count = (int)cmd.ExecuteScalar();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex);
                                }
                            }
                            if (count == 0)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT into PaymentHist VALUES (@DRNo, @date, @paidAmount, @total, @status)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                    cmd.Parameters.AddWithValue("@date", txtDatePayment.Text);
                                    cmd.Parameters.AddWithValue("@paidAmount", txtAmount.Value);
                                    cmd.Parameters.AddWithValue("@total", txtTotal.Value);
                                    if (txtAmount.Value == txtUnpaidBalancePayment.Value)
                                    {
                                        cmd.Parameters.AddWithValue("@status", "Paid");
                                        fullyPaid = true;
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@status", "Unpaid");
                                    }
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                        MessageBox.Show("Payment history updated");
                                        payHist.Add(new PaymentHistoryDataModel
                                        {
                                            date = txtDatePayment.Text,
                                            amount = (double)txtAmount.Value
                                        });


                                        using (SqlCommand cmd1 = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, '')", conn))
                                        {
                                            cmd1.Parameters.AddWithValue("@date", DateTime.Today.ToShortDateString());
                                            cmd1.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with D.R No: " + txtDRNo.Text + ", paid Php " + txtAmount.Text + ". Current Outstanding Balance is Php " + txtTotal.Text);
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

                                        using (SqlCommand cmd1 = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @amount, @total, @status)", conn))
                                        {
                                            cmd1.Parameters.AddWithValue("@date", txtDatePayment.Text);
                                            if (stockOut.Count > 0 || photocopy.Count > 0)
                                            {
                                                cmd1.Parameters.AddWithValue("@desc", "DR[Stock Out]: " + txtDRNo.Text);
                                            }
                                            else if (services.Count > 0)
                                            {
                                                cmd1.Parameters.AddWithValue("@desc", "JO[Services]:" + txtJobOrderNo.Text);
                                            }
                                            else if (tarp.Count > 0)
                                            {
                                                cmd1.Parameters.AddWithValue("@desc", "JO[Tarpaulin]: " + txtJobOrderNo.Text);
                                            }
                                            else if (items.Count > 0)
                                            {
                                                cmd1.Parameters.AddWithValue("@desc", "DR[Manual]: " + txtDRNo.Text);
                                            }
                                            cmd1.Parameters.AddWithValue("@amount", txtAmount.Value);
                                            cmd1.Parameters.AddWithValue("@total", txtTotal.Value);
                                            if (txtTotal.Value > txtAmount.Value)
                                            {
                                                cmd1.Parameters.AddWithValue("@status", "Unpaid");
                                            }
                                            else
                                            {
                                                cmd1.Parameters.AddWithValue("@status", "Paid");
                                            }
                                            try
                                            {
                                                cmd1.ExecuteNonQuery();
                                            }
                                            catch (SqlException ex)
                                            {
                                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                                Log = LogManager.GetLogger("*");
                                                Log.Error(ex);
                                            }
                                        }
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex);
                                    }

                                }
                            }
                            else
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE PaymentHist set paidAmount = @paidAmount where DRNo = @DRNo and date = @date", conn))
                                {
                                    cmd.Parameters.AddWithValue("@paidAmount", txtAmount.Value);
                                    cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                    cmd.Parameters.AddWithValue("@date", txtDatePayment.Text);
                                    try
                                    {
                                        MessageBox.Show("Payment history updated");
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex);
                                    }
                                }

                                using (SqlCommand cmd1 = new SqlCommand("UPDATE Sales set amount = @amount, status = @status where date = @date and [desc] = @desc", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@date", txtDatePayment.Text);
                                    if (stockOut.Count > 0 || photocopy.Count > 0)
                                    {
                                        cmd1.Parameters.AddWithValue("@desc", "DR[Stock Out]: " + txtDRNo.Text);
                                    }
                                    else if (services.Count > 0)
                                    {
                                        cmd1.Parameters.AddWithValue("@desc", "JO[Services]:" + txtJobOrderNo.Text);
                                    }
                                    else if (tarp.Count > 0)
                                    {
                                        cmd1.Parameters.AddWithValue("@desc", "JO[Tarpaulin]: " + txtJobOrderNo.Text);
                                    }
                                    else if (items.Count > 0)
                                    {
                                        cmd1.Parameters.AddWithValue("@desc", "DR[Manual]: " + txtDRNo.Text);
                                    }
                                    cmd1.Parameters.AddWithValue("@amount", txtAmount.Value);
                                    if (txtTotal.Value > txtAmount.Value)
                                    {
                                        cmd1.Parameters.AddWithValue("@status", "Unpaid");
                                    }
                                    else
                                    {
                                        cmd1.Parameters.AddWithValue("@status", "Paid");
                                    }
                                    try
                                    {
                                        cmd1.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex);
                                    }
                                }
                            }

                            txtUnpaidBalancePayment.Value -= txtAmount.Value;
                            txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                            if (txtUnpaidBalancePayment.Value == 0)
                            {
                                btnPayment.IsEnabled = false;
                                rdPaid.IsChecked = true;
                            }
                            else
                            {
                                btnPayment.IsEnabled = true;
                            }
                            if (fullyPaid)
                            {
                                using (SqlCommand cmd1 = new SqlCommand("UPDATE PaymentHist set status = 'Paid' where DRNo = @DRNo", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                    try
                                    {
                                        cmd1.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex);
                                    }

                                }
                                using (SqlCommand cmd1 = new SqlCommand("UPDATE TransactionDetails set status = 'Paid' where DRNo = @DRNo", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                    try
                                    {
                                        cmd1.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex);
                                    }

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
                                MessageBox.Show("Transaction has been updated");
                                chkClaimed.IsChecked = true;
                                btnClaiming.IsEnabled = false;
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex);
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
        private void BtnPrintJobOrder_Click(object sender, RoutedEventArgs e)
        {
            DocToPDFConverter converter = new DocToPDFConverter();
            PdfDocument pdfDocument;
            PdfViewerControl pdfViewer1 = new PdfViewerControl();

            //should print 2 receipts, for customer and company
            if (txtJobOrder.Text == "Printing, Services, etc.")
            {
                try
                {
                    using (WordDocument document = new WordDocument("Templates/job-order-template.docx", FormatType.Docx))
                    {
                        Syncfusion.DocIO.DLS.TextSelection textSelection;
                        WTextRange textRange;

                        textSelection = document.Find("<full name>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtCustName.Text;
                        textSelection = document.Find("<issuer>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtIssuedBy.Text;
                        textSelection = document.Find("<job order no>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtJobOrderNo.Text;
                        textSelection = document.Find("<deadline>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtDeadline.Text;
                        textSelection = document.Find("<date>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = DateTime.Today.ToShortDateString();
                        textSelection = document.Find("<address>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        TextRange address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                        textRange.Text = address.Text;
                        textSelection = document.Find("<contact no>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtContactNo.Text;
                        textSelection = document.Find("<total>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtTotal.Text;
                        textSelection = document.Find("<total>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtTotal.Text;

                        if (txtUnpaidBalancePayment.Value > 0)
                        {
                            textSelection = document.Find("<balance>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = txtUnpaidBalancePayment.Text;
                            textSelection = document.Find("<downpayment>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = txtAmount.Text;
                        }
                        else
                        {
                            textSelection = document.Find("<balance>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "0";
                            textSelection = document.Find("<downpayment>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "0";
                        }
                        //create new file if item exceeds table (10 rows)
                        int counter = 1;
                        int counter2 = 1;
                        WordDocument document2 = new WordDocument("Templates/job-order-template.docx", FormatType.Docx);
                        foreach (var item in services)
                        {
                            if (counter > 10)
                            {
                                //should print the other document first
                                //use another counter
                                textSelection = document2.Find("<full name>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtCustName.Text;
                                textSelection = document2.Find("<issuer>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtIssuedBy.Text;
                                textSelection = document2.Find("<job order no>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtJobOrderNo.Text;
                                textSelection = document2.Find("<deadline>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtDeadline.Text;
                                textSelection = document2.Find("<date>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = DateTime.Today.ToShortDateString();
                                textSelection = document2.Find("<address>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = address.Text;
                                textSelection = document2.Find("<contact no>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtContactNo.Text;
                                textSelection = document2.Find("<total>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtTotal.Text;
                                textSelection = document2.Find("<total>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtTotal.Text;
                                if (txtUnpaidBalancePayment.Value > 0)
                                {
                                    textSelection = document2.Find("<balance>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtUnpaidBalancePayment.Text;
                                    textSelection = document2.Find("<downpayment>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtAmount.Text;
                                }
                                else
                                {
                                    textSelection = document2.Find("<balance>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "0";
                                    textSelection = document2.Find("<downpayment>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "0";
                                }

                                textSelection = document2.Find("<qty" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.qty.ToString();

                                textSelection = document2.Find("<unit" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.unit;

                                textSelection = document2.Find("<description" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.material;

                                textSelection = document2.Find("<copy" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.copy;

                                textSelection = document2.Find("<size" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.size;

                                textSelection = document2.Find("<material" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.material;

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

                                textSelection = document.Find("<unit" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.unit;

                                textSelection = document.Find("<description" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.material;

                                textSelection = document.Find("<copy" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.copy;

                                textSelection = document.Find("<size" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.size;

                                textSelection = document.Find("<material" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.material;

                                textSelection = document.Find("<price" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.unitPrice.ToString();

                                textSelection = document.Find("<amount" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.amount.ToString();

                                counter++;
                            }
                        }

                        //removing unused fields
                        for (int i = counter; i <= 10; i++)
                        {
                            textSelection = document.Find("<qty" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<unit" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<description" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<copy" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<size" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<material" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<price" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<amount" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";
                        }
                        //optional
                        if (counter2 > 1)
                        {
                            for (int i = counter2; i <= 10; i++)
                            {
                                textSelection = document2.Find("<qty" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<unit" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<description" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<copy" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<size" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<material" + i + ">", false, true);
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
                            pdfViewer1.Print();
                            document2.Close();
                        }
                        pdfDocument = converter.ConvertToPDF(document);

                        pdfDocument.Save(Environment.CurrentDirectory + "/temp.pdf");
                        pdfViewer1.Load(Environment.CurrentDirectory + "/temp.pdf");
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
            else if (txtJobOrder.Text == "Tarpaulin")
            {
                try
                {
                    using (WordDocument document = new WordDocument("Templates/job-order-tarpaulin-template.docx", FormatType.Docx))
                    {
                        Syncfusion.DocIO.DLS.TextSelection textSelection;
                        WTextRange textRange;
                        IWSection section = document.AddSection();
                        section.PageSetup.Orientation = PageOrientation.Landscape;

                        textSelection = document.Find("<full name>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtCustName.Text;
                        textSelection = document.Find("<issuer>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtIssuedBy.Text;
                        textSelection = document.Find("<job order no>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtJobOrderNo.Text;
                        textSelection = document.Find("<deadline>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtDeadline.Text;
                        textSelection = document.Find("<date>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = DateTime.Today.ToShortDateString();
                        textSelection = document.Find("<address>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        TextRange address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                        textRange.Text = address.Text;
                        textSelection = document.Find("<contact no>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtContactNo.Text;
                        textSelection = document.Find("<total>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtTotal.Text;
                        textSelection = document.Find("<total>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = txtTotal.Text;
                        if (txtUnpaidBalancePayment.Value > 0)
                        {
                            textSelection = document.Find("<balance>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = txtUnpaidBalancePayment.Text;
                            textSelection = document.Find("<downpayment>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "0";
                        }
                        else
                        {
                            textSelection = document.Find("<balance>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "0";
                            textSelection = document.Find("<downpayment>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "0";
                        }

                        int counter = 1;
                        int counter2 = 1;
                        WordDocument document2 = new WordDocument("Templates/job-order-tarpaulin-template.docx", FormatType.Docx);
                        //create new file if item exceeds table (9 rows)
                        foreach (var item in tarp)
                        {
                            if (counter > 9)
                            {

                                textSelection = document2.Find("<full name>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtCustName.Text;
                                textSelection = document2.Find("<issuer>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtIssuedBy.Text;
                                textSelection = document2.Find("<job order no>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtJobOrderNo.Text;
                                textSelection = document2.Find("<deadline>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtDeadline.Text;
                                textSelection = document2.Find("<date>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = DateTime.Today.ToShortDateString();
                                textSelection = document2.Find("<address>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = address.Text;
                                textSelection = document2.Find("<contact no>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtContactNo.Text;
                                textSelection = document2.Find("<total>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtTotal.Text;
                                textSelection = document2.Find("<total>", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = txtTotal.Text;
                                if (txtUnpaidBalancePayment.Value > 0)
                                {
                                    textSelection = document2.Find("<balance>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtUnpaidBalancePayment.Text;
                                    textSelection = document2.Find("<downpayment>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "0";
                                }
                                else
                                {
                                    textSelection = document2.Find("<balance>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "0";
                                    textSelection = document2.Find("<downpayment>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "0";
                                }

                                textSelection = document2.Find("<qty" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.tarpQty.ToString();

                                textSelection = document2.Find("<file name" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.fileName;

                                textSelection = document2.Find("<size" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.tarpSize;

                                textSelection = document2.Find("<media" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.media;

                                textSelection = document2.Find("<border" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.border;

                                textSelection = document2.Find("<ilet" + counter2 + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.ILET;

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
                                textRange.Text = item.tarpQty.ToString();

                                textSelection = document.Find("<file name" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.fileName;

                                textSelection = document.Find("<size" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.tarpSize;

                                textSelection = document.Find("<media" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.media;

                                textSelection = document.Find("<border" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.border;

                                textSelection = document.Find("<ilet" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.ILET;

                                textSelection = document.Find("<price" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.unitPrice.ToString();

                                textSelection = document.Find("<amount" + counter + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = item.amount.ToString();

                                counter++;
                            }
                        }
                        //remove text from docu if item > 10

                        for (int i = counter; i <= 9; i++)
                        {
                            textSelection = document.Find("<qty" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<file name" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<size" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<media" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<border" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<ilet" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<price" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                            textSelection = document.Find("<amount" + i + ">", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";

                        }

                        //optional
                        if (counter2 > 1)
                        {
                            for (int i = counter2; i <= 9; i++)
                            {
                                textSelection = document2.Find("<qty" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<file name" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<size" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<media" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<border" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<ilet" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<price" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                textSelection = document2.Find("<amount" + i + ">", false, true);
                                textRange = textSelection.GetAsOneRange();
                                textRange.Text = "";

                                pdfDocument = converter.ConvertToPDF(document2);
                                pdfDocument.Save(Environment.CurrentDirectory + "/temp.pdf");
                                pdfViewer1.Load(Environment.CurrentDirectory + "/temp.pdf");
                                pdfViewer1.Print();
                                document2.Close();
                            }
                        }
                        pdfDocument = converter.ConvertToPDF(document);

                        pdfDocument.Save(Environment.CurrentDirectory + "/temp.pdf");
                        pdfViewer1.Load(Environment.CurrentDirectory + "/temp.pdf");
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

        }
        private void BtnPrintDR_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDRNo.Text))
            {
                MessageBox.Show("Print DR is not applicable to this transaction");
            }
            else
            {
                DocToPDFConverter converter = new DocToPDFConverter();
                PdfDocument pdfDocument;
                PdfViewerControl pdfViewer1 = new PdfViewerControl();

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
                        if (string.IsNullOrEmpty(txtJobOrderNo.Text) || txtJobOrderNo.Text.Equals("0"))
                        {
                            textSelection = document.Find("<j.o no>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = "";
                        }
                        else
                        {
                            textSelection = document.Find("<j.o no>", false, true);
                            textRange = textSelection.GetAsOneRange();
                            textRange.Text = txtJobOrderNo.Text;
                        }
                        textSelection = document.Find("<date>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        textRange.Text = DateTime.Today.ToShortDateString();
                        textSelection = document.Find("<address>", false, true);
                        textRange = textSelection.GetAsOneRange();
                        TextRange address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                        textRange.Text = address.Text;

                        //if item exceeds 13. create another file
                        //check if stock out, photocopy or what of the two job order is printing
                        WordDocument document2 = new WordDocument("Templates/receipt-template.docx", FormatType.Docx);
                        int counter = 1;
                        int counter2 = 1;
                        if (services.Count > 0)
                        {
                            var grouped = services.GroupBy(i => i.Description).Select(i => new { Description = i.Key, Quantity = i.Sum(item => item.qty), UnitPrice = i.Sum(item => item.unitPrice), Amount = i.Sum(item => item.amount) }); //group 
                            foreach (var item in grouped)
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
                                    if (string.IsNullOrEmpty(txtJobOrderNo.Text) || txtJobOrderNo.Text.Equals("0"))
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";
                                    }
                                    else
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = txtJobOrderNo.Text;
                                    }
                                    textSelection = document2.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = DateTime.Today.ToShortDateString();
                                    textSelection = document2.Find("<address>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = address.Text;

                                    textSelection = document2.Find("<qty" + counter2 + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.Quantity.ToString();

                                    textSelection = document2.Find("<description" + counter2 + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.Description;

                                    textSelection = document2.Find("<price" + counter2 + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.UnitPrice.ToString();

                                    textSelection = document2.Find("<amount" + counter2 + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.Amount.ToString();
                                    counter2++;
                                }
                                else
                                {
                                    textSelection = document.Find("<qty" + counter + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.Quantity.ToString();

                                    textSelection = document.Find("<description" + counter + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.Description;

                                    textSelection = document.Find("<price" + counter + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.UnitPrice.ToString();

                                    textSelection = document.Find("<amount" + counter + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.Amount.ToString();
                                    counter++;
                                }
                            }
                        }
                        else if (tarp.Count > 0)
                        {
                            foreach (var item in tarp)
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
                                    if (string.IsNullOrEmpty(txtJobOrderNo.Text) || txtJobOrderNo.Text.Equals("0"))
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";
                                    }
                                    else
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = txtJobOrderNo.Text;
                                    }
                                    textSelection = document2.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = DateTime.Today.ToShortDateString();
                                    textSelection = document2.Find("<address>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = address.Text;

                                    textSelection = document2.Find("<qty" + counter2 + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.tarpQty.ToString();

                                    textSelection = document2.Find("<description" + counter2 + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.fileName;

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
                                    textRange.Text = item.tarpQty.ToString();

                                    textSelection = document.Find("<description" + counter + ">", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = item.fileName;

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
                        else if (photocopy.Count > 0)
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
                                    if (string.IsNullOrEmpty(txtJobOrderNo.Text) || txtJobOrderNo.Text.Equals("0"))
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";
                                    }
                                    else
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = txtJobOrderNo.Text;
                                    }
                                    textSelection = document2.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = DateTime.Today.ToShortDateString();
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
                        else if (stockOut.Count > 0)
                        {
                            foreach (var item in stockOut)
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
                                    if (string.IsNullOrEmpty(txtJobOrderNo.Text) || txtJobOrderNo.Text.Equals("0"))
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";
                                    }
                                    else
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = txtJobOrderNo.Text;
                                    }
                                    textSelection = document2.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = DateTime.Today.ToShortDateString();
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
                        else if (items.Count > 0)
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
                                    if (string.IsNullOrEmpty(txtJobOrderNo.Text) || txtJobOrderNo.Text.Equals("0"))
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = "";
                                    }
                                    else
                                    {
                                        textSelection = document2.Find("<j.o no>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = txtJobOrderNo.Text;
                                    }
                                    textSelection = document2.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = DateTime.Today.ToShortDateString();
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
                    Log.Error(ex, "Query Error");
                    return;
                }

            }
        }

        private void BtnIssueOR_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDRNo.Text) && string.IsNullOrEmpty(txtJobOrderNo.Text))
            {
                MessageBox.Show("Please search for the transaction first.");
            }
            else
            {
                string sMessageBoxText = "Confirming issue of Original Receipt for Transaction";
                string sCaption = "Update transaction?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
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
                        using (SqlCommand cmd = new SqlCommand("UPDATE TransactionDetails set ORNo = @orNo where (DRNo = @drNo OR (jobOrderNo = @jobOrderNo AND service = @service))", conn))
                        {
                            cmd.Parameters.AddWithValue("@orNo", txtORNo.Text);
                            cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrderNo.Text);
                            cmd.Parameters.AddWithValue("@service", txtJobOrder.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex);
                                return;
                            }
                        }
                        MessageBox.Show("Transaction has been updated");
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }
        private void BtnIssueInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDRNo.Text) && string.IsNullOrEmpty(txtJobOrderNo.Text))
            {
                MessageBox.Show("Please search for the transaction first.");
            }
            else
            {
                string sMessageBoxText = "Confirming issue of Invoice for Transaction";
                string sCaption = "Update Transaction?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 InvoiceNo from TransactionDetails WHERE invoiceNo is not null AND DATALENGTH(invoiceNo) > 0 ORDER BY InvoiceNo DESC", conn))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    txtInvoiceNo.Text = "1";
                                else
                                {
                                    int invoiceNoIndex = reader.GetOrdinal("InvoiceNo");
                                    int invoiceNo = Convert.ToInt32(reader.GetValue(invoiceNoIndex)) + 1;
                                    txtInvoiceNo.Text = invoiceNo.ToString();
                                }
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("UPDATE TransactionDetails set InvoiceNo = @invoiceNo where (DRNo = @drNo OR (jobOrderNo = @jobOrderNo AND service = @service))", conn))
                        {
                            cmd.Parameters.AddWithValue("@invoiceNo", txtInvoiceNo.Text);
                            cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrderNo.Text);
                            cmd.Parameters.AddWithValue("@service", txtJobOrder.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex);
                                return;
                            }
                        }
                        MessageBox.Show("Transaction has been updated");
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void BtnIssueDR_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDRNo.Text))
            {
                MessageBox.Show("Delivery Receipt has already been issued");
            }
            else
            {
                string sMessageBoxText = "Confirming issue of Delivery Receipt for Transaction";
                string sCaption = "Update Transaction?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
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
                        using (SqlCommand cmd = new SqlCommand("UPDATE TransactionDetails set drNo = @drNo where jobOrderNo = @jobOrderNo AND service = @service", conn))
                        {
                            cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrderNo.Text);
                            cmd.Parameters.AddWithValue("@service", txtJobOrder.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                Log = LogManager.GetLogger("*");
                                Log.Error(ex);
                                return;
                            }
                        }


                        MessageBox.Show("Transaction has been updated");
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }
    }
}
