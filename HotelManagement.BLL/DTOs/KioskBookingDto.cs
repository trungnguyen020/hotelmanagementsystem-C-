using System;
using System.Collections.Generic;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// Thông tin phòng thu gọn hiển thị tại Kiosk sảnh (DCKLTTQ-85).
    /// </summary>
    public class KioskPhongDto
    {
        public string SoPhong { get; set; } = string.Empty;
        public string TenLoaiPhong { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO chứa thông tin tra cứu đặt phòng tại Kiosk công cộng (DCKLTTQ-85).
    /// TUYỆT ĐỐI KHÔNG chứa CCCD, mật khẩu hay dữ liệu nhạy cảm của khách hàng.
    /// </summary>
    public class KioskBookingDto
    {
        public string MaBooking { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public string TrangThaiBooking { get; set; } = string.Empty;
        public DateOnly NgayNhanDuKien { get; set; }
        public DateOnly NgayTraDuKien { get; set; }
        public int SoDem { get; set; }
        public int SoNguoi { get; set; }
        public List<KioskPhongDto> DanhSachPhong { get; set; } = new();
        public List<string> DanhSachDichVu { get; set; } = new();
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";
        public decimal TongTien { get; set; }
    }
}
