using FoodWebsite_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FoodWebsite_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DacSan> DacSans { get; set; }
        public DbSet<DacSanImages> DacSanImages { get; set; }
        public DbSet<CongThuc> CongThucs { get; set; }
        public DbSet<NguyenLieu> NguyenLieus { get; set; }
        public DbSet<NguyenLieuCongThuc> NguyenLieuCongThucs { get; set; }
        public DbSet<NguoiDungNguyenLieu> NguoiDungNguyenLieus { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<TinhThanh> TinhThanhs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for NguyenLieuCongThuc
            modelBuilder.Entity<NguyenLieuCongThuc>()
                .HasKey(n => new { n.CongThucId, n.NguyenLieuId });

            // Relationships
            modelBuilder.Entity<DacSan>()
                .HasMany(d => d.CongThucs)
                .WithOne(c => c.DacSan)
                .HasForeignKey(c => c.DacSanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DacSan>()
                .HasMany(d => d.DacSanImages)
                .WithOne(i => i.DacSan)
                .HasForeignKey(i => i.DacSanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.DacSan)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DacSanId);

            modelBuilder.Entity<NguoiDungNguyenLieu>()
                .HasOne(nd => nd.User)
                .WithMany(u => u.NguoiDung_NguyenLieus)
                .HasForeignKey(nd => nd.UserId);

            modelBuilder.Entity<NguoiDungNguyenLieu>()
                .HasOne(nd => nd.NguyenLieu)
                .WithMany(nl => nl.NguoiDungNguyenLieus)
                .HasForeignKey(nd => nd.NguyenLieuId);

            modelBuilder.Entity<NguyenLieuCongThuc>()
                .HasOne(nlc => nlc.CongThuc)
                .WithMany(ct => ct.NguyenLieu_CongThucs)
                .HasForeignKey(nlc => nlc.CongThucId);

            modelBuilder.Entity<NguyenLieuCongThuc>()
                .HasOne(nlc => nlc.NguyenLieu)
                .WithMany(nl => nl.NguyenLieuCongThucs)
                .HasForeignKey(nlc => nlc.NguyenLieuId);

            modelBuilder.Entity<TinhThanh>()
                .HasMany(t => t.DacSans)
                .WithOne(d => d.TinhThanh)
                .HasForeignKey(d => d.TinhThanhId);

            modelBuilder.Entity<NguoiDungNguyenLieu>()
                .Property(nd => nd.SoLuong)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NguyenLieuCongThuc>()
                .Property(nl => nl.SoLuong)
                .HasPrecision(18, 2);

            // Indexes for performance & search
            modelBuilder.Entity<DacSan>()
                .HasIndex(d => d.Ten);
            modelBuilder.Entity<DacSan>()
                .HasIndex(d => d.TenKhongDau);
            modelBuilder.Entity<DacSan>()
                .HasIndex(d => d.Slug);
            modelBuilder.Entity<DacSan>()
                .HasIndex(d => d.TinhThanhId);

            modelBuilder.Entity<NguyenLieu>()
                .HasIndex(n => n.Ten);
            modelBuilder.Entity<NguyenLieu>()
                .HasIndex(n => n.TenKhongDau);
            modelBuilder.Entity<NguyenLieu>()
                .HasIndex(n => n.Slug);

            modelBuilder.Entity<CongThuc>()
                .HasIndex(c => c.Ten);
            modelBuilder.Entity<CongThuc>()
                .HasIndex(c => c.TenKhongDau);
            modelBuilder.Entity<CongThuc>()
                .HasIndex(c => c.Slug);
            modelBuilder.Entity<CongThuc>()
                .HasIndex(c => c.DacSanId);

            modelBuilder.Entity<TinhThanh>()
                .HasIndex(t => t.Ten);
            modelBuilder.Entity<TinhThanh>()
                .HasIndex(t => t.TenKhongDau);
            modelBuilder.Entity<TinhThanh>()
                .HasIndex(t => t.Slug);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.FullName);
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.City);
        }
    }

}
