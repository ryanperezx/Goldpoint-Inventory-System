﻿using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Goldpoint_Inventory_System.Stock
{
    /// <summary>
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class ModifyInvent : UserControl
    {
        public ModifyInvent()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
        }

        private void LblSearchItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                                cmbType.Text = Convert.ToString(reader.GetValue(typeIndex));

                                int brandIndex = reader.GetOrdinal("brand");
                                txtBrand.Text = Convert.ToString(reader.GetValue(brandIndex));

                                int sizeIndex = reader.GetOrdinal("size");
                                txtSize.Text = Convert.ToString(reader.GetValue(brandIndex));

                                int qtyIndex = reader.GetOrdinal("qty");
                                txtQty.Text = Convert.ToString(reader.GetValue(qtyIndex));

                                int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                                txtCriticalLvl.Text = Convert.ToString(reader.GetValue(criticalLevelIndex));

                                int remarksIndex = reader.GetOrdinal("remarks");
                                txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));

                                int priceIndex = reader.GetOrdinal("price");
                                txtPrice.Text = Convert.ToString(reader.GetValue(priceIndex));

                                int msrpIndex = reader.GetOrdinal("MSRP");
                                txtMSRP.Text = Convert.ToString(reader.GetValue(msrpIndex));

                                int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                                txtDealersPrice.Text = Convert.ToString(reader.GetValue(dealersPriceIndex));

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
            if (string.IsNullOrEmpty(txtItemCode.Text) && string.IsNullOrEmpty(txtDesc.Text) && string.IsNullOrEmpty(txtQty.Text) && string.IsNullOrEmpty(txtCriticalLvl.Text) && string.IsNullOrEmpty(txtPrice.Text) && string.IsNullOrEmpty(txtMSRP.Text) && string.IsNullOrEmpty(txtDealersPrice.Text) && string.IsNullOrEmpty(cmbType.Text))
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
                        using (SqlCommand cmd = new SqlCommand("INSERT into InventoryItems VALUES (@itemCode, @desc, @type, @brand, @size, @qty, @criticalLvl, @remarks, @price, @msrp, @dealersPrice, '')", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@desc", txtDesc.Text);
                            cmd.Parameters.AddWithValue("@type", cmbType.Text);
                            cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                            cmd.Parameters.AddWithValue("@size", txtSize.Text);
                            cmd.Parameters.AddWithValue("@qty", txtQty.Text);
                            cmd.Parameters.AddWithValue("@criticalLevel", txtCriticalLvl.Text);
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                            cmd.Parameters.AddWithValue("@msrp", txtMSRP.Text);
                            cmd.Parameters.AddWithValue("@dealersPrice", txtDealersPrice.Text);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Item has been added to inventory!");
                                disableFields();
                                emptyFields();

                                btnSaveItem.IsEnabled = false;
                                btnAddItem.IsEnabled = true;
                                btnUpdateItem.IsEnabled = false;
                                btnDeleteItem.IsEnabled = false;
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Error has been encountered");
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
            if (string.IsNullOrEmpty(txtDesc.Text) && string.IsNullOrEmpty(txtCriticalLvl.Text) && string.IsNullOrEmpty(txtPrice.Text) && string.IsNullOrEmpty(txtMSRP.Text) && string.IsNullOrEmpty(txtDealersPrice.Text) && string.IsNullOrEmpty(cmbType.Text))
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
                        using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems SET desc = @desc, type = @type, brand = @brand, size = @size, criticalLevel = @criticalLevel, remarks = @remarks, price = @price, msrp = @msrp, dealersPrice = @dealersPrice) where itemCode = @itemCode", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@desc", txtDesc.Text);
                            cmd.Parameters.AddWithValue("@type", cmbType.Text);
                            cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                            cmd.Parameters.AddWithValue("@size", txtSize.Text);
                            cmd.Parameters.AddWithValue("@criticalLevel", txtCriticalLvl.Text);
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                            cmd.Parameters.AddWithValue("@msrp", txtMSRP.Text);
                            cmd.Parameters.AddWithValue("@dealersPrice", txtDealersPrice.Text);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Item information has been updated!");
                                disableFields();
                                emptyFields();

                                btnSaveItem.IsEnabled = false;
                                btnAddItem.IsEnabled = true;
                                btnUpdateItem.IsEnabled = false;
                                btnDeleteItem.IsEnabled = false;
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Error has been encountered");
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
                                MessageBox.Show("Error has been encountered");
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
            cmbType.Text = null;
            txtBrand.Text = null;
            txtSize.Text = null;
            txtQty.Text = null;
            txtCriticalLvl.Text = null;
            txtRemarks.Text = null;
            txtDealersPrice.Text = null;
            txtPrice.Text = null;
            txtMSRP.Text = null;
        }
    }
}
