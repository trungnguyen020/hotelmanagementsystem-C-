using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class DichVu
{
    public int IdDichvu { get; set; }

    public string TenDichvu { get; set; } = null!;

    public decimal Mucgia { get; set; }

    public virtual ICollection<ChiTietDichVu> ChiTietDichVus { get; set; } = new List<ChiTietDichVu>();
}
