using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            if (IsServerConnected() != true)
            {
                MessageBox.Show("There is no connection with the database, please check your network and see if the device is connected.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TxtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.FontStyle = FontStyles.Normal;
        }

        private void TxtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                txtPassword.FontStyle = FontStyles.Italic;
            }
            else
            {
                txtUsername.FontStyle = FontStyles.Normal;
            }
        }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUsername.FontStyle = FontStyles.Normal;
        }

        private void TxtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsername.FontStyle = FontStyles.Italic;
            }
            else
            {
                txtUsername.FontStyle = FontStyles.Normal;
            }
        }

        private void LblForgot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Please enter username before clicking forgot password");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) from Account where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    int userCount;
                    userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        Hide();
                        new ForgotPassword(txtUsername.Text).ShowDialog();
                        ShowDialog();
                        txtPassword.Password = null;
                        txtUsername.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!");
                        return;
                    }
                }

                Hide();
                new ForgotPassword(txtUsername.Text).ShowDialog();
                ShowDialog();
            }

        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new MainWindow("admin", "Admin").ShowDialog();
            ShowDialog();
        }

        private void BtnExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string sMessageBoxText = "Do you want to exit the application?";
            string sCaption = "Exit";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void BtnMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private static bool IsServerConnected()
        {
            using (SqlConnection connection = DBUtils.GetDBConnection())
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }
    }
}
