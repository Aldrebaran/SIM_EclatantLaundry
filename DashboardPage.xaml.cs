using Microsoft.Data.Sqlite;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {

        private string connectionString = "Data Source=eclatant_laundry.db";

        public DashboardPage()
        {
            InitializeComponent();
            LoadProfileData();
            RefreshDashboardSummary();
            LoadWarnings();
            
        }

        private void LoadProfileData()
        {
            txtNamaProfil.Text = UserSession.NAMA_SUPERVISOR;

            txtNamaSupervisor.Text = UserSession.NAMA_SUPERVISOR;
        }

        private void RefreshDashboardSummary()
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    var cmdTotal = new SqliteCommand("SELECT SUM(JUMLAH_TERSEDIA) FROM STOK WHERE JUMLAH_TERSEDIA > 0", connection);
                    txtTotalBatch.Text = cmdTotal.ExecuteScalar()?.ToString() ?? "0";

                    string queryExpired = @"SELECT COUNT(*) FROM STOK WHERE TGL_KEDALUWARSA <= DATE('now', '+7 days') AND TGL_KEDALUWARSA >= DATE('now') AND JUMLAH_TERSEDIA > 0";
                    var cmdExpired = new SqliteCommand(queryExpired, connection);
                    txtAkanExpired.Text = cmdExpired.ExecuteScalar()?.ToString() ?? "0";

                    string queryMenipis = @"SELECT COUNT(*) FROM STOK WHERE JUMLAH_TERSEDIA <= 5 AND JUMLAH_TERSEDIA > 0";
                    var cmdMenipis = new SqliteCommand(queryMenipis, connection);
                    txtStokMenipis.Text = cmdMenipis.ExecuteScalar()?.ToString() ?? "0";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Gagal memuat ringkasan stok: " + e.Message);
            }
        }

        private void LoadWarnings()
        {
            var daftarWarning = new List< WarningItem > ();

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string queryExpired =
                        @"SELECT 
                            b.NAMA_BARANG,
                            s.TGL_KEDALUWARSA,
                            R.TANGGAL AS TANGGAL
                        FROM STOK s
                        INNER JOIN BARANG b ON s.ID_BARANG = b.ID_BARANG
                        INNER JOIN RIWAYAT r ON r.ID_STOK = S.ID_STOK
                        WHERE R.JENIS_RIWAYAT = 'Masuk'
                            AND S.TGL_KEDALUWARSA <= date('now', '+7 days')
                            AND S.TGL_KEDALUWARSA >= date('now')
                            AND S.JUMLAH_TERSEDIA > 0";

                    var cmdExpired = new SqliteCommand(queryExpired, connection);

                    using (var r = cmdExpired.ExecuteReader())
                    {
                        while (r.Read())
                        {

                            string rawExp = r["TGL_KEDALUWARSA"].ToString();
                            string rawMasuk = r["TANGGAL"].ToString();

                            string tglExp = rawExp.Contains(" ") ? rawExp.Split(' ')[0] : rawExp;
                            string tglMasuk = rawMasuk.Contains(" ") ? rawMasuk.Split(' ')[0] : rawMasuk;

                            daftarWarning.Add(new WarningItem
                            {
                                NamaBarang = $"{r["NAMA_BARANG"]} (Masuk: {tglMasuk})",
                                PesanWarning = $"Kedaluwarsa: {tglExp}",
                                WarnaStatus = "#E53E3E"
                            });
                        }
                    }

                    string queryTipis =
                        @"SELECT
                            b.NAMA_BARANG,
                            s.JUMLAH_TERSEDIA,
                            s.STOK_MINIMUM,
                            r.TANGGAL AS TANGGAL
                            FROM STOK s
                            INNER JOIN BARANG b ON s.ID_BARANG = b.ID_BARANG
                            INNER JOIN RIWAYAT r ON r.ID_STOK = s.ID_STOK
                            WHERE R.JENIS_RIWAYAT = 'Masuk'
                                AND S.JUMLAH_TERSEDIA > 0";

                    var cmdTipis = new SqliteCommand(queryTipis, connection);

                    using (var reader = cmdTipis.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int stokSkrg = Convert.ToInt32(reader["JUMLAH_TERSEDIA"]);
                            int min = Convert.ToInt32(reader["STOK_MINIMUM"]);

                            const int batasPeringatan = 5;

                            if (stokSkrg <= batasPeringatan)
                            {
                                string tglMasuk = reader["TANGGAL"].ToString().Split(' ')[0];
                                daftarWarning.Add(new WarningItem
                                {
                                    NamaBarang = $"{reader["NAMA_BARANG"]} (Masuk: {tglMasuk})",
                                    PesanWarning = $"Stok Menipis: Sisa {stokSkrg} unit",
                                    WarnaStatus = "#DD6B20"
                                });
                            }
                        }
                    }
                }

                listWarning.ItemsSource = null;
                listWarning.ItemsSource = daftarWarning;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat daftar peringatan: " + ex.Message);
            }
        }

    }

    public class WarningItem
    {
        public string? NamaBarang { get; set; }
        public string? PesanWarning { get; set; }
        public string? WarnaStatus { get; set; }
    }
}
