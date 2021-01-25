using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;

namespace Goldpoint_Inventory_System.Stock
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class ModifyInvent : UserControl
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public ModifyInvent()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            fillUpType();

        }

        private void LblSearchItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text))
            {
                MessageBox.Show("Item code field is empty!");
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

                            while (reader.Read())
                            {
                                int descriptionIndex = reader.GetOrdinal("description");
                                txtDesc.Text = Convert.ToString(reader.GetValue(descriptionIndex));

                                int typeIndex = reader.GetOrdinal("type");
                                cmbType.SelectedValue = Convert.ToString(reader.GetValue(typeIndex));

                                int brandIndex = reader.GetOrdinal("brand");
                                txtBrand.Text = Convert.ToString(reader.GetValue(brandIndex));

                                int sizeIndex = reader.GetOrdinal("size");
                                txtSize.Text = Convert.ToString(reader.GetValue(sizeIndex));

                                int qtyIndex = reader.GetOrdinal("qty");
                                txtQty.Value = Convert.ToInt32(reader.GetValue(qtyIndex));

                                int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                                txtCriticalLvl.Value = Convert.ToInt32(reader.GetValue(criticalLevelIndex));

                                int remarksIndex = reader.GetOrdinal("remarks");
                                txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));

                                int priceIndex = reader.GetOrdinal("price");
                                txtPrice.Value = Convert.ToDouble(reader.GetValue(priceIndex));

                                int msrpIndex = reader.GetOrdinal("MSRP");
                                txtMSRP.Value = Convert.ToDouble(reader.GetValue(msrpIndex));

                                int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                                txtDealersPrice.Value = Convert.ToDouble(reader.GetValue(dealersPriceIndex));

                            }

                            enableFields();
                            txtItemCode.IsEnabled = false;
                            txtQty.IsEnabled = false;

                            btnSaveItem.IsEnabled = false;
                            btnAddItem.IsEnabled = false;
                            btnUpdateItem.IsEnabled = true;
                            btnDeleteItem.IsEnabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }

                }

            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            enableFields();
            btnSaveItem.IsEnabled = true;
            btnAddItem.IsEnabled = false;
            btnUpdateItem.IsEnabled = false;
            btnDeleteItem.IsEnabled = false;
        }

        private void BtnSaveItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text) || string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(txtCriticalLvl.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtMSRP.Text) || string.IsNullOrEmpty(txtDealersPrice.Text) || string.IsNullOrEmpty(cmbType.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                string sMessageBoxText = "Confirming addition of item";
                string sCaption = "Add Item record?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        int count = 0;
                        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) from InventoryItems where itemCode = @itemCode", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            count = (int)cmd.ExecuteScalar();

                        }
                        if(count > 0)
                        {
                            MessageBox.Show("Item code is already in used. Please check if the item is existing or choose another item code");
                            return;
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT into InventoryItems VALUES (@itemCode, @desc, @type, @brand, @size, @qty, @criticalLevel, @remarks, @price, @msrp, @dealersPrice, '')", conn))
                            {
                                cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                                cmd.Parameters.AddWithValue("@desc", txtDesc.Text);
                                cmd.Parameters.AddWithValue("@type", cmbType.Text);
                                cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                                cmd.Parameters.AddWithValue("@size", txtSize.Text);
                                cmd.Parameters.AddWithValue("@qty", txtQty.Value);
                                cmd.Parameters.AddWithValue("@criticalLevel", txtCriticalLvl.Value);
                                cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                                cmd.Parameters.AddWithValue("@price", txtPrice.Value);
                                cmd.Parameters.AddWithValue("@msrp", txtMSRP.Value);
                                cmd.Parameters.AddWithValue("@dealersPrice", txtDealersPrice.Value);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Item has been added to inventory!");
                                    disableFields();
                                    emptyFields();

                                    txtItemCode.IsEnabled = true;
                                    btnSaveItem.IsEnabled = false;
                                    btnAddItem.IsEnabled = true;
                                    btnUpdateItem.IsEnabled = false;
                                    btnDeleteItem.IsEnabled = false;
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                    Log = LogManager.GetLogger("*");
                                    Log.Error(ex, "Query Error");
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

        private void BtnUpdateItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtCriticalLvl.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtMSRP.Text) || string.IsNullOrEmpty(txtDealersPrice.Text) || string.IsNullOrEmpty(cmbType.Text))
            {
                MessageBox.Show("One or more fields should not be empty!");
            }
            else
            {
                string sMessageBoxText = "Confirming update of item";
                string sCaption = "Update Item record?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems SET description = @desc, type = @type, brand = @brand, size = @size, criticalLevel = @criticalLevel, remarks = @remarks, price = @price, msrp = @msrp, dealersPrice = @dealersPrice where itemCode = @itemCode", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@desc", txtDesc.Text);
                            cmd.Parameters.AddWithValue("@type", cmbType.Text);
                            cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                            cmd.Parameters.AddWithValue("@size", txtSize.Text);
                            cmd.Parameters.AddWithValue("@criticalLevel", txtCriticalLvl.Value);
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@price", txtPrice.Value);
                            cmd.Parameters.AddWithValue("@msrp", txtMSRP.Value);
                            cmd.Parameters.AddWithValue("@dealersPrice", txtDealersPrice.Value);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Item information has been updated!");
                                disableFields();
                                emptyFields();

                                txtItemCode.IsEnabled = true;
                                btnSaveItem.IsEnabled = false;
                                btnAddItem.IsEnabled = true;
                                btnUpdateItem.IsEnabled = false;
                                btnDeleteItem.IsEnabled = false;
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
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }

            }
            disableFields();
        }

        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text))
            {
                MessageBox.Show("Item code field is empty!");
                txtItemCode.Focus();
            }
            else
            {
                string sMessageBoxText = "Confirming deletion of item";
                string sCaption = "Delete Item record?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE from InventoryItems where itemCode = @itemCode", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Item has been deleted!");

                                btnSaveItem.IsEnabled = false;
                                btnAddItem.IsEnabled = true;
                                btnUpdateItem.IsEnabled = false;
                                btnDeleteItem.IsEnabled = false;
                                disableFields();
                                emptyFields();
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
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

        }

        private void BtnAddType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbTypeAdd.Text))
            {
                MessageBox.Show("Type field is empty!");
                cmbType.Focus();
            }
            else
            {
                string sMessageBoxText = "Confirming addition of Type record";
                string sCaption = "Add Type record?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("INSERT into Type (type) VALUES (@type)", conn))
                        {
                            cmd.Parameters.AddWithValue("@type", cmbTypeAdd.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Type record has been added!");
                                cmbTypeAdd.Text = null;
                                fillUpType();
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
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void BtnDeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbTypeAdd.Text))
            {
                MessageBox.Show("Type field is empty!");
                cmbType.Focus();
            }
            else
            {
                string sMessageBoxText = "Confirming deletion of Type record";
                string sCaption = "Delete Type record?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE from Type where type = @type", conn))
                        {
                            cmd.Parameters.AddWithValue("@type", cmbTypeAdd.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Type record has been deleted!");
                                cmbTypeAdd.Text = null;
                                fillUpType();
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
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void disableFields()
        {
            txtItemCode.IsEnabled = true;

            txtDesc.IsEnabled = false;
            cmbType.IsEnabled = false;
            txtBrand.IsEnabled = false;
            txtSize.IsEnabled = false;
            txtQty.IsEnabled = false;
            txtCriticalLvl.IsEnabled = false;
            txtRemarks.IsEnabled = false;
            txtDealersPrice.IsEnabled = false;
            txtPrice.IsEnabled = false;
            txtMSRP.IsEnabled = false;
        }
        private void enableFields()
        {
            txtDesc.IsEnabled = true;
            cmbType.IsEnabled = true;
            txtBrand.IsEnabled = true;
            txtSize.IsEnabled = true;
            txtQty.IsEnabled = true;
            txtCriticalLvl.IsEnabled = true;
            txtRemarks.IsEnabled = true;
            txtDealersPrice.IsEnabled = true;
            txtPrice.IsEnabled = true;
            txtMSRP.IsEnabled = true;
        }

        private void emptyFields()
        {
            txtItemCode.Text = null;
            txtDesc.Text = null;
            cmbType.SelectedIndex = -1;
            txtBrand.Text = null;
            txtSize.Text = null;
            txtQty.Value = 0;
            txtCriticalLvl.Value = 0;
            txtRemarks.Text = null;
            txtDealersPrice.Value = 0;
            txtPrice.Value = 0;
            txtMSRP.Value = 0;
        }

        private void BtnRefresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            disableFields();
            fillUpType();

            txtItemCode.IsEnabled = true;
            btnSaveItem.IsEnabled = false;
            btnAddItem.IsEnabled = true;
            btnUpdateItem.IsEnabled = false;
            btnDeleteItem.IsEnabled = false;
        }

        private void fillUpType()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from Type", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        cmbType.Items.Clear();
                        cmbTypeAdd.Items.Clear();
                        while (reader.Read())
                        {
                            int typeIndex = reader.GetOrdinal("type");
                            cmbType.Items.Add(Convert.ToString(reader.GetValue(typeIndex)));
                            cmbTypeAdd.Items.Add(Convert.ToString(reader.GetValue(typeIndex)));

                        }
                    }
                }
            }
        }

        private void TxtPrice_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //only works for integer values lol
            if (txtPrice.Value != 0)
            {
                txtMSRP.Value = txtPrice.Value * 1.30;
                txtDealersPrice.Value = txtPrice.Value * 1.20;
            }
            else
            {
                txtMSRP.Value = 0;
                txtDealersPrice.Value = 0;
            }
        }
    }
}
