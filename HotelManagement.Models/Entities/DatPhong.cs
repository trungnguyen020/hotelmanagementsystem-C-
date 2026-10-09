using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class DatPhong
{
    public int IdDatphong { get; set; }

    public string Mabooking { get; set; } = null!;

    public int IdKhachhang { get; set; }

    public int IdNhansu { get; set; }

    public DateTime Ngaydat { get; set; }

    public DateOnly NgaynhanDukien { get; set; }

    public DateOnly NgaytraDukien { get; set; }

    public DateTime? NgaynhanThucte { get; set; }

    public DateTime? NgaytraThucte { get; set; }

    public int Songuoi { get; set; }

    public string Trangthai { get; set; } = null!;

    public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();

    public virtual ICollection<ChiTietDichVu> ChiTietDichVus { get; set; } = new List<ChiTietDichVu>();

    public virtual HoaDon? HoaDon { get; set; }

    public virtual KhachHang IdKhachhangNavigation { get; set; } = null!;

    public virtual NhanSu IdNhansuNavigation { get; set; } = null!;
}
