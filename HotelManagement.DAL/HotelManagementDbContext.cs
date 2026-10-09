using System;
using System.Collections.Generic;
using HotelManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.DAL;

public partial class HotelManagementDbContext : DbContext
{
    public HotelManagementDbContext(DbContextOptions<HotelManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; }

    public virtual DbSet<ChiTietDichVu> ChiTietDichVus { get; set; }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<NhanSu> NhanSus { get; set; }

    public virtual DbSet<NhatKyHoatDong> NhatKyHoatDongs { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDatPhong>(entity =>
        {
            entity.ToTable("ChiTietDatPhong");

            entity.HasIndex(e => new { e.IdDatphong, e.IdPhong }, "UQ_CTDP_DatPhong_Phong").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dongia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("dongia");
            entity.Property(e => e.IdDatphong).HasColumnName("id_datphong");
            entity.Property(e => e.IdPhong).HasColumnName("id_phong");

            entity.HasOne(d => d.IdDatphongNavigation).WithMany(p => p.ChiTietDatPhongs)
                .HasForeignKey(d => d.IdDatphong)
                .HasConstraintName("FK_CTDP_DatPhong");

            entity.HasOne(d => d.IdPhongNavigation).WithMany(p => p.ChiTietDatPhongs)
                .HasForeignKey(d => d.IdPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDP_Phong");
        });

        modelBuilder.Entity<ChiTietDichVu>(entity =>
        {
            entity.ToTable("ChiTietDichVu");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dongia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("dongia");
            entity.Property(e => e.IdDatphong).HasColumnName("id_datphong");
            entity.Property(e => e.IdDichvu).HasColumnName("id_dichvu");
            entity.Property(e => e.Soluong).HasColumnName("soluong");
            entity.Property(e => e.Thoigian)
                .HasDefaultValueSql("(sysdatetime())", "DF_CTDV_ThoiGian")
                .HasColumnName("thoigian");

            entity.HasOne(d => d.IdDatphongNavigation).WithMany(p => p.ChiTietDichVus)
                .HasForeignKey(d => d.IdDatphong)
                .HasConstraintName("FK_CTDV_DatPhong");

            entity.HasOne(d => d.IdDichvuNavigation).WithMany(p => p.ChiTietDichVus)
                .HasForeignKey(d => d.IdDichvu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDV_DichVu");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.IdDatphong);

            entity.ToTable("DatPhong");

            entity.HasIndex(e => new { e.NgaynhanDukien, e.NgaytraDukien }, "IX_DatPhong_KhoangNgay");

            entity.HasIndex(e => e.Mabooking, "UQ_DatPhong_MaBooking").IsUnique();

            entity.Property(e => e.IdDatphong).HasColumnName("id_datphong");
            entity.Property(e => e.IdKhachhang).HasColumnName("id_khachhang");
            entity.Property(e => e.IdNhansu).HasColumnName("id_nhansu");
            entity.Property(e => e.Mabooking)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("mabooking");
            entity.Property(e => e.Ngaydat)
                .HasDefaultValueSql("(sysdatetime())", "DF_DatPhong_NgayDat")
                .HasColumnName("ngaydat");
            entity.Property(e => e.NgaynhanDukien).HasColumnName("ngaynhan_dukien");
            entity.Property(e => e.NgaynhanThucte).HasColumnName("ngaynhan_thucte");
            entity.Property(e => e.NgaytraDukien).HasColumnName("ngaytra_dukien");
            entity.Property(e => e.NgaytraThucte).HasColumnName("ngaytra_thucte");
            entity.Property(e => e.Songuoi).HasColumnName("songuoi");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(20)
                .HasDefaultValue("Đã xác nhận", "DF_DatPhong_TrangThai")
                .HasColumnName("trangthai");

            entity.HasOne(d => d.IdKhachhangNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.IdKhachhang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatPhong_KhachHang");

            entity.HasOne(d => d.IdNhansuNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.IdNhansu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatPhong_NhanSu");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.IdDichvu);

            entity.ToTable("DichVu");

            entity.Property(e => e.IdDichvu).HasColumnName("id_dichvu");
            entity.Property(e => e.Mucgia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("mucgia");
            entity.Property(e => e.TenDichvu)
                .HasMaxLength(100)
                .HasColumnName("ten_dichvu");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.IdHoadon);

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.IdDatphong, "UQ_HoaDon_DatPhong").IsUnique();

            entity.Property(e => e.IdHoadon).HasColumnName("id_hoadon");
            entity.Property(e => e.Giamgia)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("giamgia");
            entity.Property(e => e.IdDatphong).HasColumnName("id_datphong");
            entity.Property(e => e.Ngaylap)
                .HasDefaultValueSql("(sysdatetime())", "DF_HoaDon_NgayLap")
                .HasColumnName("ngaylap");
            entity.Property(e => e.PhuongthucThanhtoan)
                .HasMaxLength(20)
                .HasColumnName("phuongthuc_thanhtoan");
            entity.Property(e => e.Tiendichvu)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("tiendichvu");
            entity.Property(e => e.Tienphong)
                .HasColumnType("decimal(14, 2)")
                .HasColumnName("tienphong");
            entity.Property(e => e.Tonggia)
                .HasComputedColumnSql("(([tienphong]+[tiendichvu])-[giamgia])", true)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("tonggia");
            entity.Property(e => e.TrangthaiThanhtoan)
                .HasMaxLength(20)
                .HasDefaultValue("Chưa thanh toán", "DF_HoaDon_TrangThai")
                .HasColumnName("trangthai_thanhtoan");

            entity.HasOne(d => d.IdDatphongNavigation).WithOne(p => p.HoaDon)
                .HasForeignKey<HoaDon>(d => d.IdDatphong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_DatPhong");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.IdKhachhang);

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Cccd, "UQ_KhachHang_CCCD").IsUnique();

            entity.Property(e => e.IdKhachhang).HasColumnName("id_khachhang");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("cccd");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(10)
                .HasColumnName("gioitinh");
            entity.Property(e => e.Ngaysinh).HasColumnName("ngaysinh");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.IdLoaiphong);

            entity.ToTable("LoaiPhong");

            entity.Property(e => e.IdLoaiphong).HasColumnName("id_loaiphong");
            entity.Property(e => e.Mota)
                .HasMaxLength(500)
                .HasColumnName("mota");
            entity.Property(e => e.Mucgia)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("mucgia");
            entity.Property(e => e.Succhua).HasColumnName("succhua");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<NhanSu>(entity =>
        {
            entity.HasKey(e => e.IdNhansu);

            entity.ToTable("NhanSu");

            entity.HasIndex(e => e.Cccd, "UQ_NhanSu_CCCD").IsUnique();

            entity.Property(e => e.IdNhansu).HasColumnName("id_nhansu");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("cccd");
            entity.Property(e => e.Hoten)
                .HasMaxLength(100)
                .HasColumnName("hoten");
            entity.Property(e => e.Ngaysinh).HasColumnName("ngaysinh");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
        });

        modelBuilder.Entity<NhatKyHoatDong>(entity =>
        {
            entity.HasKey(e => e.IdNhatky);

            entity.ToTable("NhatKyHoatDong");

            entity.HasIndex(e => new { e.IdTaikhoan, e.Thoigian }, "IX_NhatKyHoatDong_TaiKhoan");

            entity.Property(e => e.IdNhatky).HasColumnName("id_nhatky");
            entity.Property(e => e.Ghichu)
                .HasMaxLength(255)
                .HasColumnName("ghichu");
            entity.Property(e => e.Hanhdong)
                .HasMaxLength(50)
                .HasColumnName("hanhdong");
            entity.Property(e => e.IdTaikhoan).HasColumnName("id_taikhoan");
            entity.Property(e => e.Thoigian)
                .HasDefaultValueSql("(sysdatetime())", "DF_NhatKyHoatDong_ThoiGian")
                .HasColumnName("thoigian");

            entity.HasOne(d => d.IdTaikhoanNavigation).WithMany(p => p.NhatKyHoatDongs)
                .HasForeignKey(d => d.IdTaikhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhatKyHoatDong_TaiKhoan");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.IdPhong);

            entity.ToTable("Phong");

            entity.HasIndex(e => e.Sophong, "UQ_Phong_SoPhong").IsUnique();

            entity.Property(e => e.IdPhong).HasColumnName("id_phong");
            entity.Property(e => e.IdLoaiphong).HasColumnName("id_loaiphong");
            entity.Property(e => e.Sophong)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("sophong");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(20)
                .HasDefaultValue("Trống", "DF_Phong_TrangThai")
                .HasColumnName("trangthai");

            entity.HasOne(d => d.IdLoaiphongNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.IdLoaiphong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Phong_LoaiPhong");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.IdTaikhoan);

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.IdNhansu, "UQ_TaiKhoan_NhanSu").IsUnique();

            entity.HasIndex(e => e.TenDangnhap, "UQ_TaiKhoan_TenDangNhap").IsUnique();

            entity.Property(e => e.IdTaikhoan).HasColumnName("id_taikhoan");
            entity.Property(e => e.IdNhansu).HasColumnName("id_nhansu");
            entity.Property(e => e.IdVaitro).HasColumnName("id_vaitro");
            entity.Property(e => e.MatkhauHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("matkhau_hash");
            entity.Property(e => e.SolanSaimatkhau).HasColumnName("solan_saimatkhau");
            entity.Property(e => e.TenDangnhap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ten_dangnhap");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động", "DF_TaiKhoan_TrangThai")
                .HasColumnName("trangthai");

            entity.HasOne(d => d.IdNhansuNavigation).WithOne(p => p.TaiKhoan)
                .HasForeignKey<TaiKhoan>(d => d.IdNhansu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_NhanSu");

            entity.HasOne(d => d.IdVaitroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.IdVaitro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_VaiTro");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.IdVaitro);

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaitro, "UQ_VaiTro_Ten").IsUnique();

            entity.Property(e => e.IdVaitro).HasColumnName("id_vaitro");
            entity.Property(e => e.TenVaitro)
                .HasMaxLength(50)
                .HasColumnName("ten_vaitro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
