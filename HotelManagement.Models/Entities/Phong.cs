using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class Phong
{
    public int IdPhong { get; set; }

    public string Sophong { get; set; } = null!;

    public int IdLoaiphong { get; set; }

    public string Trangthai { get; set; } = null!;

    public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();

    public virtual LoaiPhong IdLoaiphongNavigation { get; set; } = null!;
}
