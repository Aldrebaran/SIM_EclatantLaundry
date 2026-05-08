using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EclatantLaundry.Models
{
    [Table("RIWAYAT")]
    public class Riwayat
    {
        [Key]
        [Column("ID_RIWAYAT")]
        public int ID_RIWAYAT { get; set; }

        [Column("ID_BARANG")]
        public int ID_BARANG { get; set; }

        [Column("ID_SUPERVISOR")]
        public int ID_SUPERVISOR { get; set; }

        [Column("ID_STOK")]
        public int ID_STOK { get; set; }

        [Column("TANGGAL")]
        public DateTime TANGGAL { get; set; }

        [Column("JUMLAH")]
        public double JUMLAH { get; set; }

        [Column("JENIS_RIWAYAT")]
        public string JENIS_RIWAYAT { get; set; } = string.Empty;

        [Column("EXPIRED_DATE")]
        public DateTime? EXPIRED_DATE { get; set; }

        [ForeignKey("ID_BARANG")]
        public virtual Barang? Barang { get; set; }
        [ForeignKey("ID_SUPERVISOR")]
        public virtual Supervisor? Supervisor { get; set; }
        [ForeignKey("ID_STOK")]
        public virtual Stok? Stok { get; set; }
    }
}
