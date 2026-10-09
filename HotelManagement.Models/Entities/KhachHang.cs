using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class KhachHang
{
    public int IdKhachhang { get; set; }

    public string Ten { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Cccd { get; set; } = null!;

    public string? Gioitinh { get; set; }

    public DateOnly? Ngaysinh { get; set; }

    public string Email { get; set; } = null!;

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
