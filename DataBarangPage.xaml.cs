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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EclatantLaundry.Models;
using Microsoft.EntityFrameworkCore;

namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for DataBarangPage.xaml
    /// </summary>
    public partial class DataBarangPage : Page
    {
        private int currentPage = 1;
        private int itemsPerPage = 7;

        private List<EclatantLaundry.Models.Stok> allData = new List<EclatantLaundry.Models.Stok>();

        public DataBarangPage()
        {
            InitializeComponent();

            RefreshData();

        }

        private void RefreshData()
        {
            using (var db = new AppDbContext())
            {
                allData = db.Stok
                    .Include(b => b.Barang)
                    .Where(b => b.JUMLAH_TERSEDIA > 0)
                    .OrderBy(b => b.TGL_KEDALUWARSA)
                    .ToList();
            }

            currentPage = 1;
            LoadData();
        }

        private void LoadData()
        {
            var pagedData = allData.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            dgBarang.ItemsSource = pagedData;
            lblPageNumber.Text = $"Halaman {currentPage}";
            btnPrev.IsEnabled = currentPage > 1;
            btnNext.IsEnabled = (currentPage * itemsPerPage) < allData.Count;

            CekData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (allData == null) return;

            string keyword = txtSearch.Text.ToLower().Trim();

            if (string .IsNullOrWhiteSpace(keyword))
            {
                currentPage = 1;
                LoadData();
            }
            else
            {
                var filteredList = allData.Where
                    (b =>
                        (b.Barang != null && b.Barang.NAMA_BARANG.ToLower().Contains(keyword)) ||
                        (b.TGL_KEDALUWARSA.HasValue && b.TGL_KEDALUWARSA.Value.ToString("dd/MM/yyyy").Contains(keyword))
                    ).ToList();

                dgBarang.ItemsSource = filteredList;
            }
        }

        private void btnHapus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var stokTerpilih = button?.CommandParameter as Stok;

            if (stokTerpilih == null) return;

            var result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus Batch ini?\n\n" +
                "Tindakan ini akan menghapus:\n" +
                "- Data Stok (Batch) ini\n" +
                "- Riwayat transaksi untuk batch ini saja\n\n" +
                "CATATAN: Kategori barang di ComboBox tidak akan hilang.",
                "Konfirmasi Hapus Batch",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var stokDiDb = db.Stok.Find(stokTerpilih.ID_STOK);

                        if (stokDiDb != null)
                        {
                            db.Stok.Remove(stokDiDb);

                            db.SaveChanges();

                            MessageBox.Show("Data berhasil dihapus permanen.");

                            RefreshData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menghapus data: " + ex.Message);
                }
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadData();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if ((currentPage * itemsPerPage) < allData.Count)
            {
                currentPage++;
                LoadData();
            }
        }

        private void CekData()
        {
            if (allData.Count == 0)
            {
                lblEmpty.Visibility = Visibility.Visible;
                dgBarang.Visibility = Visibility.Visible;
            }
            else
            {
                lblEmpty.Visibility = Visibility.Collapsed;
                dgBarang.Visibility = Visibility.Visible;
            }
        }

    }
}
