using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Goldpoint_Inventory_System.Stock
{
    /// <summary>
    /// Interaction logic for InventCheck.xaml
    /// </summary>
    public partial class InventCheck : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();

        public InventCheck()
        {
            InitializeComponent();
            fillUpType();
            dgInventory.ItemsSource = items;
            fillUpInventory();
        }

        private void BtnSearchItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                            items.Clear();
                            while (reader.Read())
                            {
                                int descriptionIndex = reader.GetOrdinal("description");
                                int typeIndex = reader.GetOrdinal("type");
                                int brandIndex = reader.GetOrdinal("brand");
                                int sizeIndex = reader.GetOrdinal("size");
                                int qtyIndex = reader.GetOrdinal("qty");
                                int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                                int remarksIndex = reader.GetOrdinal("remarks");
                                int priceIndex = reader.GetOrdinal("price");
                                int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                                int msrpIndex = reader.GetOrdinal("MSRP");
                                int fastMovingIndex = reader.GetOrdinal("fastMoving");

                                bool criticalState = false;
                                if (Convert.ToInt32(reader.GetValue(qtyIndex)) < Convert.ToInt32(reader.GetValue(criticalLevelIndex)))
                                {
                                    criticalState = true;
                                }



                                items.Add(new ItemDataModel
                                {
                                    itemCode = txtItemCode.Text,
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    type = Convert.ToString(reader.GetValue(typeIndex)),
                                    brand = Convert.ToString(reader.GetValue(brandIndex)),
                                    size = Convert.ToString(reader.GetValue(sizeIndex)),
                                    qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                    criticalLvl = Convert.ToInt32(reader.GetValue(criticalLevelIndex)),
                                    criticalState = criticalState,
                                    msrp = Convert.ToDouble(reader.GetValue(msrpIndex)),
                                    price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                    dealersPrice = Convert.ToDouble(reader.GetValue(dealersPriceIndex)),
                                    fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex)),
                                    remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                });

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

        private void ChkCritical_Checked(object sender, RoutedEventArgs e)
        {
            txtDesc.Text = null;
            txtItemCode.Text = null;
            items.Clear();
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where qty < criticalLevel ORDER BY itemCode DESC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        items.Clear();
                        while (reader.Read())
                        {
                            int itemCodeIndex = reader.GetOrdinal("itemCode");
                            int descriptionIndex = reader.GetOrdinal("description");
                            int typeIndex = reader.GetOrdinal("type");
                            int brandIndex = reader.GetOrdinal("brand");
                            int sizeIndex = reader.GetOrdinal("size");
                            int qtyIndex = reader.GetOrdinal("qty");
                            int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                            int remarksIndex = reader.GetOrdinal("remarks");
                            int priceIndex = reader.GetOrdinal("price");
                            int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                            int msrpIndex = reader.GetOrdinal("MSRP");
                            int fastMovingIndex = reader.GetOrdinal("fastMoving");

                            items.Add(new ItemDataModel
                            {
                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                criticalLvl = Convert.ToInt32(reader.GetValue(criticalLevelIndex)),
                                criticalState = true,
                                msrp = Convert.ToDouble(reader.GetValue(msrpIndex)),
                                price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                dealersPrice = Convert.ToDouble(reader.GetValue(dealersPriceIndex)),
                                fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex)),
                                remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                            });

                        }
                    }
                }
            }
        }

        private void ChkCritical_Unchecked(object sender, RoutedEventArgs e)
        {
            txtDesc.Text = null;
            txtItemCode.Text = null;
            items.Clear();
            fillUpInventory();
        }

        private void fillUpInventory()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems ORDER BY Description ASC", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        items.Clear();
                        while (reader.Read())
                        {
                            int itemCodeIndex = reader.GetOrdinal("itemCode");
                            int descriptionIndex = reader.GetOrdinal("description");
                            int typeIndex = reader.GetOrdinal("type");
                            int brandIndex = reader.GetOrdinal("brand");
                            int sizeIndex = reader.GetOrdinal("size");
                            int qtyIndex = reader.GetOrdinal("qty");
                            int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                            int remarksIndex = reader.GetOrdinal("remarks");
                            int priceIndex = reader.GetOrdinal("price");
                            int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                            int msrpIndex = reader.GetOrdinal("MSRP");
                            int fastMovingIndex = reader.GetOrdinal("fastMoving");

                            bool criticalState = false;
                            if (Convert.ToInt32(reader.GetValue(qtyIndex)) < Convert.ToInt32(reader.GetValue(criticalLevelIndex)))
                            {
                                criticalState = true;
                            }

                            items.Add(new ItemDataModel
                            {
                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                criticalLvl = Convert.ToInt32(reader.GetValue(criticalLevelIndex)),
                                criticalState = criticalState,
                                msrp = Convert.ToDouble(reader.GetValue(msrpIndex)),
                                price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                dealersPrice = Convert.ToDouble(reader.GetValue(dealersPriceIndex)),
                                fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex)),
                                remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                            });

                        }
                    }
                }
            }
        }

        private void TxtDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDesc.Text))
            {
                items.Clear();
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where description LIKE @desc", conn))
                {
                    cmd.Parameters.AddWithValue("@desc", '%' + txtDesc.Text + '%');
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                                int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                                int remarksIndex = reader.GetOrdinal("remarks");
                                int priceIndex = reader.GetOrdinal("price");
                                int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                                int msrpIndex = reader.GetOrdinal("MSRP");
                                int fastMovingIndex = reader.GetOrdinal("fastMoving");

                                bool criticalState = false;
                                if (Convert.ToInt32(reader.GetValue(qtyIndex)) < Convert.ToInt32(reader.GetValue(criticalLevelIndex)))
                                {
                                    criticalState = true;
                                }

                                items.Add(new ItemDataModel
                                {
                                    itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    type = Convert.ToString(reader.GetValue(typeIndex)),
                                    brand = Convert.ToString(reader.GetValue(brandIndex)),
                                    size = Convert.ToString(reader.GetValue(sizeIndex)),
                                    qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                    criticalLvl = Convert.ToInt32(reader.GetValue(criticalLevelIndex)),
                                    criticalState = criticalState,
                                    msrp = Convert.ToDouble(reader.GetValue(msrpIndex)),
                                    price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                    dealersPrice = Convert.ToDouble(reader.GetValue(dealersPriceIndex)),
                                    fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex)),
                                    remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                fillUpInventory();
            }
        }

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = (sender as Syncfusion.Windows.Tools.Controls.ComboBoxAdv).SelectedItem as string;
            if (text != null)
            {
                items.Clear();
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where type = @type", conn))
                {
                    cmd.Parameters.AddWithValue("@type", text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                                int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                                int remarksIndex = reader.GetOrdinal("remarks");
                                int priceIndex = reader.GetOrdinal("price");
                                int dealersPriceIndex = reader.GetOrdinal("dealersPrice");
                                int msrpIndex = reader.GetOrdinal("MSRP");
                                int fastMovingIndex = reader.GetOrdinal("fastMoving");

                                bool criticalState = false;
                                if (Convert.ToInt32(reader.GetValue(qtyIndex)) < Convert.ToInt32(reader.GetValue(criticalLevelIndex)))
                                {
                                    criticalState = true;
                                }

                                items.Add(new ItemDataModel
                                {
                                    itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    type = Convert.ToString(reader.GetValue(typeIndex)),
                                    brand = Convert.ToString(reader.GetValue(brandIndex)),
                                    size = Convert.ToString(reader.GetValue(sizeIndex)),
                                    qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                    criticalLvl = Convert.ToInt32(reader.GetValue(criticalLevelIndex)),
                                    criticalState = criticalState,
                                    msrp = Convert.ToDouble(reader.GetValue(msrpIndex)),
                                    price = Convert.ToDouble(reader.GetValue(priceIndex)),
                                    dealersPrice = Convert.ToDouble(reader.GetValue(dealersPriceIndex)),
                                    fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex)),
                                    remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                });

                            }
                        }
                    }
                }
            }
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
                        while (reader.Read())
                        {
                            int typeIndex = reader.GetOrdinal("type");
                            cmbType.Items.Add(Convert.ToString(reader.GetValue(typeIndex)));
                        }
                    }
                }
            }
        }

        private void BtnRefresh_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtDesc.Text = null;
            txtItemCode.Text = null;
            fillUpType();
            fillUpInventory();
        }
    }
}
