using System;
using System.Collections.Generic;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO chứa thông tin đầy đủ của Hóa đơn phục vụ hiển thị thanh toán, in PDF và gửi email (DCKLTTQ-75).
    /// </summary>
    public class HoaDonDto
    {
        public int IdHoaDon { get; set; }
        public int IdDatPhong { get; set; }
        public string MaBooking { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public string SdtKhachHang { get; set; } = string.Empty;
        public string CccdKhachHang { get; set; } = string.Empty;
        public string EmailKhachHang { get; set; } = string.Empty;

        public DateTime NgayLap { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongGia { get; set; }

        public string? PhuongThucThanhtoan { get; set; }
        public string TrangThaiThanhtoan { get; set; } = string.Empty;

        // Thông tin thời gian lưu trú phục vụ chi tiết hóa đơn
        public DateOnly NgayNhanDuKien { get; set; }
        public DateOnly NgayTraDuKien { get; set; }
        public DateTime? NgayNhanThucTe { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        public int SoDemLuuTru { get; set; }

        // Danh sách phòng và dịch vụ chi tiết
        public List<ChiTietPhongDatDto> DanhSachPhong { get; set; } = new();
        public List<ChiTietDichVuDto> DanhSachDichVu { get; set; } = new();
    }
}
