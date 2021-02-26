using System;
using System.Data.SqlClient;
using System.Windows;
using NLog;
using System.Windows.Input;

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public Login()
        {
            InitializeComponent();

            if (IsServerConnected() != true)
            {
                MessageBox.Show("There is no connection with the database, please check your network and see if the device is connected.");
                btnLogin.IsEnabled = false;
                lblForgot.IsEnabled = false;
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

             if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("One or more fields are empty!");
                return;
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                Nullable<int> loginAttempts;
                string fullName = "";
                string adminLevel = "";
                using (SqlCommand cmd = new SqlCommand("SELECT tries FROM Account WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    loginAttempts = Convert.ToInt32(cmd.ExecuteScalar());
                }
                if (loginAttempts < 5)
                {
                    string un = txtUsername.Text;
                    string pw = txtPassword.Password;


                    using (SqlCommand cmd = new SqlCommand("SELECT username,  (firstName + ' ' +  lastName) as fullName, adminLevel from Account where username = @username AND password = @password", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", un);
                        cmd.Parameters.AddWithValue("@password", pw);
                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            fullName = Convert.ToString(reader.GetValue(reader.GetOrdinal("fullName")));
                            adminLevel = Convert.ToString(reader.GetValue(reader.GetOrdinal("adminLevel")));

                            reader.Close();
                            reader.Dispose();
                            using (SqlCommand cmd2 = new SqlCommand("UPDATE Account SET tries = 0", conn))
                            {
                                cmd2.ExecuteNonQuery();
                                MessageBox.Show("Login Successful");
                                Log = LogManager.GetLogger("userLogin");
                                Log.Info(" Account Name: " + txtUsername.Text + " has logged in.");
                            }

                        }
                        else
                        {
                            reader.Close();
                            reader.Dispose();
                            using (SqlCommand cmd2 = new SqlCommand("UPDATE Account SET tries = tries + 1 where username = @username", conn))
                            {
                                cmd2.Parameters.AddWithValue("@username", un);
                                cmd2.ExecuteNonQuery();
                            }
                            MessageBox.Show("Username or Password is invalid");
                            return;
                        }
                    }
                    txtPassword.Password = null;
                    txtUsername.Text = null;
                    Hide();
                    new MainWindow(adminLevel,fullName).ShowDialog();
                    ShowDialog();

                }
                else
                {
                    string sMessageBoxText = "Due to multiple login attempts, your account has been locked. \nPlease unlock it to continue.";
                    string sCaption = "Account Recovery";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            Hide();
                            new ForgotPassword(txtUsername.Text).ShowDialog();
                            ShowDialog();
                            break;

                        case MessageBoxResult.No: break;
                    }
                }
            }
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
