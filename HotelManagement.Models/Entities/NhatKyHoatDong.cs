using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class NhatKyHoatDong
{
    public int IdNhatky { get; set; }

    public int IdTaikhoan { get; set; }

    public string Hanhdong { get; set; } = null!;

    public DateTime Thoigian { get; set; }

    public string? Ghichu { get; set; }

    public virtual TaiKhoan IdTaikhoanNavigation { get; set; } = null!;
}
