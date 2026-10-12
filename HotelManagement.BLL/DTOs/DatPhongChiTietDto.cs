using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO chứa thông tin đầy đủ của một lượt đặt phòng phục vụ màn hình Check-in / Check-out / Tra cứu (DCKLTTQ-71, DCKLTTQ-72).
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

        // Danh sách dịch vụ phát sinh thuộc booking
        public List<ChiTietDichVuDto> DanhSachDichVu { get; set; } = new();

        // Tiền tạm tính
        public decimal TongTienPhong => DanhSachPhong.Sum(p => p.DonGia) * Math.Max(1, (NgayTraDuKien.DayNumber - NgayNhanDuKien.DayNumber));
        public decimal TongTienDichVu => DanhSachDichVu.Sum(dv => dv.ThanhTien);
        public decimal TongTienTamTinh => TongTienPhong + TongTienDichVu;
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

    /// <summary>
    /// DTO chi tiết dịch vụ đã sử dụng trong đợt lưu trú (DCKLTTQ-72).
    /// </summary>
    public class ChiTietDichVuDto
    {
        public int Id { get; set; }
        public int IdDatPhong { get; set; }
        public int IdDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        public DateTime ThoiGian { get; set; }
    }

    /// <summary>
    /// DTO danh mục dịch vụ để hiển thị chọn khi thêm dịch vụ phát sinh (DCKLTTQ-72).
    /// </summary>
    public class DichVuDto
    {
        public int IdDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal MucGia { get; set; }
    }
}
