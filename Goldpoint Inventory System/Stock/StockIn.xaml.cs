using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;

namespace Goldpoint_Inventory_System.Stock
{

    public partial class StockIn : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public StockIn()
        {
            InitializeComponent();

            dgStockIn.ItemsSource = items;
        }

        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(txtDesc.Text))
            {
                MessageBox.Show("One or more fields is empty!");
            }
            else if(txtQty.Value == 0)
            {
                MessageBox.Show("Please change quantity value to anything greater than zero.");
            }
            else
            {
                var found = items.FirstOrDefault(x => txtItemCode.Text == x.itemCode);

                if (found != null)
                {
                    if (rdReplacementYes.IsChecked == true)
                        found.replacement = "Yes";
                    else
                        found.replacement = "No";

                    items.Add(new ItemDataModel
                    {
                        itemCode = found.itemCode,
                        description = found.description,
                        type = found.type,
                        brand = found.brand,
                        size = found.size,
                        qty = Convert.ToInt32(txtQty.Value),
                        remarks = txtRemarks.Text,
                        replacement = found.replacement,
                        fastMoving = cmbFastMoving.Text
                    });
                    //there is no refresh for syncfusion datagrid soo have to work it out somew
                    foreach (var item in items.Where(x => txtItemCode.Text == x.itemCode).ToList())
                    {
                        items.Remove(item);
                        break;
                    }
                }
                else
                {
                    string replacement = "No";
                    if (rdReplacementYes.IsChecked == true)
                        replacement = "Yes";
                    items.Add(new ItemDataModel
                    {
                        itemCode = txtItemCode.Text,
                        description = txtDesc.Text,
                        type = txtType.Text,
                        brand = txtBrand.Text,
                        size = txtSize.Text,
                        qty = Convert.ToInt32(txtQty.Value),
                        remarks = txtRemarks.Text,
                        replacement = replacement,
                        fastMoving = cmbFastMoving.Text
                    });
                }

                searched = false;
                emptyFields();


            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDate.Text))
            {
                MessageBox.Show("Please enter date before adding to stock");
            }
            else if (items.Count == 0)
            {
                MessageBox.Show("Item list is empty!");
            }
            else
            {
                string sMessageBoxText = "Confirming replenishment of Inventory";
                string sCaption = "Replenish Inventory?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        bool success = false;
                        foreach (var item in items)
                        {
                            if (item.replacement == "Yes")
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems set remarks = @remarks, fastMoving = @fastMoving where itemCode = @itemCode", conn))
                                {
                                    cmd.Parameters.AddWithValue("@remarks", item.remarks);
                                    cmd.Parameters.AddWithValue("@fastMoving", item.fastMoving);
                                    cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
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
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE InventoryItems set qty = qty + @qty, remarks = @remarks, fastMoving = @fastMoving where itemCode = @itemCode", conn))
                                {
                                    cmd.Parameters.AddWithValue("@qty", item.qty);
                                    cmd.Parameters.AddWithValue("@remarks", item.remarks);
                                    cmd.Parameters.AddWithValue("@fastMoving", item.fastMoving);
                                    cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
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
                                        return;

                                    }
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("INSERT into ImportDetails (date, itemCode, qty, remarks, fastMoving, replacement) VALUES (@date, @itemCode, @qty, @remarks, @fastMoving, @replacement)", conn))
                            {
                                cmd.Parameters.AddWithValue("@qty", item.qty);
                                cmd.Parameters.AddWithValue("@remarks", item.remarks);
                                cmd.Parameters.AddWithValue("@fastMoving", item.fastMoving);
                                cmd.Parameters.AddWithValue("@itemCode", item.itemCode);
                                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                                cmd.Parameters.AddWithValue("@replacement", item.replacement);
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
                        }
                        if (success)
                        {
                            MessageBox.Show("Item(s) has been replenished!");
                            emptyFields();
                            items.Clear();
                        }
                        break;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }
        private void BtnSearchItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                                txtType.Text = Convert.ToString(reader.GetValue(typeIndex));

                                int brandIndex = reader.GetOrdinal("brand");
                                txtBrand.Text = Convert.ToString(reader.GetValue(brandIndex));

                                int sizeIndex = reader.GetOrdinal("size");
                                txtSize.Text = Convert.ToString(reader.GetValue(sizeIndex));

                                int remarksIndex = reader.GetOrdinal("remarks");
                                txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));

                                searched = true;

                            }
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }

                }

            }
        }
        private void BtnRefresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            items.Clear();
        }
        private void emptyFields()
        {
            txtItemCode.Text = null;
            txtDesc.Text = null;
            txtQty.Value = 0;
            txtSize.Text = null;
            txtType.Text = null;
            txtBrand.Text = null;
            txtRemarks.Text = null;
            rdReplacementNo.IsChecked = true;
            cmbFastMoving.SelectedIndex = 3;
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
                txtRemarks.Text = null;
                searched = false;
            }
        }

        private void TxtItemCode_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
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
                                txtType.Text = Convert.ToString(reader.GetValue(typeIndex));

                                int brandIndex = reader.GetOrdinal("brand");
                                txtBrand.Text = Convert.ToString(reader.GetValue(brandIndex));

                                int sizeIndex = reader.GetOrdinal("size");
                                txtSize.Text = Convert.ToString(reader.GetValue(sizeIndex));

                                int remarksIndex = reader.GetOrdinal("remarks");
                                txtRemarks.Text = Convert.ToString(reader.GetValue(remarksIndex));

                                searched = true;

                            }
                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }

                }
            }
        }
    }
}
