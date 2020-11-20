using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Goldpoint_Inventory_System.Log
{
    /// <summary>
    /// Interaction logic for ImportDetails.xaml
    /// </summary>
    public partial class ImportDetails : UserControl
    {
        ObservableCollection<ItemDataModel> details = new ObservableCollection<ItemDataModel>();
        public ImportDetails()
        {
            InitializeComponent();
            dgDetails.ItemsSource = details;
        }

        private void BtnRefresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            details.Clear();
            txtItemCode.Text = null;
            txtDateFrom.Text = DateTime.Today.ToString();
            txtDateTo.Text = DateTime.Today.ToString();
        }


        private void TxtDateFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemCode.Text))
            {
                //if itemcode is valid then..
                //if itemcode and txtdateto has text then..
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                int exist = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) from ImportDetails where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    exist = (int)cmd.ExecuteScalar();
                }
                if (exist == 0)
                {
                    if (!string.IsNullOrEmpty(txtDateTo.Text))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where CAST(id.date AS datetime) between @dateFrom and @dateTo", conn))
                        {
                            cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                            cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
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
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.date = @date", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDateFrom.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
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
                else
                {
                    if (!string.IsNullOrEmpty(txtDateTo.Text))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.itemCode = @itemCode and CAST(id.date AS datetime) between @dateFrom and @dateTo", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                            cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
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
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where ud.itemCode = @itemCode and ud.date = @date", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@date", txtDateFrom.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
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
            }
            else if (!string.IsNullOrEmpty(txtDateTo.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.itemCode = @itemCode and CAST(id.date AS datetime) between @dateFrom and @dateTo", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                    cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            details.Clear();
                            while (reader.Read())
                            {
                                int itemCodeIndex = reader.GetOrdinal("itemCode");
                                int dateIndex = reader.GetOrdinal("date");
                                int descriptionIndex = reader.GetOrdinal("description");
                                int qtyIndex = reader.GetOrdinal("qty");
                                int replacementIndex = reader.GetOrdinal("replacement");
                                int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                int remarksIndex = reader.GetOrdinal("remarks");

                                details.Add(new ItemDataModel
                                {
                                    date = Convert.ToString(reader.GetValue(dateIndex)),
                                    itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                    replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                    fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
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

        private void TxtDateTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemCode.Text))
            {
                //if itemcode is valid then..
                //if itemcode and txtdateto has text then..
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                int exist = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) from ImportDetails where itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    exist = (int)cmd.ExecuteScalar();
                }
                if (exist == 0)
                {
                    if (!string.IsNullOrEmpty(txtDateTo.Text))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where CAST(id.date AS datetime) between @dateFrom and @dateTo", conn))
                        {
                            cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                            cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
                                        });

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.date = @date", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDateTo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
                                        });

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtDateTo.Text))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.itemCode = @itemCode and CAST(id.date AS datetime) between @dateFrom and @dateTo", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                            cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
                                        });

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.itemCode = @itemCode and id.date = @date", conn))
                        {
                            cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                            cmd.Parameters.AddWithValue("@date", txtDateTo.Text);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    details.Clear();
                                    while (reader.Read())
                                    {
                                        int itemCodeIndex = reader.GetOrdinal("itemCode");
                                        int dateIndex = reader.GetOrdinal("date");
                                        int descriptionIndex = reader.GetOrdinal("description");
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int replacementIndex = reader.GetOrdinal("replacement");
                                        int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                        int remarksIndex = reader.GetOrdinal("remarks");

                                        details.Add(new ItemDataModel
                                        {
                                            date = Convert.ToString(reader.GetValue(dateIndex)),
                                            itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                            description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                            qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                            replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                            fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
                                        });

                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(txtDateFrom.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * from ImportDetails where itemCode = @itemCode and CAST(date AS datetime) between @dateFrom and @dateTo", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                    cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            details.Clear();
                            while (reader.Read())
                            {
                                int itemCodeIndex = reader.GetOrdinal("itemCode");
                                int dateIndex = reader.GetOrdinal("date");
                                int descriptionIndex = reader.GetOrdinal("description");
                                int qtyIndex = reader.GetOrdinal("qty");
                                int replacementIndex = reader.GetOrdinal("replacement");
                                int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                int remarksIndex = reader.GetOrdinal("remarks");

                                details.Add(new ItemDataModel
                                {
                                    date = Convert.ToString(reader.GetValue(dateIndex)),
                                    itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                    replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                    fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
                                });

                            }
                        }
                    }
                }
            }
        }

        private void BtnSearchItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                using (SqlCommand cmd = new SqlCommand("SELECT id.date, ii.description, id.qty, id.replacement, id.fastMoving, id.remarks from ImportDetails id LEFT JOIN InventoryItems ii on id.itemCode = ii.itemCode where id.itemCode = @itemCode", conn))
                {
                    cmd.Parameters.AddWithValue("@itemCode", txtItemCode.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            details.Clear();
                            while (reader.Read())
                            {
                                int dateIndex = reader.GetOrdinal("date");
                                int descriptionIndex = reader.GetOrdinal("description");
                                int qtyIndex = reader.GetOrdinal("qty");
                                int replacementIndex = reader.GetOrdinal("replacement");
                                int fastMovingIndex = reader.GetOrdinal("fastMoving");
                                int remarksIndex = reader.GetOrdinal("remarks");

                                details.Add(new ItemDataModel
                                {
                                    date = Convert.ToString(reader.GetValue(dateIndex)),
                                    itemCode = txtItemCode.Text,
                                    description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                    qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                    replacement = Convert.ToString(reader.GetValue(replacementIndex)),
                                    fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex))
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

        private void getReplenishedInventoryToday()
        {

        }
    }
}
