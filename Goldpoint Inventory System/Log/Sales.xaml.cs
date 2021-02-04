using NLog;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Goldpoint_Inventory_System.Log
{
    /// <summary>
    /// Interaction logic for Dailyeport.xaml
    /// </summary>
    public partial class Sales : UserControl
    {
        ObservableCollection<SalesDataModel> data = new ObservableCollection<SalesDataModel>();
        ObservableCollection<SalesDataModel> soldMaterials = new ObservableCollection<SalesDataModel>();
        ObservableCollection<SalesDataModel> sales = new ObservableCollection<SalesDataModel>();

        private static Logger Log = LogManager.GetCurrentClassLogger();
        double overallTotal = 0;

        public Sales()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgSoldMaterials.ItemsSource = soldMaterials;
            dgSales.ItemsSource = sales;
            columnSeries.ItemsSource = data;

        }

        private void BtnAddtoList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtTotal.Text) || string.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (txtAmount.Value > txtTotal.Value)
            {
                MessageBox.Show("Amount paid can't be greater than the total amount.");

            }
            else
            {
                string sMessageBoxText = "Confirming addition of Service";
                string sCaption = "Add Service?";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @amount, @total, @status)", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@desc", txtDesc.Text);
                            cmd.Parameters.AddWithValue("@amount", txtAmount.Value);
                            cmd.Parameters.AddWithValue("@total", txtTotal.Value);
                            cmd.Parameters.AddWithValue("@status", "Paid");
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Service successfully added!");
                                txtDesc.Text = null;
                                txtAmount.Value = 0;
                                txtTotal.Value = 0;
                                txtDate.Text = DateTime.Today.ToShortDateString();
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
        }

        private void TxtSearch_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDate.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                soldMaterials.Clear();
                sales.Clear();
                txtSoldMaterialTotal.Value = 0;
                txtAccountReceivable.Value = 0;
                txtCashOnHand.Value = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT item, SUM(qty) as qty, SUM(total) as total from SoldMaterials where date = @date GROUP BY item", conn))
                {
                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        overallTotal = 0;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string description = Convert.ToString(reader.GetValue(reader.GetOrdinal("item")));
                                int qty = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("qty")));
                                double total = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("total")));

                                soldMaterials.Add(new SalesDataModel
                                {
                                    desc = description,
                                    qty = qty,
                                    total = total
                                });

                                overallTotal += total;
                            }
                            txtSoldMaterialTotal.Value = overallTotal;
                        }
                        else
                        {
                            MessageBox.Show("The given date has no sales records!");
                            return;
                        }

                    }
                }

                using (SqlCommand cmd = new SqlCommand("SELECT [desc], amount, total, status from Sales where date = @date", conn))
                {
                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string description = Convert.ToString(reader.GetValue(reader.GetOrdinal("desc")));
                                double amount = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("amount")));
                                double total = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("total")));
                                string status = Convert.ToString(reader.GetValue(reader.GetOrdinal("status")));

                                sales.Add(new SalesDataModel
                                {
                                    desc = description,
                                    amount = amount,
                                    total = total,
                                    status = status
                                });

                                txtAccountReceivable.Value += total - amount;
                                txtCashOnHand.Value += amount;

                            }
                        }
                        else
                        {
                            MessageBox.Show("The given date has no sales records!");
                            return;

                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter date before searching");
                txtDate.Focus();
            }
        }

        private void TxtDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            Syncfusion.Windows.Shared.DateTimeEdit dateTime = (Syncfusion.Windows.Shared.DateTimeEdit)sender;
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if (dateTime.Name == "txtDateFrom")
            {
                using (SqlCommand cmd = new SqlCommand("SELECT cast(date as date) as date, SUM(amount) as amount from Sales where CAST(date AS date) between @dateFrom and @dateTo GROUP BY cast(date as date) order by cast(date as date)", conn))
                {
                    cmd.Parameters.AddWithValue("@dateFrom", dateTime.DateTime);
                    cmd.Parameters.AddWithValue("@dateTo", txtDateTo.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Clear();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                double total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("amount")));
                                DateTime date = Convert.ToDateTime(reader.GetValue(reader.GetOrdinal("date")));
                                data.Add(new SalesDataModel()
                                {
                                    date = date.ToShortDateString(),
                                    total = total
                                });
                            }
                        }
                    }
                }

            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("SELECT cast(date as date) as date, SUM(amount) as amount from Sales where CAST(date AS date) between @dateFrom and @dateTo GROUP BY cast(date as date) order by cast(date as date)", conn))
                {
                    cmd.Parameters.AddWithValue("@dateFrom", txtDateFrom.Text);
                    cmd.Parameters.AddWithValue("@dateTo", dateTime.DateTime);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Clear();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                double total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("amount")));
                                DateTime date = Convert.ToDateTime(reader.GetValue(reader.GetOrdinal("date")));
                                data.Add(new SalesDataModel()
                                {
                                    date = date.ToShortDateString(),
                                    total = total
                                });
                            }
                        }
                    }
                }
            }
        }

        private void BtnReset_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txtDate.Text = DateTime.Today.ToShortDateString();
            txtDesc.Text = null;
            txtAmount.Value = 0;
            txtTotal.Value = 0;
            txtSoldMaterialTotal.Value = 0;
            txtAccountReceivable.Value = 0;
            txtCashOnHand.Value = 0;
            data.Clear();
            soldMaterials.Clear();
            sales.Clear();
        }
    }
}
