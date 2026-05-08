using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EclatantLaundry.Models
{
    [Table ("SUPERVISOR")]
    public class Supervisor
    {
        [Key]
        [Column ("ID_SUPERVISOR")]
        public int ID_SUPERVISOR { get; set; }

        [Column("NAMA_SUPERVISOR")]
        public string NAMA_SUPERVISOR { get; set; } = string.Empty;

        [Column("USERNAME")]
        public string USERNAME { get; set; } = string.Empty;

        [Column("PASSWORD")]
        public string PASSWORD { get; set; } = string.Empty;

        public virtual ICollection<Riwayat> Riwayat { get; set; } = new List<Riwayat>();
    }
}
