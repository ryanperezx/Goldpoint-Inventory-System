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
        ObservableCollection<JobOrderDataModel> tarp = new ObservableCollection<JobOrderDataModel>();

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
                int drNo = 0;
                double unpaidBalance = 0;
                exPhotocopy.IsEnabled = false;
                exStockOut.IsEnabled = false;
                exJobOrder.IsEnabled = false;
                exJobOrderTarp.IsEnabled = false;
                emptyFields();
                clearServices();
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
                                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                        textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                        txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
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
                        else if (service == "Printing, Services, etc.")
                        {
                            exJobOrder.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial sm INNER JOIN InventoryItems ii on sm.itemCode = ii.itemCode where JobOrderNo = @jobOrderNo", conn))
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
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int materialIndex = reader.GetOrdinal("material");
                                        int copyIndex = reader.GetOrdinal("copy");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int itemQtyIndex = reader.GetOrdinal("itemQty");
                                        int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                        services.Add(new JobOrderDataModel
                                        {
                                            Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            unit = Convert.ToString(reader.GetValue(unitIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            material = Convert.ToString(reader.GetValue(materialIndex)),
                                            copy = Convert.ToString(reader.GetValue(copyIndex)),
                                            size = Convert.ToString(reader.GetValue(sizeIndex)),
                                            itemQty = Convert.ToInt32(reader.GetValue(itemQtyIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(itemQtyIndex))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Tarpaulin")
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
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
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
                                        txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
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
                        else if (service == "Printing, Services, etc.")
                        {
                            exJobOrder.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial sm INNER JOIN InventoryItems ii on sm.itemCode = ii.itemCode where JobOrderNo = @jobOrderNo", conn))
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
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int materialIndex = reader.GetOrdinal("material");
                                        int copyIndex = reader.GetOrdinal("copy");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int itemQtyIndex = reader.GetOrdinal("itemQty");
                                        int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                        services.Add(new JobOrderDataModel
                                        {
                                            Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            unit = Convert.ToString(reader.GetValue(unitIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            material = Convert.ToString(reader.GetValue(materialIndex)),
                                            copy = Convert.ToString(reader.GetValue(copyIndex)),
                                            size = Convert.ToString(reader.GetValue(sizeIndex)),
                                            itemQty = Convert.ToInt32(reader.GetValue(itemQtyIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(itemQtyIndex))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Tarpaulin")
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
                                        int contactNoIndex = reader.GetOrdinal("contactNo");
                                        int remarks = reader.GetOrdinal("remarks");
                                        int orNoIndex = reader.GetOrdinal("ORNo");
                                        int drNoIndex = reader.GetOrdinal("DRNo");
                                        int statusIndex = reader.GetOrdinal("status");

                                        txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                                        txtDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                                        txtCustName.Text = Convert.ToString(reader.GetValue(custNameIndex));
                                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                        textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                                        txtDRNo.Text = Convert.ToString(reader.GetValue(drNoIndex));
                                        txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
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
                        else if (service == "Printing, Services, etc.")
                        {
                            exJobOrder.IsEnabled = true;
                            using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial sm INNER JOIN InventoryItems ii on sm.itemCode = ii.itemCode where JobOrderNo = @jobOrderNo", conn))
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
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int materialIndex = reader.GetOrdinal("material");
                                        int copyIndex = reader.GetOrdinal("copy");
                                        int sizeIndex = reader.GetOrdinal("size");
                                        int itemQtyIndex = reader.GetOrdinal("itemQty");
                                        int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                        services.Add(new JobOrderDataModel
                                        {
                                            Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            unit = Convert.ToString(reader.GetValue(unitIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            material = Convert.ToString(reader.GetValue(materialIndex)),
                                            copy = Convert.ToString(reader.GetValue(copyIndex)),
                                            size = Convert.ToString(reader.GetValue(sizeIndex)),
                                            itemQty = Convert.ToInt32(reader.GetValue(itemQtyIndex)),
                                            unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                            amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(itemQtyIndex))
                                        });
                                    }
                                }
                            }
                        }
                        else if (service == "Tarpaulin")
                        {
                            exJobOrderTarp.IsEnabled = true;
                        }

                        break;
                    case "Job Order (Tarpaulin)":
                        //should be able to issue or and dr here
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo = @jobOrderNo and service = 'Tarpaulin'", conn))
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
                                    exJobOrderTarp.IsEnabled = true;

                                }
                                else
                                {
                                    MessageBox.Show("Job order does not exist.");
                                    exJobOrderTarp.IsEnabled = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case "Job Order (Printing, Services, etc.)":
                        //should be able to issue or and dr here
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo = @jobOrderNo and service = 'Printing, Services, etc.'", conn))
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
                                    exJobOrder.IsEnabled = true;


                                }
                                else
                                {
                                    MessageBox.Show("Job order does not exist.");
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
                        txtUnpaidBalancePayment.Value = txtTotal.Value - unpaidBalance;
                        txtAmount.MaxValue = (double)txtUnpaidBalancePayment.Value;
                        using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial sm INNER JOIN InventoryItems ii on sm.itemCode = ii.itemCode where JobOrderNo = @jobOrderNo", conn))
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
                                    int itemCodeIndex = reader.GetOrdinal("itemCode");
                                    int materialIndex = reader.GetOrdinal("material");
                                    int copyIndex = reader.GetOrdinal("copy");
                                    int sizeIndex = reader.GetOrdinal("size");
                                    int itemQtyIndex = reader.GetOrdinal("itemQty");
                                    int totalPerItemIndex = reader.GetOrdinal("totalPerItem");

                                    services.Add(new JobOrderDataModel
                                    {
                                        Description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                        unit = Convert.ToString(reader.GetValue(unitIndex)),
                                        qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                        itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                        material = Convert.ToString(reader.GetValue(materialIndex)),
                                        copy = Convert.ToString(reader.GetValue(copyIndex)),
                                        size = Convert.ToString(reader.GetValue(sizeIndex)),
                                        itemQty = Convert.ToInt32(reader.GetValue(itemQtyIndex)),
                                        unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                        amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(itemQtyIndex))
                                    });
                                }
                            }
                        }
                        break;
                }
                if (txtUnpaidBalancePayment.Value == 0)
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
        }

        private void emptyFields()
        {
            txtAmount.Value = 0;
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
                                    cmd.Parameters.AddWithValue("@status", "Downpayment");
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
                                    txtUnpaidBalancePayment.Value -= txtAmount.Value;
                                    txtAmount.MaxValue = (double) txtUnpaidBalancePayment.Value;
                                    if (txtUnpaidBalancePayment.Value == 0)
                                    {
                                        btnPayment.IsEnabled = false;
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
                                                MessageBox.Show("An error has been encountered! " + ex);

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
                                                MessageBox.Show("An error has been encountered! " + ex);

                                            }

                                        }
                                    }

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
