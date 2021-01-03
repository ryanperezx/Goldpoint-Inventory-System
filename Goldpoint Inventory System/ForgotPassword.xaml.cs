using NLog;
using System;
using System.Data.SqlClient;
using System.Windows;

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {
        string user, question, answer;
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public ForgotPassword(string username)
        {
            InitializeComponent();
        }
    }
}
