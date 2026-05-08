using EclatantLaundry.Models;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System;

namespace EclatantLaundry
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = "";
                txtUsername.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TxtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Username";
                txtUsername.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void TxtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TxtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                txtPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ForgotPasswordWindow forgotPassword = new ForgotPasswordWindow();
            forgotPassword.Show();
            this.Close();
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();
            register.Show();

            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string inputUsername = txtUsername.Text.Trim();
            string inputPassword = txtPassword.Password.Trim();

            try
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Supervisor.FirstOrDefault(u => u.USERNAME == inputUsername && u.PASSWORD == inputPassword);

                    if (user != null)
                    {
                        UserSession.ID_SUPERVISOR = user.ID_SUPERVISOR;
                        UserSession.NAMA_SUPERVISOR = user.NAMA_SUPERVISOR;

                        MessageBox.Show("Login Berhasil! Selamat datang, " + user.NAMA_SUPERVISOR);

                        DashboardWindow dashboard = new DashboardWindow();
                        dashboard.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Username atau Password salah!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Database: " + ex.Message);
            }
        }
    }
}