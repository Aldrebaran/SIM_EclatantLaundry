using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        
        public DashboardWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new System.Uri("DashboardPage.xaml", System.UriKind.Relative));
        }

        private void Nav_General_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                foreach (var child in MenuPanel.Children)
                {
                    if (child is Button btn) btn.Tag = "Inactive";
                }

                clickedButton.Tag = "Active";

                string pageName = "";
                switch (clickedButton.Content.ToString())
                {
                    case "Beranda": pageName = "DashboardPage.xaml"; break;
                    case "Data Barang": pageName = "DataBarangPage.xaml"; break;
                    case "Barang Masuk": pageName = "BarangMasukPage.xaml"; break;
                    case "Barang Keluar": pageName = "BarangKeluarPage.xaml"; break;
                    case "Laporan": pageName = "LaporanPage.xaml"; break;
                }

                if (!string.IsNullOrEmpty(pageName))
                {
                    MainFrame.Navigate(new System.Uri(pageName, System.UriKind.Relative));
                }
            }
        }

        private void Keluar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi Keluar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();

                this.Close();
            }
        }
    }
}
