using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using NLog;

namespace Goldpoint_Inventory_System.Log
{
    /// <summary>
    /// Interaction logic for Dailyeport.xaml
    /// </summary>
    public partial class Sales : UserControl
    {
        public ObservableCollection<SalesDataModel> data = new ObservableCollection<SalesDataModel>();
        ObservableCollection<SalesDataModel> services = new ObservableCollection<SalesDataModel>();
        private static Logger Log = LogManager.GetCurrentClassLogger();
        double overallTotal = 0;

        public Sales()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            dgDailyService.ItemsSource = services;
            columnSeries.ItemsSource = data;

        }

        private void BtnAddtoList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtContent.Text) || string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtTotal.Text))
            {
                MessageBox.Show("One or more fields are empty!");
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
                        using (SqlCommand cmd = new SqlCommand("INSERT into Sales VALUES (@date, @desc, @qty, @total)", conn))
                        {
                            cmd.Parameters.AddWithValue("@date", txtDate.Text);
                            cmd.Parameters.AddWithValue("@desc", txtContent.Text);
                            cmd.Parameters.AddWithValue("@qty", txtQty.Value);
                            cmd.Parameters.AddWithValue("@total", txtTotal.Text);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Service successfully added!");
                                txtContent.Text = null;
                                txtQty.Value = 0;
                                txtTotal.Value = 0;
                                txtDate.Text = DateTime.Today.ToShortDateString();
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

        private void TxtSearch_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDate.Text))
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT desc, qty, SUM(total) as total from Sales where date = @date GROUP BY desc", conn))
                {
                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        services.Clear();
                        overallTotal = 0;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string description = Convert.ToString(reader.GetValue(reader.GetOrdinal("desc")));
                                int qty = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("qty")));
                                double total = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("total")));

                                services.Add(new SalesDataModel
                                {
                                    desc = description,
                                    qty = qty,
                                    total = total
                                });

                                overallTotal += total;
                            }
                            txtOverallTotal.Value = overallTotal;
                        }
                        else
                        {
                            MessageBox.Show("The given date has no sales records!");
                            txtOverallTotal.Value = 0;
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
                using (SqlCommand cmd = new SqlCommand("SELECT cast(date as date) as date, SUM(total) as total from Sales where CAST(date AS date) between @dateFrom and @dateTo GROUP BY cast(date as date) order by cast(date as date)", conn))
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
                                double total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("total")));
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
                using (SqlCommand cmd = new SqlCommand("SELECT cast(date as date) as date, SUM(total) as total from Sales where CAST(date AS date) between @dateFrom and @dateTo GROUP BY cast(date as date) order by cast(date as date)", conn))
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
                                double total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("total")));
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
            txtContent.Text = null;
            txtTotal.Value = 0;
            txtDate.Text = DateTime.Today.ToShortDateString();
            data.Clear();
            services.Clear();
        }
    }
}
