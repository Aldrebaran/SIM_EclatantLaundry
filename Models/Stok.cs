using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EclatantLaundry.Models
{
    [Table("STOK")]
    public class Stok
    {
        [Key]
        [Column("ID_STOK")]
        public int ID_STOK { get; set; }

        [Column("ID_BARANG")]
        public int ID_BARANG { get; set; }

        [Column("JUMLAH_TERSEDIA")]
        public double JUMLAH_TERSEDIA { get; set; }

        [Column("STOK_MINIMUM")]
        public double STOK_MINIMUM { get; set; }

        [Column("TGL_KEDALUWARSA")]
        public DateTime? TGL_KEDALUWARSA { get; set; }

        [ForeignKey("ID_BARANG")]
        public virtual Barang? Barang { get; set; }

        public virtual ICollection<Riwayat> ListRiwayat { get; set; } = new List<Riwayat>();

        public string InfoBatch
        {
            get
            {
                string exp = TGL_KEDALUWARSA.HasValue
                    ? TGL_KEDALUWARSA.Value.ToString("MM/yyyy")
                    : "N/A";

                string nama = Barang?.NAMA_BARANG ?? "Barang";

                return $"{Barang?.NAMA_BARANG} - Batch: {ID_STOK} (Sisah: {JUMLAH_TERSEDIA} - Exp: {exp})";
            }
        }
    }
}
