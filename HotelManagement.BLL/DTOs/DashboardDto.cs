using System;
using System.Collections.Generic;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// Tham số lọc dữ liệu cho Dashboard (DCKLTTQ-84).
    /// </summary>
    public class DashboardFilterDto
    {
        public DateTime TuNgay { get; set; } = DateTime.Today.AddDays(-6); // Mặc định 7 ngày gần nhất
        public DateTime DenNgay { get; set; } = DateTime.Today;
    }

    /// <summary>
    /// Các chỉ số KPI tổng hợp nhanh hiển thị trên thẻ card đầu Dashboard.
    /// </summary>
    public class DashboardKpiDto
    {
        public decimal TongDoanhThu { get; set; }
        public decimal TongTienPhong { get; set; }
        public decimal TongTienDichVu { get; set; }
        public double TyLeLapDayTrungBinh { get; set; } // %
        public int TongSoLuotDat { get; set; }
        public int TongSoPhong { get; set; }
        public int SoPhongDangO { get; set; }
        public int SoPhongTrong { get; set; }
    }

    /// <summary>
    /// Doanh thu phân rã theo ngày trong khoảng lọc (Dùng cho Biểu đồ Cột).
    /// </summary>
    public class DoanhThuTheoNgayDto
    {
        public DateTime Ngay { get; set; }
        public string NhanNgay { get; set; } = string.Empty; // "dd/MM"
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    /// <summary>
    /// Phân bổ cơ cấu trạng thái phòng hiện tại (Dùng cho Biểu đồ Tròn/Donut).
    /// </summary>
    public class TrangThaiPhongThongKeDto
    {
        public string TrangThai { get; set; } = string.Empty; // "Trống", "Đang ở", "Đang dọn", "Bảo trì"
        public int SoLuong { get; set; }
        public double TyLePhanTram { get; set; }
    }

    /// <summary>
    /// Tỷ lệ lấp đầy phòng theo từng ngày trong khoảng lọc (Dùng cho Biểu đồ Đường).
    /// </summary>
    public class TyLeLapDayTheoNgayDto
    {
        public DateTime Ngay { get; set; }
        public string NhanNgay { get; set; } = string.Empty; // "dd/MM"
        public int TongSoPhong { get; set; }
        public int SoPhongCoKhach { get; set; }
        public double TyLeLapDayPhanTram { get; set; }
    }

    /// <summary>
    /// DTO đóng gói toàn bộ dữ liệu cung cấp cho FormDashboard.
    /// </summary>
    public class DashboardDataDto
    {
        public DashboardFilterDto Filter { get; set; } = new();
        public DashboardKpiDto Kpi { get; set; } = new();
        public List<DoanhThuTheoNgayDto> DoanhThuTheoNgay { get; set; } = new();
        public List<TrangThaiPhongThongKeDto> CoCauTrangThaiPhong { get; set; } = new();
        public List<TyLeLapDayTheoNgayDto> XuHuongLapDayTheoNgay { get; set; } = new();
    }
}
