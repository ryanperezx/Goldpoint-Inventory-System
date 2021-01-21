using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
        public JobOrder()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgService.ItemsSource = services;
            dgTarpaulin.ItemsSource = tarp;
            getDRNo();
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
                    expTarp.IsEnabled = false;
                    emptyTarp();
                }
                else if (value == "Tarpaulin")
                {

                    expServ.IsEnabled = false;
                    expTarp.IsEnabled = true;
                    emptyService();
                }
                else
                {
                    expServ.IsEnabled = false;
                    expTarp.IsEnabled = false;
                    emptyService();
                    emptyTarp();
                }
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
        private void getDRNo()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 DRNo from TransactionDetails WHERE TRIM(DRNo) is not null AND DATALENGTH(DRNo) > 0 ORDER BY DRNo DESC", conn))
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
        private void getJobOrderNo()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 jobOrderNo from TransactionDetails WHERE TRIM(jobOrderNo) is not null AND DATALENGTH(jobOrderNo) > 0 and service = 'Printing, Services, etc.' ORDER BY jobOrderNo DESC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        txtJobOrder.Text = "1";
                    else
                    {
                        int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                        int jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)) + 1;
                        txtJobOrder.Text = jobOrderNo.ToString();
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
            txtItemCode.IsReadOnly = true;
            txtMaterial.IsReadOnly = true;
            txtCopy.IsReadOnly = true;
            txtSize.IsReadOnly = true;
            txtItemQty.IsReadOnly = true;

            txtFileName.IsReadOnly = true;
            txtItemQty.IsReadOnly = true;
            txtTarpSize.IsReadOnly = true;
            txtTarpMedia.IsReadOnly = true;
            txtTarpBorder.IsReadOnly = true;
            txtTarpBorder.IsReadOnly = true;
            txtTarpILET.IsReadOnly = true;
            txtTarpUnitPrice.IsReadOnly = true;
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
            txtItemCode.IsReadOnly = false;
            txtMaterial.IsReadOnly = false;
            txtCopy.IsReadOnly = false;
            txtSize.IsReadOnly = false;
            txtItemQty.IsReadOnly = false;

            txtFileName.IsReadOnly = false;
            txtItemQty.IsReadOnly = false;
            txtTarpSize.IsReadOnly = false;
            txtTarpMedia.IsReadOnly = false;
            txtTarpBorder.IsReadOnly = false;
            txtTarpBorder.IsReadOnly = false;
            txtTarpILET.IsReadOnly = false;
            txtTarpUnitPrice.IsReadOnly = false;
        }
        private void emptyFields()
        {
            cmbJobOrder.SelectedIndex = 0;
            txtJobOrder.Text = null;
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
            txtDescUnit.Text = null;
            txtDescQty.Value = 0;
            txtItemCode.Text = null;
            txtMaterial.Text = null;
            txtCopy.Text = null;
            txtSize.Text = null;
            txtItemQty.Text = null;
            txtPricePerItem.Value = 0;
        }
        private void emptyTarp()
        {
            txtFileName.Text = null;
            txtTarpQty.Value = 0;
            txtTarpSize.Text = null;
            txtTarpMedia.Text = null;
            txtTarpBorder.Text = null;
            txtTarpILET.Text = null;
            txtTarpUnitPrice.Value = 0;
        }
        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            getDRNo();
            services.Clear();
            tarp.Clear();
            txtJobOrder.IsEnabled = true;
            txtJobOrder.Text = null;
            cmbJobOrder.IsEnabled = true;
            btnCancelJobOrder.IsEnabled = false;
            btnAddJobOrder.IsEnabled = true;
            btnSaveJobOrder.IsEnabled = false;
            btnAddService.IsEnabled = true;
            btnRemoveLastService.IsEnabled = true;
            btnAddTarp.IsEnabled = true;
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
                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
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
                        using (SqlCommand cmd = new SqlCommand("SELECT * from ServiceMaterial sm INNER JOIN InventoryItems ii on sm.itemCode = ii.itemCode where JobOrderNo = @jobOrderNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
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

                                    txtItemTotal.Value += Convert.ToDouble(reader.GetValue(totalPerItemIndex)) / Convert.ToInt32(reader.GetValue(itemQtyIndex));
                                }
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT * from TarpMaterial where JobOrderNo = @jobOrderNo", conn))
                        {
                            cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
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
                    disableFields();
                }
            }
        }
        private void TxtSearchItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                            txtMaterial.Text = Convert.ToString(reader.GetValue(descriptionIndex));

                            txtItemQty.Value = 0;

                            int sizeIndex = reader.GetOrdinal("size");
                            txtSize.Text = Convert.ToString(reader.GetValue(sizeIndex));

                            int msrpIndex = reader.GetOrdinal("MSRP");
                            txtPricePerItem.Value = Convert.ToDouble(reader.GetValue(msrpIndex));
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }
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
                                cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered!" + ex);
                                }
                            }
                            if (cmbJobOrder.Text == "Printing, Services, etc.")
                            {
                                foreach (var item in services)
                                {
                                    using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems set qty = qty + @qty where itemCode = @itemCode", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@qty", item.itemQty);
                                        cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                        catch (SqlException ex)
                                        {
                                            MessageBox.Show("An error has been encountered!" + ex);
                                            return;
                                        }
                                    }

                                    using (SqlCommand cmd = new SqlCommand("DELETE from ReleasedMaterials where itemCode = @itemCode and DRNo = @drNo", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                        cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                        catch (SqlException ex)
                                        {
                                            MessageBox.Show("An error has been encountered!" + ex);
                                            return;
                                        }
                                    }
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
            if (string.IsNullOrEmpty(cmbJobOrder.Text))
            {
                MessageBox.Show("Please select service before adding");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                if (cmbJobOrder.Text == "Printing, Services, etc.")
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 jobOrderNo from TransactionDetails WHERE TRIM(jobOrderNo) is not null AND DATALENGTH(jobOrderNo) > 0 and service = 'Printing, Services, etc.' ORDER BY jobOrderNo DESC", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                txtJobOrder.Text = "1";
                            else
                            {
                                int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                int jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)) + 1;
                                txtJobOrder.Text = jobOrderNo.ToString();
                            }
                            txtJobOrder.IsEnabled = false;
                            cmbJobOrder.IsEnabled = false;
                            btnCancelJobOrder.IsEnabled = false;
                            btnAddJobOrder.IsEnabled = false;
                            btnSaveJobOrder.IsEnabled = true;
                            getDRNo();
                        }
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 jobOrderNo from TransactionDetails WHERE TRIM(jobOrderNo) is not null AND DATALENGTH(jobOrderNo) > 0 and service = 'Tarpaulin' ORDER BY jobOrderNo DESC", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                txtJobOrder.Text = "1";
                            else
                            {
                                int jobOrderNoIndex = reader.GetOrdinal("jobOrderNo");
                                int jobOrderNo = Convert.ToInt32(reader.GetValue(jobOrderNoIndex)) + 1;
                                txtJobOrder.Text = jobOrderNo.ToString();
                            }
                            txtJobOrder.IsEnabled = false;
                            cmbJobOrder.IsEnabled = false;
                            btnCancelJobOrder.IsEnabled = false;
                            btnAddJobOrder.IsEnabled = false;
                            btnSaveJobOrder.IsEnabled = true;
                            getDRNo();
                        }
                    }
                }
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
            else if (string.IsNullOrEmpty(txtCustName.Text) || string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtDateDeadline.Text) || string.IsNullOrEmpty(address))
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
                        getJobOrderNo();
                        getDRNo();

                        if (cmbJobOrder.Text == "Printing, Services, etc.")
                        {
                            foreach (var item in services)
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems set qty = qty - @qty where itemCode = @itemCode", conn))
                                {
                                    cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                    cmd.Parameters.AddWithValue("@qty", item.itemQty);
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered!" + ex);
                                        success = false;
                                        return;
                                    }
                                }

                                using (SqlCommand cmd = new SqlCommand("INSERT into ServiceMaterial VALUES (@jobOrderNo, @description, @unit, @qty, @itemCode, @material, @copy, @size, @itemQty, @totalPerItem)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                                    cmd.Parameters.AddWithValue("@description", item.Description);
                                    cmd.Parameters.AddWithValue("@unit", item.unit);
                                    cmd.Parameters.AddWithValue("@qty", item.qty);
                                    cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                    cmd.Parameters.AddWithValue("@material", item.material);
                                    cmd.Parameters.AddWithValue("@copy", item.copy);
                                    cmd.Parameters.AddWithValue("@size", item.size);
                                    cmd.Parameters.AddWithValue("@itemQty", item.itemQty);
                                    cmd.Parameters.AddWithValue("@totalPerItem", item.amount);
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered!" + ex);
                                        success = false;
                                        return;
                                    }
                                }

                                using (SqlCommand cmd = new SqlCommand("INSERT into ReleasedMaterials VALUES (@DRNo, @itemCode, @desc, @type, @brand, @size, @qty, @totalPerItem)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
                                    cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                    cmd.Parameters.AddWithValue("@desc", item.material);
                                    cmd.Parameters.AddWithValue("@type", item.type);
                                    cmd.Parameters.AddWithValue("@brand", item.brand);
                                    cmd.Parameters.AddWithValue("@size", item.size);
                                    cmd.Parameters.AddWithValue("@qty", item.itemQty);
                                    cmd.Parameters.AddWithValue("@totalPerItem", item.amount);
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered!" + ex);
                                        success = false;
                                        return;
                                    }
                                }

                            }


                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (drNo, jobOrderNo, service, date, deadline, customerName, contactNo, address, remarks, status, claimed, inaccessible) VALUES (@drNo, @jobOrderNo, @service, @date, @deadline, @customerName, @contactNo, @address, @remarks, @status, 'Unclaimed', 1)", conn))
                            {
                                cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@deadline", txtDateDeadline.Text);
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
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
                                    cmd.Parameters.AddWithValue("@status", "Downpayment");
                                }
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered!" + ex);
                                    success = false;
                                    return;
                                }
                            }
                        }
                        else if (cmbJobOrder.Text == "Tarpaulin")
                        {

                            foreach (var tarp in tarp)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT into TarpMaterial VALUES (@jobOrderNo, @fileName, @qty, @size, @media, @border, @iLET, @unitPrice)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
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
                                        MessageBox.Show("An error has been encountered!" + ex);
                                        success = false;
                                    }
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionDetails (drNo, jobOrderNo, service, date, deadline, customerName, contactNo, address, remarks, status, claimed, inaccessible) VALUES (@drNo, @jobOrderNo, @service, @date, @deadline, @customerName, @contactNo, @address, @remarks, @status, 'Unclaimed', 1)", conn))
                            {
                                cmd.Parameters.AddWithValue("@drNo", txtDRNo.Text);
                                cmd.Parameters.AddWithValue("@jobOrderNo", txtJobOrder.Text);
                                cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@deadline", txtDateDeadline.Text);
                                cmd.Parameters.AddWithValue("@customerName", txtCustName.Text);
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
                                    cmd.Parameters.AddWithValue("@status", "Downpayment");
                                }
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered!" + ex);
                                    success = false;
                                }
                            }

                        }
                        if (success)
                        {
                            if (rdDownpayment.IsChecked == true)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @service, @total, @status)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                    cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                    cmd.Parameters.AddWithValue("@total", txtDownpayment.Value);
                                    cmd.Parameters.AddWithValue("@status", "Downpayment");
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered!" + ex);
                                        success = false;
                                    }
                                }
                            }
                            else if (rdPaid.IsChecked == true)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @service, @total, @status)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                    cmd.Parameters.AddWithValue("@service", cmbJobOrder.Text);
                                    cmd.Parameters.AddWithValue("@total", txtItemTotal.Value);
                                    cmd.Parameters.AddWithValue("@status", "Paid");
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered!" + ex);
                                        success = false;
                                    }
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into TransactionLogs (date, [transaction], remarks) VALUES (@date, @transaction, @remarks)", conn))
                            {
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@transaction", "Customer: " + txtCustName.Text + ", with Job Order No: " + txtJobOrder.Text + ", had an " + cmbJobOrder.Text + " transaction amounting to Php " + txtItemTotal.Text);
                                cmd.Parameters.AddWithValue("@remarks", remarks);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered!" + ex);
                                    success = false;
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into PaymentHist VALUES (@DRNo, @date, @paidAmt, @total, @status)", conn))
                            {
                                cmd.Parameters.AddWithValue("@DRNo", txtDRNo.Text);
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
                                    cmd.Parameters.AddWithValue("@status", "Downpayment");
                                }
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered!" + ex);
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
                        getDRNo();
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
            else if (string.IsNullOrEmpty(txtItemQty.Text) || string.IsNullOrEmpty(txtItemCode.Text))
            {
                MessageBox.Show("One or more fields are empty");
            }
            else if (txtDescQty.Value == 0)
            {
                MessageBox.Show("Please set description quantity to any greater than 0");
            }
            else if (txtItemQty.Value == 0)
            {
                MessageBox.Show("Please set item quantity to any greater than 0");
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
                            int brandIndex = reader.GetOrdinal("brand");
                            int typeIndex = reader.GetOrdinal("type");
                            int qtyIndex = reader.GetOrdinal("qty");
                            if (Convert.ToInt32(reader.GetValue(qtyIndex)) < txtItemQty.Value)
                            {
                                MessageBox.Show("Quantity to be prepared for job order is greater than the stock quantity");
                                return;
                            }

                            services.Add(new JobOrderDataModel
                            {
                                Description = txtDesc.Text,
                                qty = (int)txtDescQty.Value,
                                unit = txtDescUnit.Text,
                                itemCode = txtItemCode.Text,
                                material = txtMaterial.Text,
                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                copy = txtCopy.Text,
                                size = txtSize.Text,
                                unitPrice = (double)txtPricePerItem.Value,
                                itemQty = (int)txtItemQty.Value,
                                amount = (double)(txtPricePerItem.Value * txtItemQty.Value)
                            });

                            txtItemTotal.Value += (txtPricePerItem.Value * txtItemQty.Value);
                            txtDownpayment.MaxValue = (double)txtItemTotal.Value;

                            txtItemCode.Text = null;
                            txtMaterial.Text = null;
                            txtCopy.Text = null;
                            txtSize.Text = null;
                            txtItemQty.Value = 0;
                            txtPricePerItem.Value = 0;
                        }
                    }
                }
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
                services.RemoveAt(services.Count - 1);
            }
        }
        private void BtnAddTarp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text) || string.IsNullOrEmpty(txtTarpQty.Text) || string.IsNullOrEmpty(txtTarpSize.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                tarp.Add(new JobOrderDataModel
                {
                    fileName = txtFileName.Text,
                    tarpQty = (int)txtTarpQty.Value,
                    tarpSize = txtTarpSize.Text,
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
    }
}
