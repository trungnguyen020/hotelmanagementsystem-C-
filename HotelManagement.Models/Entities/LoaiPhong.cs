using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class LoaiPhong
{
    public int IdLoaiphong { get; set; }

    public string Ten { get; set; } = null!;

    public decimal Mucgia { get; set; }

    public int Succhua { get; set; }

    public string? Mota { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
