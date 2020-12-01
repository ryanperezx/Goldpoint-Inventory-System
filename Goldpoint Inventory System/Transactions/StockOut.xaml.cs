using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Goldpoint_Inventory_System.Transactions
{
    /// <summary>
    /// Interaction logic for StockOut.xaml
    /// </summary>
    public partial class StockOut : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        public StockOut()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgStockOut.ItemsSource = items;
            //to avoid null error
            txtQty.TextChanged += TxtQty_TextChanged;

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
                case "Down payment":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = true;
                    break;
                case "Paid":
                    txtDownpayment.Text = null;
                    txtDownpayment.IsEnabled = false;
                    break;
                case "Company Use":
                    break;
            }
        }

        private void checkboxService(object sender, RoutedEventArgs e)
        {
            if (chkCompany.IsChecked == true)
            {
                chkDR.IsChecked = false;
                chkInv.IsChecked = false;
                chkOR.IsChecked = false;
            }
            else
            {
                CheckBox chkbox = (CheckBox)sender;
                string value = chkbox.Content.ToString();

                if (chkbox.IsChecked == true && value == "Company Use")
                {
                    chkDR.IsChecked = false;
                    chkInv.IsChecked = false;
                    chkOR.IsChecked = false;

                    txtInv.IsEnabled = false;
                    txtDRNo.IsEnabled = false;
                    txtORNo.IsEnabled = false;
                }

                if (chkbox.IsChecked == true && value == "Delivery Receipt")
                {
                    txtDRNo.IsEnabled = true;
                }
                if (chkbox.IsChecked == true && value == "Original Receipt")
                {
                    txtORNo.IsEnabled = true;
                }
                if (chkbox.IsChecked == true && value == "Invoice")
                {
                    txtInv.IsEnabled = true;
                }

            }
        }

        private void unCheckBoxService(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            string value = chkbox.Content.ToString();

            if (chkbox.IsChecked == false && value == "Invoice")
            {
                txtInv.IsEnabled = false;
            }
            if (chkbox.IsChecked == false && value == "Delivery Receipt")
            {
                txtDRNo.IsEnabled = false;
            }
            if (chkbox.IsChecked == false && value == "Original Receipt")
            {
                txtORNo.IsEnabled = false;
            }
        }

        private void BtnAddtoList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemCode.Text) || string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("One or more fields are empty!");
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
                            int typeIndex = reader.GetOrdinal("type");
                            int brandIndex = reader.GetOrdinal("brand");
                            int sizeIndex = reader.GetOrdinal("size");
                            int qtyIndex = reader.GetOrdinal("qty");
                            if (Convert.ToInt32(reader.GetValue(qtyIndex)) < txtQty.Value)
                            {
                                MessageBox.Show("Quantity to be stock out is greater than the stock quantity");
                                return;
                            }

                            items.Add(new ItemDataModel
                            {
                                itemCode = txtItemCode.Text,
                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                type = Convert.ToString(reader.GetValue(typeIndex)),
                                brand = Convert.ToString(reader.GetValue(brandIndex)),
                                size = Convert.ToString(reader.GetValue(sizeIndex)),
                                qty = (int)txtQty.Value,
                                totalPerItem = (double)txtTotalPerItem.Value
                            });

                        }
                        else
                        {
                            MessageBox.Show("Item does not exist in the inventory");
                        }
                    }
                }
            }
        }

        private void BtnSearchItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

                                int msrpIndex = reader.GetOrdinal("MSRP");
                                txtItemPrice.Value = Convert.ToInt32(reader.GetValue(msrpIndex));

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

        private void TxtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtItemPrice.Value > 0 && txtQty.Value > 0)
            {
                txtTotalPerItem.Value = txtItemPrice.Value * txtQty.Value;
            }
            else
            {
                txtTotalPerItem.Value = 0;
            }
        }
    }
}
