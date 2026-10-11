using System;
using System.Collections.Generic;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO chứa thông tin đầy đủ của một lượt đặt phòng phục vụ màn hình Check-in / Tra cứu (DCKLTTQ-71).
    /// </summary>
    public class DatPhongChiTietDto
    {
        public int IdDatPhong { get; set; }
        public string MaBooking { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public DateOnly NgayNhanDuKien { get; set; }
        public DateOnly NgayTraDuKien { get; set; }
        public DateTime? NgayNhanThucTe { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        public int SoNguoi { get; set; }

        // Thông tin khách hàng
        public int IdKhachHang { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SdtKhachHang { get; set; } = string.Empty;
        public string CccdKhachHang { get; set; } = string.Empty;
        public string EmailKhachHang { get; set; } = string.Empty;

        // Danh sách phòng thuộc booking
        public List<ChiTietPhongDatDto> DanhSachPhong { get; set; } = new();
    }

    /// <summary>
    /// DTO thông tin từng phòng trong booking.
    /// </summary>
    public class ChiTietPhongDatDto
    {
        public int IdPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public string TrangThaiPhongHienTai { get; set; } = string.Empty;
    }
}
