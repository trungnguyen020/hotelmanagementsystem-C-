using System;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO hiển thị tóm tắt lịch sử đặt phòng của khách hàng (DCKLTTQ-109).
    /// </summary>
    public class DatPhongLichSuDto
    {
        public int IdDatPhong { get; set; }
        public string MaBooking { get; set; } = string.Empty;
        public DateTime? NgayDat { get; set; }
        public DateOnly NgayNhan { get; set; }
        public DateOnly NgayTra { get; set; }
        public int SoDem => Math.Max(1, NgayTra.DayNumber - NgayNhan.DayNumber);
        public int SoNguoi { get; set; }
        public string DanhSachPhong { get; set; } = string.Empty;
        public string TrangThaiBooking { get; set; } = string.Empty;
        public bool CoHoaDon { get; set; }
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";
        public decimal TongTien { get; set; }
    }
}

