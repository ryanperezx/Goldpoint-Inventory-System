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
            SqlConnection conn = DBUtils.GetDBConnection();
            user = username;
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT securityQuestion, answer FROM Account WHERE username = @username", conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                using (SqlDataReader reader = cmd.ExecuteReader())
                    if (reader.Read())
                    {
                        int securityQuestionIndex = reader.GetOrdinal("securityQuestion");
                        question = Convert.ToString(reader.GetValue(securityQuestionIndex));

                        int answerIndex = reader.GetOrdinal("answer");
                        answer = Convert.ToString(reader.GetValue(answerIndex));

                        txtSecurityQuestion.Text = question;
                    }
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAnswer.Password))
            {
                MessageBox.Show("Please answer the security question before changing password");
            }
            else if (string.IsNullOrEmpty(txtNewPassword.Password) || string.IsNullOrEmpty(txtConfirmPassword.Password))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                if (txtNewPassword.Password.Equals(txtConfirmPassword.Password))
                {
                    if (answer.Equals(txtAnswer.Password))
                    {
                        string sMessageBoxText = "Are all fields checked?";
                        string sCaption = "Confirm Change Password";
                        MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                        MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                        switch (dr)
                        {
                            case MessageBoxResult.Yes:
                                SqlConnection conn = DBUtils.GetDBConnection();
                                conn.Open();
                                using (SqlCommand cmd1 = new SqlCommand("UPDATE Account SET password = @password, tries = 0 WHERE username = @username", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@username", user);
                                    cmd1.Parameters.AddWithValue("@password", txtNewPassword.Password);
                                    try
                                    {
                                        cmd1.ExecuteNonQuery();
                                        MessageBox.Show("Password has been changed.");
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex, "Query Error");
                                    }


                                    Log = LogManager.GetLogger("AccountLog");
                                    Log.Info("Account: " + user + " changed their password");
                                }
                                this.DialogResult = false;

                                break;


                            case MessageBoxResult.No: break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect security question answer. Please try again.");
                    }

                }
                else
                {
                    MessageBox.Show("New password and confirmation password do not match.");
                }
            }
        }
    }
}
