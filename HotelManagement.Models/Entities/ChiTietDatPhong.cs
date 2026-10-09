using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class ChiTietDatPhong
{
    public int Id { get; set; }

    public int IdDatphong { get; set; }

    public int IdPhong { get; set; }

    public decimal Dongia { get; set; }

    public virtual DatPhong IdDatphongNavigation { get; set; } = null!;

    public virtual Phong IdPhongNavigation { get; set; } = null!;
}
