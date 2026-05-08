using EclatantLaundry.Models;
using System;
using System.Data.SQLite;
using System.Windows;

namespace EclatantLaundry
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow() => InitializeComponent();

        private void RubahPassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtForgotUsername.Text) || string.IsNullOrEmpty(txtNewPassword.Password))
            {
                MessageBox.Show("Semua kolom harus diisi!");
                return;
            }

            if (txtNewPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Password tidak cocok!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Supervisor.FirstOrDefault(u => u.USERNAME == txtForgotUsername.Text);

                    if (user != null)
                    {
                        user.PASSWORD = txtNewPassword.Password;
                        db.SaveChanges();

                        MessageBox.Show("Password berhasil diubah!");

                        MainWindow login = new MainWindow();
                        login.Show();
                        this.Close();

                    }
                    else
                    {
                        MessageBox.Show("Username tidak ditemukan!");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }  
        }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e) => 
            lblUserPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtUsername_LostFocus(object sender, RoutedEventArgs e) { 
            if (string.IsNullOrEmpty(txtForgotUsername.Text)) lblUserPlaceholder.Visibility = Visibility.Visible; }

        private void TxtNewPass_GotFocus(object sender, RoutedEventArgs e) => 
            lblNewPassPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtNewPass_LostFocus(object sender, RoutedEventArgs e) 
        { if (string.IsNullOrEmpty(txtNewPassword.Password)) lblNewPassPlaceholder.Visibility = Visibility.Visible; }

        private void TxtConfirm_GotFocus(object sender, RoutedEventArgs e) => 
            lblConfirmPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtConfirm_LostFocus(object sender, RoutedEventArgs e) 
        { if (string.IsNullOrEmpty(txtConfirmPassword.Password)) lblConfirmPlaceholder.Visibility = Visibility.Visible; }

        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}