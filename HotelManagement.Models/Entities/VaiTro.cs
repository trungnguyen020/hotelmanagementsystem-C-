using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class VaiTro
{
    public int IdVaitro { get; set; }

    public string TenVaitro { get; set; } = null!;

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
