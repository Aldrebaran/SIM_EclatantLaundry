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

namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for LaporanPage.xaml
    /// </summary>
    public partial class LaporanPage : Page
    {
        bool isMingguan = true;

        public LaporanPage()
        {
            InitializeComponent();

            int tahunRilis = 2026;
            int tahunSekarang = DateTime.Now.Year;

            for (int i = tahunSekarang; i >= tahunRilis; i--)
            {
                cbTahun.Items.Add(i);
            }

            if (cbTahun.Items.Count > 0)
            {
                cbTahun.SelectedIndex = 0;
            }
        }

        private void BtnMingguan_Click(object sender, RoutedEventArgs e)
        {
            isMingguan = true;

            btnMingguan.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            btnMingguan.Foreground = Brushes.White;

            btnBulanan.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
            btnBulanan.Foreground = Brushes.Black;

            panelMinggu.Visibility = Visibility.Visible;
        }

        private void BtnBulanan_Click(Object sender, RoutedEventArgs e)
        {
            isMingguan = false;

            btnBulanan.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            btnBulanan.Foreground = Brushes.White;

            btnMingguan.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
            btnMingguan.Foreground = Brushes.Black;

            panelMinggu.Visibility = Visibility.Collapsed;
        }

        private void BtnTampilkan_Click(object sender, RoutedEventArgs e)
        {
            if (cbTahun.SelectedItem == null || cbBulan.SelectedItem == null)
            {
                MessageBox.Show("Silakan pilih Tahun dan Bulan terlebih dahulu!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tahun = cbTahun.SelectedItem.ToString();

            string bulan = (cbBulan.SelectedIndex + 1).ToString("D2");

            string kueriSQL = "";

            if (isMingguan)
            {
                if (cbMinggu.SelectedItem == null)
                {
                    MessageBox.Show("Silakan pilih Minggu!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int mingguKe = cbMinggu.SelectedIndex;
                int tglMulai = (mingguKe * 7) + 1;
                int tglSelesai = tglMulai + 6;

                string hariSelesai = (mingguKe == 3) ? "31" : tglSelesai.ToString("D2");
                string hariMulai = tglMulai.ToString("D2");

                kueriSQL = $@"SELECT r.TANGGAL, b.NAMA_BARANG, r.JENIS_RIWAYAT, r.JUMLAH, b.SATUAN
                            FROM RIWAYAT r
                            JOIN BARANG b ON r.ID_BARANG = b.ID_BARANG
                            WHERE r.TANGGAL BETWEEN '{tahun}-{bulan}-{hariMulai} 00:00:00'
                            AND '{tahun}-{bulan}-{hariSelesai} 23:59:59'
                            ORDER BY r.tanggal DESC";
            }
            else
            {
                kueriSQL = $@"SELECT r.TANGGAL, b.NAMA_BARANG, r.JENIS_RIWAYAT, r.JUMLAH, b.SATUAN
                            FROM RIWAYAT r
                            JOIN BARANG b ON r.ID_BARANG = b.ID_BARANG
                            WHERE r.TANGGAL LIKE '{tahun}-{bulan}%'
                            ORDER BY r.tanggal DESC";
            }

            EksekusiDataLaporan(kueriSQL);

        }

        private void EksekusiDataLaporan(string kueriSQL)
        {
            var daftarLaporan = new List<LaporanModel>();
            int totalMasuk = 0;
            int totalKeluar = 0;

            try
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=eclatant_laundry.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = kueriSQL;

                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tglRaw = reader.GetString(0);
                            string nama = reader.GetString(1);
                            string jenis = reader.GetString(2);
                            int jml = reader.GetInt32(3);
                            string sat = reader.GetString(4);

                            DateTime tglFull = DateTime.Parse(tglRaw);

                            daftarLaporan.Add(new LaporanModel
                            {
                                Tanggal = tglFull.ToString("dd/MM/yyyy HH:mm"),
                                NamaBarang = nama,
                                Jenis = jenis,
                                Jumlah = jml,
                                Satuan = sat
                            });

                            if (jenis.Equals("Masuk", StringComparison.OrdinalIgnoreCase))
                                totalMasuk += jml;
                            else if (jenis.Equals("Keluar", StringComparison.OrdinalIgnoreCase))
                                totalKeluar += jml;
                        }
                    }
                }

                dgLaporan.ItemsSource = daftarLaporan;

                txtTotalMasuk.Text = totalMasuk.ToString();
                txtTotalKeluar.Text = totalKeluar.ToString();
                txtStokAkhir.Text = (totalMasuk - totalKeluar).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat memuat data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCetakPDF_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                StackPanel halaman = new StackPanel();
                halaman.Width = pd.PrintableAreaWidth;
                halaman.Margin = new Thickness(45);
                halaman.Background = Brushes.White;

                halaman.Children.Add(new TextBlock
                {
                    Text = "Laporan Riwayat Barang Inventaris",
                    FontSize = 22,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                halaman.Children.Add(new TextBlock
                {
                    Text = "Periode: " + cbBulan.Text + " " + cbTahun.Text,
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 30)
                });

                StackPanel ringkasan = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 30)
                };

                var styleLabel = new Func<string, TextBlock>((txt) => new TextBlock { Text = txt, FontWeight = FontWeights.Bold, FontSize = 12 });
                var styleValue = new Func<string, TextBlock>((txt) => new TextBlock { Text = txt + "    ", FontSize = 12 });

                ringkasan.Children.Add(styleLabel("TOTAL MASUK: "));
                ringkasan.Children.Add(styleValue(txtTotalMasuk.Text));

                ringkasan.Children.Add(styleLabel("TOTAL KELUAR: "));
                ringkasan.Children.Add(styleValue(txtTotalKeluar.Text));

                ringkasan.Children.Add(styleLabel("STOK AKHIR: "));
                ringkasan.Children.Add(styleValue(txtStokAkhir.Text));

                halaman.Children.Add(ringkasan);

                halaman.Children.Add(BuatBarisTabel("Tanggal", "Nama Barang", "Jenis", "Jumlah", "Satuan", true));

                var dataSource = dgLaporan.ItemsSource as System.Collections.IEnumerable;
                if (dataSource != null)
                {
                    foreach (var item in dataSource)
                    {
                        dynamic d = item;
                        halaman.Children.Add(BuatBarisTabel(
                            d.Tanggal.ToString(),
                            d.NamaBarang.ToString(),
                            d.Jenis.ToString(),
                            d.Jumlah.ToString(),
                            d.Satuan.ToString(),
                            false));
                    }
                }

                halaman.Measure(new Size(pd.PrintableAreaWidth, double.PositiveInfinity));
                halaman.Arrange(new Rect(new Point(0, 0), halaman.DesiredSize));

                pd.PrintVisual(halaman, "Laporan Inventaris");
            }
        }

        private Grid BuatBarisTabel(string t1, string t2, string t3, string t4, string t5, bool isHeader)
        {
            Grid row = new Grid();
            row.Background = isHeader ? new SolidColorBrush(Color.FromRgb(240, 240, 240)) : Brushes.Transparent;

            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3.5, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });

            string[] isi = { t1, t2, t3, t4, t5 };
            for (int i = 0; i < isi.Length; i++)
            {
                var tb = new TextBlock
                {
                    Text = isi[i],
                    Padding = new Thickness(8, 5, 8, 5),
                    FontSize = 10.5,
                    FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                    HorizontalAlignment = (i == 1) ? HorizontalAlignment.Center : HorizontalAlignment.Center
                };
                Grid.SetColumn(tb, i);
                row.Children.Add(tb);
            }

            Border garis = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 0, 0.5),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Grid.SetColumnSpan(garis, 5);
            row.Children.Add(garis);

            return row;
        }

        private UIElement BuatKotakSummary(string label, string value, int col)
        {
            Border frame = new Border
            {
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Background = Brushes.GhostWhite,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5)
            };

            StackPanel sp = new StackPanel();
            sp.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            sp.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            });

            frame.Child = sp;
            Grid.SetColumn(frame, col);
            return frame;
        }

        public class LaporanModel
        {
            public string? Tanggal { get; set; }
            public string? NamaBarang { get; set; }
            public string? Jenis { get; set; }
            public int Jumlah { get; set; }
            public string? Satuan { get; set; }
        }
    }
}
