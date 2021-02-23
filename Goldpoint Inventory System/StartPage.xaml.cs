using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Office2019Colorful.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : UserControl
    {
        ObservableCollection<ItemDataModel> items = new ObservableCollection<ItemDataModel>();
        ObservableCollection<UserTransactionDataModel> customer = new ObservableCollection<UserTransactionDataModel>();
        ObservableCollection<UserTransactionDataModel> pendings = new ObservableCollection<UserTransactionDataModel>();

        public StartPage(string fullname)
        {
            InitializeComponent();
            dgCritical.ItemsSource = items;
            dgRecentTransact.ItemsSource = customer;
            dgPending.ItemsSource = pendings;
            fillUpItems();
            fillUpRecentTransact();
            forClaiming();
        }

        private void BtnRefresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            fillUpItems();
            fillUpRecentTransact();
            forClaiming();
        }

        private void fillUpItems()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * from InventoryItems where qty <= criticalLevel", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        items.Clear();
                        while (reader.Read())
                        {
                            int itemCodeIndex = reader.GetOrdinal("itemCode");
                            int descriptionIndex = reader.GetOrdinal("description"); ;
                            int qtyIndex = reader.GetOrdinal("qty");
                            int criticalLevelIndex = reader.GetOrdinal("criticalLevel");
                            int fastMovingIndex = reader.GetOrdinal("fastMoving");

                            items.Add(new ItemDataModel
                            {
                                itemCode = Convert.ToString(reader.GetValue(itemCodeIndex)),
                                description = Convert.ToString(reader.GetValue(descriptionIndex)),
                                qty = Convert.ToInt32(reader.GetValue(qtyIndex)),
                                criticalLvl = Convert.ToInt32(reader.GetValue(criticalLevelIndex)),
                                criticalState = true,
                                fastMoving = Convert.ToString(reader.GetValue(fastMovingIndex)),
                            });

                        }
                    }
                }
            }
        }
        private void forClaiming()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            //get all non company use
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT service, deadline, customerName, jobOrderNo, status, claimed, issuedBy from TransactionDetails WHERE (claimed = 'Unclaimed' AND TRY_CAST(deadline as date) >= CONVERT(VARCHAR(10), getdate(), 23)) AND jobOrderNo > 0 AND inaccessible = 1", conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    pendings.Clear();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int serviceIndex = reader.GetOrdinal("service");
                            int deadlineIndex = reader.GetOrdinal("deadline");
                            int custNameIndex = reader.GetOrdinal("customerName");
                            int jobOrderNoIndex = reader.GetOrdinal("JobOrderNo");
                            int statusIndex = reader.GetOrdinal("status");
                            int claimedIndex = reader.GetOrdinal("claimed");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");

                            pendings.Add(new UserTransactionDataModel
                            {
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                customerName = Convert.ToString(reader.GetValue(custNameIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                receiptNo = Convert.ToString(reader.GetValue(jobOrderNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                                claimed = Convert.ToString(reader.GetValue(claimedIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex))
                            });

                        }
                    }

                }
            }
        }      
        private void fillUpRecentTransact()
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            //get all non company use
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT td.service, td.deadline, td.customerName, td.DRNo, td.status, td.remarks, td.issuedBy, ph.total from TransactionDetails td INNER JOIN PaymentHist ph on td.DRNo = ph.DRNo WHERE ph.date = @date", conn))
            {
                cmd.Parameters.AddWithValue("@date", DateTime.Today.ToShortDateString());
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    customer.Clear();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int serviceIndex = reader.GetOrdinal("service");
                            int deadlineIndex = reader.GetOrdinal("deadline");
                            int custNameIndex = reader.GetOrdinal("customerName");
                            int drNoIndex = reader.GetOrdinal("DRNo");
                            int statusIndex = reader.GetOrdinal("status");
                            int totalIndex = reader.GetOrdinal("total");
                            int remarksIndex = reader.GetOrdinal("remarks");
                            int issuedByIndex = reader.GetOrdinal("issuedBy");

                            customer.Add(new UserTransactionDataModel
                            {
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                customerName = Convert.ToString(reader.GetValue(custNameIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                receiptNo = Convert.ToString(reader.GetValue(drNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                                total = Convert.ToDouble(reader.GetValue(totalIndex)),
                                remarks = Convert.ToString(reader.GetValue(remarksIndex)),
                                issuedBy = Convert.ToString(reader.GetValue(issuedByIndex))
                            });

                        }
                    }

                }
            }
            //get all company use
            using (SqlCommand cmd = new SqlCommand("SELECT td.date, td.deadline, td.drNo, td.service, td.customerName, td.address, td.contactNo, td.status from TransactionDetails td LEFT JOIN PaymentHist ph on ph.DRNo = td.DRNo where ph.DRNo IS null and td.date = @date", conn))
            {
                cmd.Parameters.AddWithValue("@date", DateTime.Today.ToShortDateString());
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int serviceIndex = reader.GetOrdinal("service");
                            int deadlineIndex = reader.GetOrdinal("deadline");
                            int custNameIndex = reader.GetOrdinal("customerName");
                            int drNoIndex = reader.GetOrdinal("DRNo");
                            int statusIndex = reader.GetOrdinal("status");

                            customer.Add(new UserTransactionDataModel
                            {
                                deadline = Convert.ToString(reader.GetValue(deadlineIndex)),
                                customerName = Convert.ToString(reader.GetValue(custNameIndex)),
                                service = Convert.ToString(reader.GetValue(serviceIndex)),
                                receiptNo = Convert.ToString(reader.GetValue(drNoIndex)),
                                status = Convert.ToString(reader.GetValue(statusIndex)),
                            });

                        }
                    }

                }
            }
        }
    }
}
