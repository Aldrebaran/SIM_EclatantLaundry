using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EclatantLaundry.Models
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Supervisor> Supervisor { get; set; }
        public DbSet<Barang> Barang { get; set; }  
        public DbSet<Riwayat> Riwayat { get; set; }
        public DbSet<Stok> Stok { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=eclatant_laundry.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stok>()
                .HasKey(s => s.ID_STOK);

            modelBuilder.Entity<Stok>()
                .HasOne(b => b.Barang)
                .WithMany(s => s.Stok)
                .HasForeignKey(s => s.ID_BARANG)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Riwayat>()
                .HasOne(r => r.Barang)
                .WithMany(b => b.Riwayat)
                .HasForeignKey(r => r.ID_BARANG)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Riwayat>()
                .HasOne(r => r.Stok)
                .WithMany(s => s.ListRiwayat)
                .HasForeignKey(r => r.ID_STOK)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Barang>().HasData
                (
                    new Barang { ID_BARANG = 1, NAMA_BARANG = "Deterjen", SATUAN = "Liter" },
                    new Barang { ID_BARANG = 2, NAMA_BARANG = "Pewangi", SATUAN = "Liter" },
                    new Barang { ID_BARANG = 3, NAMA_BARANG = "Plastik", SATUAN = "Roll" },
                    new Barang { ID_BARANG = 4, NAMA_BARANG = "Hanger", SATUAN = "Lusin" }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
