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
using System.Windows.Input;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for JobOrder.xaml
    /// </summary>
    public partial class JobOrder : UserControl
    {
        ObservableCollection<JobOrderDataModel> services = new ObservableCollection<JobOrderDataModel>();
        ObservableCollection<JobOrderDataModel> tarp = new ObservableCollection<JobOrderDataModel>();
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public JobOrder(string fullName)
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            tarpStack.DataContext = new ExpanderListViewModel();
            dgService.ItemsSource = services;
            dgTarpaulin.ItemsSource = tarp;
            txtIssuedBy.Text = fullName;
        }

        private void SearchJobOrders_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            JobOrders jo = new JobOrders();
            jo.Show();
        }

        private void CmbJobOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.ComboBoxItemAdv comboBox = (Syncfusion.Windows.Tools.Controls.ComboBoxItemAdv)cmbJobOrder.SelectedItem;

            if (comboBox != null)
            {
                string value = comboBox.Content.ToString();
                if (value == "Printing, Services, etc.")
                {
                    expServ.IsEnabled = true;
                    expServ.IsExpanded = true;
                    expTarp.IsEnabled = false;
                    emptyTarp();
                }
                else if (value == "Tarpaulin")
                {

                    expServ.IsEnabled = false;
                    expTarp.IsEnabled = true;
                    expTarp.IsExpanded = true;
                    emptyService();
                }
            }
            else
            {
                expServ.IsEnabled = false;
                expTarp.IsEnabled = false;
                emptyService();
                emptyTarp();
            }
        }
        private void radiobuttonPayment(object sender, System.Windows.RoutedEventArgs e)
        {
            RadioButton radiobtn = (RadioButton)sender;
            string value = radiobtn.Content.ToString();
            switch (value)
            {
                case "Unpaid":
                    txtDownpayment.Value = null;
                    txtDownpayment.IsEnabled = false;
                    break;
                case "Downpayment":
                    txtDownpayment.Value = null;
                    txtDownpayment.IsEnabled = true;
                    break;
                case "Paid":
                    txtDownpayment.Value = null;
                    txtDownpayment.IsEnabled = false;
                    break;
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
        private void getJobOrderNo()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if (cmbJobOrder.Text == "Printing, Services, etc.")
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 jobOrderNo from TransactionDetails WHERE jobOrderNo is not null AND DATALENGTH(jobOrderNo) > 0 and service = 'Printing, Services, etc.' ORDER BY jobOrderNo DESC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            txtJobOrder.Value = 1;
                        else
                        {
                            int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                            int jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)) + 1;
                            txtJobOrder.Value = jobOrderNo;
                        }
                        txtJobOrder.IsEnabled = false;
                        cmbJobOrder.IsEnabled = false;
                        btnCancelJobOrder.IsEnabled = false;
                        btnAddJobOrder.IsEnabled = false;
                        btnSaveJobOrder.IsEnabled = true;
                    }
                }
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 jobOrderNo from TransactionDetails WHERE jobOrderNo is not null AND DATALENGTH(jobOrderNo) > 0 and service = 'Tarpaulin' ORDER BY jobOrderNo DESC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            txtJobOrder.Value = 1;
                        else
                        {
                            int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                            int jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)) + 1;
                            txtJobOrder.Value = jobOrderNo;
                        }
                        txtJobOrder.IsEnabled = false;
                        cmbJobOrder.IsEnabled = false;
                        btnCancelJobOrder.IsEnabled = false;
                        btnAddJobOrder.IsEnabled = false;
                        btnSaveJobOrder.IsEnabled = true;
                    }
                }
            }
        }
        private void chkServ_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            string value = chkBox.Content.ToString();
            switch (value)
            {
                case "Delivery Receipt":
                    txtDRNo.Text = null;
                    break;
            }
        }

        private void disableFields()
        {
            txtDate.IsReadOnly = true;
            txtDateDeadline.IsReadOnly = true;
            txtCustName.IsReadOnly = true;
            txtContactNo.IsReadOnly = true;
            txtAddress.IsReadOnly = true;
            txtDownpayment.IsReadOnly = true;
            txtRemarks.IsReadOnly = true;
            rdDownpayment.IsEnabled = false;
            rdPaid.IsEnabled = false;
            rdUnpaid.IsEnabled = false;

            txtDesc.IsReadOnly = true;
            txtDescUnit.IsReadOnly = true;
            txtDescQty.IsReadOnly = true;
            txtMaterial.IsReadOnly = true;
            txtCopy.IsReadOnly = true;
            txtSize.IsReadOnly = true;

            txtFileName.IsReadOnly = true;
            txtTarpX.IsReadOnly = true;
            txtTarpY.IsReadOnly = true;
            txtTarpMedia.IsReadOnly = true;
            txtTarpBorder.IsReadOnly = true;
            txtTarpBorder.IsReadOnly = true;
            txtTarpILET.IsReadOnly = true;
            txtTarpUnitPrice.IsReadOnly = true;

            txtServiceName.IsReadOnly = true;
            txtServiceFee.IsReadOnly = true;
        }
        private void enableFields()
        {
            txtDate.IsReadOnly = false;
            txtDateDeadline.IsReadOnly = false;
            txtCustName.IsReadOnly = false;
            txtContactNo.IsReadOnly = false;
            txtAddress.IsReadOnly = false;
            txtDownpayment.IsReadOnly = false;
            txtRemarks.IsReadOnly = false;
            rdDownpayment.IsEnabled = true;
            rdPaid.IsEnabled = true;
            rdUnpaid.IsEnabled = true;

            txtDesc.IsReadOnly = false;
            txtDescUnit.IsReadOnly = false;
            txtDescQty.IsReadOnly = false;
            txtMaterial.IsReadOnly = false;
            txtCopy.IsReadOnly = false;
            txtSize.IsReadOnly = false;

            txtFileName.IsReadOnly = false;
            txtTarpX.IsReadOnly = false;
            txtTarpY.IsReadOnly = false;
            txtTarpMedia.IsReadOnly = false;
            txtTarpBorder.IsReadOnly = false;
            txtTarpBorder.IsReadOnly = false;
            txtTarpILET.IsReadOnly = false;
            txtTarpUnitPrice.IsReadOnly = false;

            txtServiceName.IsReadOnly = false;
            txtServiceFee.IsReadOnly = false;
        }
        private void emptyFields()
        {
            cmbJobOrder.SelectedIndex = -1;
            txtJobOrder.Value = 0;
            cmbJobOrder.IsEnabled = true;

            txtDate.Text = DateTime.Today.ToShortDateString();
            txtDateDeadline.Text = DateTime.Today.ToShortDateString();
            txtCustName.Text = null;
            txtContactNo.Text = null;
            txtAddress.Document.Blocks.Clear();
            txtRemarks.Document.Blocks.Clear();
            rdUnpaid.IsChecked = true;
            txtDownpayment.Value = 0;
            txtItemTotal.Value = 0;
            txtDownpayment.MaxValue = 0;


        }
        private void emptyService()
        {
            txtDesc.Text = null;
            txtDescUnit.Text = "";
            txtDescUnit.SelectedIndex = -1;
            txtDescQty.Value = 0;
            txtMaterial.Text = null;
            txtCopy.Text = null;
            txtSize.Text = null;
            txtPricePerItem.Value = 0;
        }
        private void emptyTarp()
        {
            txtFileName.Text = null;
            txtTarpQty.Value = 0;
            txtTarpX.Value = 0;
            txtTarpY.Value = 0;
            txtTarpMedia.Text = null;
            txtTarpBorder.Text = null;
            txtTarpILET.Text = null;
            txtTarpUnitPrice.Value = 0;
            txtMediaPrice.Value = 0;

            txtServiceName.Text = null;
            txtServiceFee.Value = 0;
        }
        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            services.Clear();
            tarp.Clear();
            txtJobOrder.IsEnabled = true;
            txtJobOrder.Value = null;
            cmbJobOrder.IsEnabled = true;
            btnCancelJobOrder.IsEnabled = false;
            btnAddJobOrder.IsEnabled = true;
            btnSaveJobOrder.IsEnabled = false;
            btnAddService.IsEnabled = true;
            btnRemoveLastService.IsEnabled = true;
            btnRemoveTarp.IsEnabled = true;
            btnAddTarp.IsEnabled = true;
            cmbJobOrder.SelectedIndex = -1;
            expServ.IsEnabled = false;
            expTarp.IsEnabled = false;
            emptyFields();
            emptyService();
            emptyTarp();
            enableFields();
        }
        private void BtnSearchJO_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(cmbJobOrder.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (string.IsNullOrEmpty(txtJobOrder.Text))
            {
                MessageBox.Show("Please enter Job Order no.");
                txtJobOrder.Focus();
            }
            else
            {
                string service = null;
                int drNo = 0;
                bool exist = false;
                txtItemTotal.Value = 0;
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from TransactionDetails where jobOrderNo = @jobOrderNo and service = @service and inaccessible = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                    cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            int drNoIndex = reader.GetOrdinal("DRNo");
                            int serviceIndex = reader.GetOrdinal("service");
                            int dateIndex = reader.GetOrdinal("date");
                            int deadlineIndex = reader.GetOrdinal("deadline");
                            int customerNameIndex = reader.GetOrdinal("customerName");
                            int addressIndex = reader.GetOrdinal("address");
                            int contactNoIndex = reader.GetOrdinal("contactNo");
                            int remarksIndex = reader.GetOrdinal("remarks");
                            int statusIndex = reader.GetOrdinal("status");

                            txtDate.Text = Convert.ToString(reader.GetValue(dateIndex));
                            cmbJobOrder.Text = Convert.ToString(reader.GetValue(serviceIndex));
                            txtDateDeadline.Text = Convert.ToString(reader.GetValue(deadlineIndex));
                            txtCustName.Text = Convert.ToString(reader.GetValue(customerNameIndex));
                            TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                            textRange.Text = Convert.ToString(reader.GetValue(addressIndex));
                            txtContactNo.Text = Convert.ToString(reader.GetValue(contactNoIndex));
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
                        else
                        {
                            MessageBox.Show("Job order does not exist.");
                            return;
                        }
                    }
                }
                if (exist == true)
                {
                    if (service == "Printing, Services, etc.")
                    {
                        services.Clear();
                        using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial where JobOrderNo = @jobOrderNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
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
                                        unitPrice = Convert.ToDouble(reader.GetValue(totalPerItemIndex)),
                                        amount = Convert.ToDouble(reader.GetValue(totalPerItemIndex))
                                    });

                                    txtItemTotal.Value += Convert.ToDouble(reader.GetValue(totalPerItemIndex));
                                }
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TarpMaterial where JobOrderNo = @jobOrderNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
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
                                        size = Convert.ToString(reader.GetValue(sizeIndex)),
                                        media = Convert.ToString(reader.GetValue(mediaIndex)),
                                        border = Convert.ToString(reader.GetValue(borderIndex)),
                                        ILET = Convert.ToString(reader.GetValue(iLETIndex)),
                                        unitPrice = Convert.ToDouble(reader.GetValue(unitPriceIndex)),
                                        amount = (double)(Convert.ToDouble(reader.GetValue(unitPriceIndex)) * Convert.ToInt32(reader.GetValue(qtyIndex)))
                                    });

                                    txtItemTotal.Value += (double)(Convert.ToDouble(reader.GetValue(unitPriceIndex)) * Convert.ToInt32(reader.GetValue(qtyIndex)));

                                }
                            }
                        }
                    }


                    txtJobOrder.IsEnabled = false;
                    btnCancelJobOrder.IsEnabled = true;
                    btnAddJobOrder.IsEnabled = false;
                    btnSaveJobOrder.IsEnabled = false;
                    btnAddService.IsEnabled = false;
                    btnRemoveLastService.IsEnabled = false;
                    btnAddTarp.IsEnabled = false;
                    btnRemoveTarp.IsEnabled = false;
                    disableFields();
                }
            }
        }
        private void BtnCancelJobOrder_Click(object sender, RoutedEventArgs e)
        {
            //should return materials if cancelled
            if (string.IsNullOrEmpty(txtJobOrder.Text) || string.IsNullOrEmpty(cmbJobOrder.Text))
            {
                MessageBox.Show("Please input Job Order No before cancelling.");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                bool exist = false;
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) from TransactionDetails where jobOrderNo = @jobOrderNo and service = @service and inaccessible = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                    cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            exist = true;
                        }
                        else
                        {
                            MessageBox.Show("Job order does not exist");
                            return;
                        }
                    }
                }
                if (exist)
                {
                    string sMessageBoxText = "Confirm cancellation of Job Order";
                    string sCaption = "Cancel Job Order?";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            using (SqlCommand cmd = new SqlCommand("UPDATE TransactionDetails SET inaccessible = 0 where jobOrderNo = @jobOrderNo and service = @service", conn))
                            {
                                cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
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
                            MessageBox.Show("Job Order has been cancelled successfully");
                            emptyFields();
                            emptyService();
                            emptyTarp();
                            txtJobOrder.IsEnabled = true;
                            btnCancelJobOrder.IsEnabled = false;
                            btnAddJobOrder.IsEnabled = true;
                            btnSaveJobOrder.IsEnabled = true;
                            btnAddService.IsEnabled = true;
                            btnRemoveLastService.IsEnabled = true;
                            btnAddTarp.IsEnabled = true;
                            btnRemoveTarp.IsEnabled = true;

                            break;
                        case MessageBoxResult.No:
                            return;
                        case MessageBoxResult.Cancel:
                            return;
                    }

                }
            }
        }
        private void BtnAddJobOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbJobOrder.Text) || cmbJobOrder.SelectedIndex == -1)
            {
                MessageBox.Show("Please select service before adding");
            }
            else
            {
                getJobOrderNo();
            }


        }
        private void BtnSaveJobOrder_Click(object sender, RoutedEventArgs e)
        {
            string address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd).Text;
            string remarks = new TextRange(txtRemarks.Document.ContentStart, txtRemarks.Document.ContentEnd).Text;
            if (string.IsNullOrEmpty(txtJobOrder.Text))
            {
                MessageBox.Show("Please enter job order");
            }
            else if (string.IsNullOrEmpty(cmbJobOrder.Text))
            {
                MessageBox.Show("Please select job order");
            }
            else if (cmbJobOrder.Text == "Printing, Services, etc." && services.Count == 0)
            {
                MessageBox.Show("Item list is empty!");
            }
            else if (cmbJobOrder.Text == "Tarpaulin" && tarp.Count == 0)
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
                string sMessageBoxText = "Confirm addition of Job Order";
                string sCaption = "Add Job Order?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        bool success = true;
                        if (chkDR.IsChecked == true)
                            getDRNo();
                        getJobOrderNo();
                        if (cmbJobOrder.Text == "Printing, Services, etc.")
                        {
                            foreach (var item in services)
                            {

                                using (SqlCommand cmd = new SqlCommand("INSERT into ServiceMaterial VALUES (@jobOrderNo, @description, @unit, @qty, @material, @copy, @size, @totalPerItem)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                                    cmd.Parameters.AddWithValue("@description", item.Description);
                                    cmd.Parameters.AddWithValue("@unit", item.unit);
                                    cmd.Parameters.AddWithValue("@qty", item.qty);
                                    cmd.Parameters.AddWithValue("@material", item.material);
                                    cmd.Parameters.AddWithValue("@copy", item.copy);
                                    cmd.Parameters.AddWithValue("@size", item.size);
                                    cmd.Parameters.AddWithValue("@totalPerItem", item.amount);
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
                                        return;
                                    }
                                }

                            }


                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (drNo, jobOrderNo, service, date, deadline, customerName, issuedBy, contactNo, address, remarks, status, claimed, inaccessible) VALUES (@drNo, @jobOrderNo, @service, @date, @deadline, @customerName, @issuedBy, @contactNo, @address, @remarks, @status, 'Unclaimed', 1)", conn))
                            {
                                if (string.IsNullOrEmpty(txtDRNo.Text))
                                {
                                    cmd.Parameters.AddWithValue("@drNo", DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                                }
                                cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@deadline", txtDateDeadline.Text);
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                                cmd.Parameters.AddWithValue("@issuedBy", txtIssuedBy.Text);
                                cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);
                                cmd.Parameters.AddWithValue("@address", address);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                }
                                if (rdUnpaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");

                                }
                                if (rdDownpayment.IsChecked == true)
                                {
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
                                    return;
                                }
                            }
                        }
                        else if (cmbJobOrder.Text == "Tarpaulin")
                        {

                            foreach (var tarp in tarp)
                            {
                                if (tarp.tarpSize == "")
                                {
                                    using (SqlCommand cmd = new SqlCommand("INSERT into TarpMaterial (jobOrderNo, fileName, qty, unitPrice) VALUES (@jobOrderNo, @fileName, @qty, @unitPrice)", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                                        cmd.Parameters.AddWithValue("@fileName", tarp.fileName);
                                        cmd.Parameters.AddWithValue("@qty", tarp.tarpQty);
                                        cmd.Parameters.AddWithValue("@unitPrice", tarp.unitPrice);
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
                                    using (SqlCommand cmd = new SqlCommand("INSERT into TarpMaterial VALUES (@jobOrderNo, @fileName, @qty, @size, @media, @border, @iLET, @unitPrice)", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                                        cmd.Parameters.AddWithValue("@fileName", tarp.fileName);
                                        cmd.Parameters.AddWithValue("@qty", tarp.tarpQty);
                                        cmd.Parameters.AddWithValue("@size", tarp.tarpSize);
                                        cmd.Parameters.AddWithValue("@media", tarp.media);
                                        cmd.Parameters.AddWithValue("@border", tarp.border);
                                        cmd.Parameters.AddWithValue("@iLET", tarp.ILET);
                                        cmd.Parameters.AddWithValue("@unitPrice", tarp.unitPrice);
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

                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (drNo, jobOrderNo, service, date, deadline, customerName, issuedBy, contactNo, address, remarks, status, claimed, inaccessible) VALUES (@drNo, @jobOrderNo, @service, @date, @deadline, @customerName, @issuedBy, @contactNo, @address, @remarks, @status, 'Unclaimed', 1)", conn))
                            {
                                if (string.IsNullOrEmpty(txtDRNo.Text))
                                {
                                    cmd.Parameters.AddWithValue("@drNo", DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                                }
                                cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Value);
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@deadline", txtDateDeadline.Text);
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
                                cmd.Parameters.AddWithValue("@issuedBy", txtIssuedBy.Text);
                                cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);
                                cmd.Parameters.AddWithValue("@address", address);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                }
                                if (rdUnpaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");

                                }
                                if (rdDownpayment.IsChecked == true)
                                {
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

                        }
                        if (success)
                        {
                            if (!string.IsNullOrEmpty(txtDRNo.Text))
                            {
                                promptPrintDR();
                            }
                            promptPrintJobOrder();
                            string jobOrder = null;
                            if (cmbJobOrder.Text == "Printing, Services, etc.")
                            {
                                jobOrder = "Services";
                            }
                            else
                            {
                                jobOrder = "Tarpaulin";
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @amount, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@desc", "JO[" + jobOrder + "]: " + txtJobOrder.Value);
                                if (rdDownpayment.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@amount", txtDownpayment.Value);
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");

                                }
                                else if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@amount", txtItemTotal.Value);
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                }
                                else if (rdUnpaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@amount", 0);
                                    cmd.Parameters.AddWithValue("@status", "Unpaid");
                                }
                                cmd.Parameters.AddWithValue("@total", txtItemTotal.Value);

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
                                cmd.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with Job Order No: " + txtJobOrder.Value + ", had an " + cmbJobOrder.Text + " transaction amounting to Php " + txtItemTotal.Text);
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
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                if (!string.IsNullOrEmpty(txtJobOrder.Text))
                                {
                                    cmd.Parameters.AddWithValue("@receiptNo", txtJobOrder.Text);
                                }
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@total", txtItemTotal.Value);
                                if (rdPaid.IsChecked == true)
                                {
                                    cmd.Parameters.AddWithValue("@paidAmt", txtItemTotal.Value);
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
                            MessageBox.Show("Job Order has been added");
                        }
                        txtJobOrder.IsEnabled = true;
                        btnCancelJobOrder.IsEnabled = false;
                        btnAddJobOrder.IsEnabled = true;
                        btnSaveJobOrder.IsEnabled = false;
                        emptyFields();
                        emptyService();
                        emptyTarp();
                        services.Clear();
                        tarp.Clear();
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtDescQty.Text) || string.IsNullOrEmpty(txtDescUnit.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (txtDescQty.Value == 0)
            {
                MessageBox.Show("Please set description quantity to any greater than 0");
            }
            else
            {

                services.Add(new JobOrderDataModel
                {
                    Description = txtDesc.Text,
                    qty = (int)txtDescQty.Value,
                    unit = txtDescUnit.Text,
                    material = txtMaterial.Text,
                    copy = txtCopy.Text,
                    size = txtSize.Text,
                    unitPrice = (double)txtPricePerItem.Value,
                    amount = (double)(txtPricePerItem.Value * txtDescQty.Value)
                });

                txtItemTotal.Value += (double)(txtPricePerItem.Value * txtDescQty.Value);
                txtDownpayment.MaxValue = (double)txtItemTotal.Value;

                txtMaterial.Text = null;
                txtCopy.Text = null;
                txtSize.Text = null;
                txtPricePerItem.Value = 0;
            }
        }
        private void BtnRemoveLastService_Click(object sender, RoutedEventArgs e)
        {
            if (services.Count == 0)
            {
                MessageBox.Show("Service/Material list is empty");
            }
            else
            {
                double amount = services[services.Count - 1].amount;
                txtItemTotal.Value -= amount;
                services.RemoveAt(services.Count - 1);
            }
        }
        private void BtnAddTarp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text) || string.IsNullOrEmpty(txtTarpQty.Text) || string.IsNullOrEmpty(txtTarpX.Text) || string.IsNullOrEmpty(txtTarpY.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                tarp.Add(new JobOrderDataModel
                {
                    fileName = txtFileName.Text,
                    tarpQty = (int)txtTarpQty.Value,
                    tarpSize = txtTarpX.Value.ToString() + " x " + txtTarpY.Value.ToString(),
                    media = txtTarpMedia.Text,
                    border = txtTarpBorder.Text,
                    ILET = txtTarpILET.Text,
                    unitPrice = (double)txtTarpUnitPrice.Value,
                    amount = (double)(txtTarpUnitPrice.Value * txtTarpQty.Value)
                });
                txtItemTotal.Value += (double)(txtTarpUnitPrice.Value * txtTarpQty.Value);
                txtDownpayment.MaxValue = (double)txtItemTotal.Value;
                emptyTarp();
            }
        }
        private void BtnAddFee_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServiceName.Text))
            {
                MessageBox.Show("One or more field are empty!");
            }
            else if (txtServiceFee.Value == 0)
            {
                MessageBox.Show("Please set service fee to anything greater than zero.");
            }
            else
            {
                tarp.Add(new JobOrderDataModel
                {
                    fileName = txtServiceName.Text,
                    tarpQty = 1,
                    unitPrice = (double)txtServiceFee.Value,
                    amount = (double)txtServiceFee.Value * 1,
                    tarpSize = "",
                    media = "",
                    border = "",
                    ILET = "",

                });

                txtItemTotal.Value += (double)txtServiceFee.Value;
                txtDownpayment.MaxValue = (double)txtItemTotal.Value;

                txtServiceName.Text = null;
                txtServiceFee.Value = 0;
            }
        }
        private void BtnRemoveTarp_Click(object sender, RoutedEventArgs e)
        {
            if (tarp.Count == 0)
            {
                MessageBox.Show("Tarpaulin list is empty");
            }
            else
            {
                double amount = tarp[tarp.Count - 1].amount;
                txtItemTotal.Value -= amount;
                tarp.RemoveAt(tarp.Count - 1);
            }
        }
        private void TxtMediaPrice_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (txtTarpX.Value != 0 && txtTarpY.Value != 0)
            {
                txtTarpUnitPrice.Value = (txtTarpX.Value * txtTarpY.Value * txtMediaPrice.Value);
            }
            else
            {
                txtTarpUnitPrice.Value = 0;
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
                                if (string.IsNullOrEmpty(txtJobOrder.Text) || txtJobOrder.Value == 0)
                                {
                                    textSelection = document.Find("<j.o no>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = "";
                                }
                                else
                                {
                                    textSelection = document.Find("<j.o no>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtJobOrder.Text;
                                }
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
                                            if (string.IsNullOrEmpty(txtJobOrder.Text) || txtJobOrder.Value == 0)
                                            {
                                                textSelection = document2.Find("<j.o no>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = "";
                                            }
                                            else
                                            {
                                                textSelection = document.Find("<j.o no>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = txtJobOrder.Text;
                                            }
                                            textSelection = document2.Find("<date>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDate.Text;
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
                                            if (string.IsNullOrEmpty(txtJobOrder.Text) || txtJobOrder.Value == 0)
                                            {
                                                textSelection = document2.Find("<j.o no>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = "";
                                            }
                                            else
                                            {
                                                textSelection = document.Find("<j.o no>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = txtJobOrder.Text;
                                            }
                                            textSelection = document2.Find("<date>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDate.Text;
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
        private void promptPrintJobOrder()
        {
            string sMessageBoxText = "Do you want to print the Job Order Receipt?";
            string sCaption = "Print Job Order Receipt";
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
                        if (cmbJobOrder.Text == "Printing, Services, etc.")
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
                                    textRange.Text = txtJobOrder.Value.ToString();
                                    textSelection = document.Find("<deadline>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtDateDeadline.Text;
                                    textSelection = document.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtDate.Text;
                                    textSelection = document.Find("<address>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    TextRange address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                    textRange.Text = address.Text;
                                    textSelection = document.Find("<contact no>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtContactNo.Text;
                                    textSelection = document.Find("<total>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtItemTotal.Text;
                                    textSelection = document.Find("<total>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtItemTotal.Text;
                                    if ((txtItemTotal.Value - txtDownpayment.Value) > 0)
                                    {
                                        textSelection = document.Find("<balance>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = (txtItemTotal.Value - txtDownpayment.Value).ToString();
                                        textSelection = document.Find("<downpayment>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = txtDownpayment.Text;
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
                                            textRange.Text = txtJobOrder.Value.ToString();
                                            textSelection = document2.Find("<deadline>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDateDeadline.Text;
                                            textSelection = document2.Find("<date>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDate.Text;
                                            textSelection = document2.Find("<address>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = address.Text;
                                            textSelection = document2.Find("<contact no>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtContactNo.Text;
                                            textSelection = document2.Find("<total>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtItemTotal.Text;
                                            textSelection = document2.Find("<total>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtItemTotal.Text;
                                            if ((txtItemTotal.Value - txtDownpayment.Value) > 0)
                                            {
                                                textSelection = document2.Find("<balance>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = (txtItemTotal.Value - txtDownpayment.Value).ToString();
                                                textSelection = document2.Find("<downpayment>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = txtDownpayment.Text;
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
                                            textRange.Text = item.Description;

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
                                            textRange.Text = item.Description;

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
                        else if (cmbJobOrder.Text == "Tarpaulin")
                        {
                            try
                            {
                                using (WordDocument document = new WordDocument("Templates/job-order-tarpaulin-template.docx", FormatType.Docx))
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
                                    textRange.Text = txtJobOrder.Value.ToString();
                                    textSelection = document.Find("<deadline>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtDateDeadline.Text;
                                    textSelection = document.Find("<date>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtDate.Text;
                                    textSelection = document.Find("<address>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    TextRange address = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                                    textRange.Text = address.Text;
                                    textSelection = document.Find("<contact no>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtContactNo.Text;
                                    textSelection = document.Find("<total>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtItemTotal.Text;
                                    textSelection = document.Find("<total>", false, true);
                                    textRange = textSelection.GetAsOneRange();
                                    textRange.Text = txtItemTotal.Text;
                                    if ((txtItemTotal.Value - txtDownpayment.Value) > 0)
                                    {
                                        textSelection = document.Find("<balance>", false, true);
                                        textRange = textSelection.GetAsOneRange();
                                        textRange.Text = (txtItemTotal.Value - txtDownpayment.Value).ToString();
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
                                            textRange.Text = txtJobOrder.Value.ToString();
                                            textSelection = document2.Find("<deadline>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDateDeadline.Text;
                                            textSelection = document2.Find("<date>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtDate.Text;
                                            textSelection = document2.Find("<address>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = address.Text;
                                            textSelection = document2.Find("<contact no>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtContactNo.Text;
                                            textSelection = document2.Find("<total>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtItemTotal.Text;
                                            textSelection = document2.Find("<total>", false, true);
                                            textRange = textSelection.GetAsOneRange();
                                            textRange.Text = txtItemTotal.Text;
                                            if ((txtItemTotal.Value - txtDownpayment.Value) > 0)
                                            {
                                                textSelection = document2.Find("<balance>", false, true);
                                                textRange = textSelection.GetAsOneRange();
                                                textRange.Text = (txtItemTotal.Value - txtDownpayment.Value).ToString();
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
                    }
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void ChkDR_Checked(object sender, RoutedEventArgs e)
        {
            getDRNo();
        }
    }
}
