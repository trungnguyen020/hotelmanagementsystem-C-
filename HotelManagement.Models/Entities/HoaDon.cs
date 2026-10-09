using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class HoaDon
{
    public int IdHoadon { get; set; }

    public int IdDatphong { get; set; }

    public DateTime Ngaylap { get; set; }

    public decimal Tienphong { get; set; }

    public decimal Tiendichvu { get; set; }

    public decimal Giamgia { get; set; }

    public decimal? Tonggia { get; set; }

    public string? PhuongthucThanhtoan { get; set; }

    public string TrangthaiThanhtoan { get; set; } = null!;

    public virtual DatPhong IdDatphongNavigation { get; set; } = null!;
}
