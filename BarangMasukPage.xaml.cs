using EclatantLaundry.Models;
using EclatantLaundry.Services;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for BarangMasukPage.xaml
    /// </summary>
    public partial class BarangMasukPage : Page
    {
        public BarangMasukPage()
        {
            InitializeComponent();
        }

        private void cbKategori_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {
           if (cbKategori.SelectedItem is ComboBoxItem selectedBarang)
            {
                string id = selectedBarang.Tag.ToString()!;

                switch (id)
                {
                    case "1":
                    case "2":
                        txtSatuan.Text = "Liter";
                        break;
                    case "3":
                        txtSatuan.Text = "Roll";
                        break;
                    case "4":
                        txtSatuan.Text = "Lusin";
                        break;
                    default:
                        txtSatuan.Text = "";
                        break;
                }
            }
           
        }

        private void btnSimpan_Click(object sender, RoutedEventArgs e)
        {

            if (UserSession.ID_SUPERVISOR == 0)
            {
                MessageBox.Show("Sesi login tidak ditemukan. Silakan login kembali.");
                return;
            }

            if (cbKategori.SelectedValue == null || string.IsNullOrEmpty(txtJumlah.Text))
            {
                MessageBox.Show("Mohon pilih kategori barang dan isi jumlahnya.");
                return;
            }



            try
            {
                using (var db = new AppDbContext())
                {
                    var selectedItem = (ComboBoxItem)cbKategori.SelectedItem;

                    if (selectedItem == null || selectedItem.Tag == null)
                    {
                        MessageBox.Show("Silahkan pilih kategori barang terlebih dahulu.");
                        return;
                    }

                    int idBarang = int.Parse(selectedItem.Tag.ToString()!);
                    string namaBarang = selectedItem.Content.ToString()!;
                    DateTime? tglExpired = dtpExpired.SelectedDate;

                    if (!double.TryParse(txtJumlah.Text, out double jumlahInput) || jumlahInput <= 0)
                    {
                        MessageBox.Show("Input jumlah harus angka positif dan lebih dari 0!");
                        return;
                    }

                    var cekBarang = db.Barang.Find(idBarang);
                    if (cekBarang == null)
                    {
                        var dataMasterBaru = new Barang
                        {
                            ID_BARANG = idBarang,
                            NAMA_BARANG = namaBarang,
                            SATUAN = txtSatuan.Text
                        };
                        db.Barang.Add(dataMasterBaru);
                        db.SaveChanges();
                    }

                    var stokBaru = new Stok
                    {
                        ID_BARANG = idBarang,
                        JUMLAH_TERSEDIA = jumlahInput,
                        STOK_MINIMUM = 0,
                        TGL_KEDALUWARSA = tglExpired
                    };

                    db.Stok.Add(stokBaru);
                    db.SaveChanges();

                    DateTime tanggalDipilih = dtpTanggal.SelectedDate ?? DateTime.Today;
                    DateTime tanggalLengkap = tanggalDipilih.Date.Add(DateTime.Now.TimeOfDay);

                    var baru = new Riwayat
                    {
                        ID_BARANG = idBarang,
                        ID_STOK = stokBaru.ID_STOK,
                        JENIS_RIWAYAT = "Masuk",
                        JUMLAH = jumlahInput,
                        TANGGAL = tanggalLengkap,
                        EXPIRED_DATE= dtpExpired.SelectedDate,
                        ID_SUPERVISOR = UserSession.ID_SUPERVISOR
                    };

                    db.Riwayat.Add(baru);
                    db.SaveChanges();

                    MessageBox.Show($"Berhasil menyimpan {jumlahInput} {namaBarang}!");

                    txtJumlah.Clear();
                    cbKategori.SelectedIndex = -1;
                    txtSatuan.Clear();
                    dtpExpired.SelectedDate = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menyimpan: " + ex.Message);
            }
        }
    }
}
