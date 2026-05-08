using System;
using System.Windows;
using EclatantLaundry.Models; // Pastikan namespace ini sesuai dengan folder Models kamu
using Microsoft.EntityFrameworkCore;

namespace EclatantLaundry
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Daftar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegUsername.Text) ||
                string.IsNullOrWhiteSpace(txtRegNama.Text) ||
                string.IsNullOrWhiteSpace(txtRegPassword.Password) ||
                string.IsNullOrWhiteSpace(txtRegConfirm.Password))
            {
                MessageBox.Show("Mohon lengkapi semua data!");
                return;
            }

            if (txtRegPassword.Password != txtRegConfirm.Password)
            {
                MessageBox.Show("Password tidak cocok!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var baru = new Supervisor
                    {
                        USERNAME = txtRegUsername.Text,
                        NAMA_SUPERVISOR = txtRegNama.Text,
                        PASSWORD = txtRegPassword.Password,

                    };

                    db.Supervisor.Add(baru);
                    db.SaveChanges();

                    MessageBox.Show("Registrasi Berhasil! Silakan Login.");

                    MainWindow login = new MainWindow();
                    login.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null) errorMsg += "\nDetail: " + ex.InnerException.Message;
                MessageBox.Show("Error Database: " + errorMsg);
            }
        }
        private void Login_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void TxtPassword_GotFocus(object sender, RoutedEventArgs e) => 
            lblPassPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtPassword_LostFocus(object sender, RoutedEventArgs e) 
        { if (string.IsNullOrEmpty(txtRegPassword.Password)) lblPassPlaceholder.Visibility = Visibility.Visible; }

        private void TxtConfirm_GotFocus(object sender, RoutedEventArgs e) 
            => lblConfirmPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtConfirm_LostFocus(object sender, RoutedEventArgs e) 
        { if (string.IsNullOrEmpty(txtRegConfirm.Password)) lblConfirmPlaceholder.Visibility = Visibility.Visible; }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e) 
            => lblUserPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtUsername_LostFocus(object sender, RoutedEventArgs e) 
        { if (string.IsNullOrEmpty(txtRegUsername.Text)) lblUserPlaceholder.Visibility = Visibility.Visible; }

        private void TxtNama_GotFocus(object sender, RoutedEventArgs e) 
            => lblNamaPlaceholder.Visibility = Visibility.Collapsed;

        private void TxtNama_LostFocus(object sender, RoutedEventArgs e) 
        { if (string.IsNullOrEmpty(txtRegNama.Text)) lblNamaPlaceholder.Visibility = Visibility.Visible; }
    }
}