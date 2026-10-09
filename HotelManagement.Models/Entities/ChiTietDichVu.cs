using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class ChiTietDichVu
{
    public int Id { get; set; }

    public int IdDatphong { get; set; }

    public int IdDichvu { get; set; }

    public int Soluong { get; set; }

    public decimal Dongia { get; set; }

    public DateTime Thoigian { get; set; }

    public virtual DatPhong IdDatphongNavigation { get; set; } = null!;

    public virtual DichVu IdDichvuNavigation { get; set; } = null!;
}
