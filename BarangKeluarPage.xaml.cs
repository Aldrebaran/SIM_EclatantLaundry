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
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EclatantLaundry.Models;

namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for BarangKeluarPage.xaml
    /// </summary>
    public partial class BarangKeluarPage : Page
    {
        public BarangKeluarPage()
        {
            InitializeComponent();
            dtpTanggalKeluar.SelectedDate = DateTime.Now;
        }

        private void cbKategori_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if(cbKategori.SelectedItem is ComboBoxItem selectedItem)
            {
                string id = selectedItem.Tag.ToString()!;
                txtSatuan.Text = id switch
                {
                    "1" or "2" => "Liter",
                    "3" => "Roll",
                    "4" => "Lusin",
                    _ => ""
                };

                using (var db = new AppDbContext())
                {
                    int idBarang = int.Parse(id);
                    cbBatchStok.ItemsSource = db.Stok
                        .Include(s => s.Barang)
                        .Where(s => s.ID_BARANG == idBarang && s.JUMLAH_TERSEDIA > 0)
                        .OrderBy(s => s.ID_STOK)
                        .ToList();
                }
            }
        }

        private void btnSimpan_Click(object sender, RoutedEventArgs e)
        {
            if (cbBatchStok.SelectedItem is not Stok batchTerpilih) return;

            if (!double.TryParse(txtJumlahKeluar.Text, out double jmlKeluar) || jmlKeluar <= 0)
            {
                MessageBox.Show("Input jumlah harus angka positif dan lebih dari 0!");
                return;
            }

            using (var db = new AppDbContext())
            {
                var stok = db.Stok.Find(batchTerpilih.ID_STOK);

                if (stok == null || stok.JUMLAH_TERSEDIA < jmlKeluar)
                {
                    MessageBox.Show("Stok tidak mencukupi.");
                    return;
                }

                stok.JUMLAH_TERSEDIA -= jmlKeluar;

                DateTime tglKeluarDipilih = dtpTanggalKeluar.SelectedDate ?? DateTime.Today;
                DateTime tglKeluarLengkap = tglKeluarDipilih.Date.Add(DateTime.Now.TimeOfDay);

                db.Riwayat.Add(new Riwayat
                {
                    ID_BARANG = stok.ID_BARANG,
                    ID_STOK = stok.ID_STOK,
                    JENIS_RIWAYAT = "Keluar",
                    JUMLAH = jmlKeluar,
                    TANGGAL = tglKeluarLengkap,
                    ID_SUPERVISOR = UserSession.ID_SUPERVISOR
                });

                db.SaveChanges();
                MessageBox.Show($"Berhasil memakai {jmlKeluar} {txtSatuan.Text} {batchTerpilih.Barang?.NAMA_BARANG}!");

                cbKategori_SelectionChanged(null, null);
                txtJumlahKeluar.Clear();
            }
        }

    }
}
