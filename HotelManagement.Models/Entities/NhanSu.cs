using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class NhanSu
{
    public int IdNhansu { get; set; }

    public string Hoten { get; set; } = null!;

    public DateOnly Ngaysinh { get; set; }

    public string Cccd { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
