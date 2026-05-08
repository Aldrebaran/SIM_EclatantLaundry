using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EclatantLaundry.Models
{
    [Table("BARANG")]
    public class Barang
    {
        [Key]
        [Column("ID_BARANG")]
        public int ID_BARANG { get; set; }

        [Column("NAMA_BARANG")]
        public string NAMA_BARANG { get; set; } = string.Empty;

        [Column("SATUAN")]
        public string SATUAN { get; set; } = string.Empty;

        public virtual ICollection<Stok> Stok { get; set; } = new List<Stok>();

        public virtual ICollection<Riwayat> Riwayat { get; set; } = new List<Riwayat>();

        [NotMapped]
        public string TanggalKedaluwarsaTampil
        { 
            get
            {
                var dataTerakhir = Riwayat?
                .Where(r => r.JENIS_RIWAYAT == "Masuk" && r.EXPIRED_DATE != null)
                .OrderByDescending(r => r.TANGGAL)
                .FirstOrDefault();

                return dataTerakhir?.EXPIRED_DATE?.ToString("dd/MM/yyyy") ?? "-";
            }
        }      
    }
}