using System;
using System.Collections.Generic;

namespace HotelManagement.Models.Entities;

public partial class LichSuGuiThongBao
{
    public int IdLichsu { get; set; }

    public int? IdDatphong { get; set; }

    public int? IdHoadon { get; set; }

    public string LoaiThongbao { get; set; } = null!;

    public string NguoiNhan { get; set; } = null!;

    public string? TieuDe { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? LyDoLoi { get; set; }

    public DateTime ThoiGianGui { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual DatPhong? IdDatphongNavigation { get; set; }

    public virtual HoaDon? IdHoadonNavigation { get; set; }
}
