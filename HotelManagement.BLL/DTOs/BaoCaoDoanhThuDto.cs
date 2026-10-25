using System;
using System.Collections.Generic;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// Tham số lọc báo cáo doanh thu theo khoảng ngày và loại phòng (DCKLTTQ-78).
    /// </summary>
    public class BaoCaoFilterDto
    {
        public DateTime TuNgay { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime DenNgay { get; set; } = DateTime.Today;
        public int? IdLoaiPhong { get; set; }
        public string? TenLoaiPhong { get; set; }
    }

    /// <summary>
    /// Thống kê doanh thu và tỷ trọng theo từng loại phòng (Breakdown).
    /// </summary>
    public class DoanhThuTheoLoaiPhongDto
    {
        public int IdLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public int SoLuongPhong { get; set; }
        public int SoLuotDat { get; set; }
        public int TongSoDem { get; set; }
        public decimal DoanhThuPhong { get; set; }
        public double TyTrongPhanTram { get; set; }
    }

    /// <summary>
    /// Thông tin chi tiết từng hóa đơn trong kỳ báo cáo.
    /// </summary>
    public class ChiTietHoaDonBaoCaoDto
    {
        public int IdHoaDon { get; set; }
        public string MaBooking { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public string DanhSachPhong { get; set; } = string.Empty;
        public DateTime NgayThanhToan { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongTien { get; set; } // Lấy trực tiếp từ computed column Tonggia
        public string PhuongThucThanhToan { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO tổng hợp toàn bộ kết quả báo cáo doanh thu & hiệu suất phòng (DCKLTTQ-78).
    /// </summary>
    public class BaoCaoDoanhThuDto
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string? TenLoaiPhongLoc { get; set; }
        public DateTime NgayXuatBaoCao { get; set; } = DateTime.Now;
        public string NguoiLapBaoCao { get; set; } = "Admin";

        // Chỉ số tổng quan (KPIs)
        public int TongSoBooking { get; set; }
        public int TongSoDemLuuTru { get; set; }
        public decimal TongDoanhThu { get; set; } // Tổng computed column Tonggia
        public decimal TongTienPhong { get; set; }
        public decimal TongTienDichVu { get; set; }
        public decimal TongGiamGia { get; set; }

        // Tỷ lệ lấp đầy phòng (Occupancy Rate)
        public int TongSoPhongKhachSan { get; set; }
        public int TongSoDemKhaDung { get; set; }
        public int TongSoDemDaPhucVu { get; set; }
        public double TyLeLapDayPhanTram { get; set; }

        // Dữ liệu phân rã & chi tiết
        public List<DoanhThuTheoLoaiPhongDto> DanhSachLoaiPhong { get; set; } = new();
        public List<ChiTietHoaDonBaoCaoDto> DanhSachHoaDon { get; set; } = new();
    }
}
