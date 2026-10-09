using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class TaiKhoan
{
    public int IdTaikhoan { get; set; }

    public string TenDangnhap { get; set; } = null!;

    public string MatkhauHash { get; set; } = null!;

    public int IdNhansu { get; set; }

    public int IdVaitro { get; set; }

    public string Trangthai { get; set; } = null!;

    public int SolanSaimatkhau { get; set; }

    public virtual NhanSu IdNhansuNavigation { get; set; } = null!;

    public virtual VaiTro IdVaitroNavigation { get; set; } = null!;

    public virtual ICollection<NhatKyHoatDong> NhatKyHoatDongs { get; set; } = new List<NhatKyHoatDong>();
}
