﻿using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Office2019Colorful.WPF;
using System;
using System.Collections.Generic;
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
using NLog;
using System.Data.SqlClient;
using System.ComponentModel;

namespace Goldpoint_Inventory_System
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : UserControl
    {
        public string passwordStatus;
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public Account()
        {
            InitializeComponent();

            Office2019ColorfulThemeSettings themeSettings = new Office2019ColorfulThemeSettings();
            themeSettings.PrimaryBackground = new SolidColorBrush(Colors.DarkGoldenrod);
            themeSettings.PrimaryForeground = new SolidColorBrush(Colors.White);
            themeSettings.BodyFontSize = 16;
            themeSettings.HeaderFontSize = 14;
            themeSettings.SubHeaderFontSize = 14;
            themeSettings.TitleFontSize = 14;
            themeSettings.SubTitleFontSize = 14;
            themeSettings.FontFamily = new FontFamily("Calibri");
            SfSkinManager.RegisterThemeSettings("Office2019Colorful", themeSettings);
            SfSkinManager.SetTheme(this, new Theme("Office2019Colorful"));
        }

        private void BtnSearchUsername_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("Username field is empty!");
                txtUser.Focus();
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select COUNT(1) from Account where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUser.Text);
                    int userCount;
                    userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        using (SqlCommand cmd1 = new SqlCommand("SELECT firstName, lastName, securityQuestion from Account where username = @username", conn))
                        {
                            cmd1.Parameters.AddWithValue("@username", txtUser.Text);
                            using (SqlDataReader reader = cmd1.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    try
                                    {
                                        reader.Read();
                                        int firstNameIndex = reader.GetOrdinal("firstName");
                                        string firstName = Convert.ToString(reader.GetValue(firstNameIndex));

                                        int lastNameIndex = reader.GetOrdinal("lastName");
                                        string lastName = Convert.ToString(reader.GetValue(lastNameIndex));

                                        int securityQuestionIndex = reader.GetOrdinal("securityQuestion");
                                        string securityQuestion = Convert.ToString(reader.GetValue(securityQuestionIndex));

                                        txtFirstName.Text = firstName;
                                        txtLastName.Text = lastName;
                                        cmbQuestion.Text = securityQuestion;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("An error has been encountered! Log has been updated with the error");
                                        Log = LogManager.GetLogger("*");
                                        Log.Error(ex);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!");
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(cmbQuestion.Text) || string.IsNullOrEmpty(txtAns.Password) || string.IsNullOrEmpty(txtConfirmPass.Password) || string.IsNullOrEmpty(txtPass.Password) || string.IsNullOrEmpty(txtPass.Password))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                if (txtPass.Password.Equals(txtConfirmPass.Password))
                {
                    SqlConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Select COUNT(1) from Account where username = @username", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUser.Text);
                        int userCount;
                        userCount = (int)cmd.ExecuteScalar();
                        if (userCount > 0)
                        {
                            MessageBox.Show("User already exist!");
                        }
                        else
                        {
                            string sMessageBoxText = "Are all fields correct?";
                            string sCaption = "Add Account";
                            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                            switch (dr)
                            {
                                case MessageBoxResult.Yes:
                                    using (SqlCommand cmd1 = new SqlCommand("INSERT into Account (firstName, lastName, username, password, securityQuestion, answer, tries, userLevel) VALUES (@firstName, @lastName, @username, @password, @securityQuestion, @answer, 0, @userLevel)", conn))
                                    {
                                        cmd1.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                                        cmd1.Parameters.AddWithValue("@lastName", txtLastName.Text);
                                        cmd1.Parameters.AddWithValue("@username", txtUser.Text);
                                        cmd1.Parameters.AddWithValue("@password", txtPass.Password);
                                        cmd1.Parameters.AddWithValue("@securityQuestion", cmbQuestion.Text);
                                        cmd1.Parameters.AddWithValue("@answer", txtAns.Password);
                                        if (cmbUserLevel.Text.Equals("Secretary"))
                                            cmd1.Parameters.AddWithValue("@userLevel", 0);
                                        else
                                            cmd1.Parameters.AddWithValue("@userLevel", 1);

                                        try
                                        {
                                            cmd1.ExecuteNonQuery();
                                            MessageBox.Show("Registered successfully");
                                            Log = LogManager.GetLogger("registerAccount");
                                            Log.Info("Account " + txtUser.Text + " has been registered!");
                                            emptyFields();

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
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Your password and confirmation password do not match.");
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("Username field is empty!");
            }
            else
            {
                SqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select COUNT(1) from Account where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUser.Text);
                    int userCount;
                    userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        string sMessageBoxText = "Do you want to delete the account?";
                        string sCaption = "Delete Account";
                        MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                        MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                        switch (dr)
                        {
                            case MessageBoxResult.Yes:
                                using (SqlCommand command = new SqlCommand("DELETE from Account where username= @username", conn))
                                {
                                    command.Parameters.AddWithValue("@username", txtUser.Text);
                                    try
                                    {
                                        command.ExecuteNonQuery();
                                        MessageBox.Show("Account has been deleted!");
                                        Log = LogManager.GetLogger("deleteAccount");
                                        Log.Info("Account " + txtUser.Text + " has been deleted!");
                                        emptyFields();
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
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!");

                    }
                }
            }
        }

        private void BtnReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
        }

        private void emptyFields()
        {
            txtFirstName.Text = null;
            txtLastName.Text = null;
            txtUser.Text = null;
            txtPass.Password = null;
            txtConfirmPass.Password = null;
            cmbQuestion.SelectedIndex = -1;
            cmbUserLevel.SelectedIndex = -1;
            txtAns.Password = null;
        }
    }
}
