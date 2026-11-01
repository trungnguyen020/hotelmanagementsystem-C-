using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ tổng hợp dữ liệu số liệu & chuỗi thời gian cho Dashboard (DCKLTTQ-84).
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<DashboardDataDto> LayDuLieuDashboardAsync(DashboardFilterDto filter)
        {
            // 1. Chuẩn hóa khoảng thời gian
            DateTime tuNgay = filter.TuNgay.Date;
            DateTime denNgay = filter.DenNgay.Date;
            if (tuNgay > denNgay)
            {
                (tuNgay, denNgay) = (denNgay, tuNgay);
            }

            DateTime startDateTime = tuNgay;
            DateTime endDateTime = denNgay.AddDays(1).AddTicks(-1);
            DateOnly startDateOnly = DateOnly.FromDateTime(tuNgay);
            DateOnly endDateOnly = DateOnly.FromDateTime(denNgay);

            var result = new DashboardDataDto
            {
                Filter = new DashboardFilterDto { TuNgay = tuNgay, DenNgay = denNgay }
            };

            // 2. Thống kê trạng thái phòng hiện tại (Thời gian thực - dùng cho Biểu đồ Tròn)
            var danhSachPhong = (await _unitOfWork.PhongRepository.GetAllAsync()).ToList();
            int tongSoPhong = danhSachPhong.Count;

            var nhomTrangThai = danhSachPhong
                .GroupBy(p => p.Trangthai?.Trim() ?? "Chưa rõ")
                .Select(g => new TrangThaiPhongThongKeDto
                {
                    TrangThai = g.Key,
                    SoLuong = g.Count(),
                    TyLePhanTram = tongSoPhong > 0 ? Math.Round((double)g.Count() / tongSoPhong * 100, 1) : 0
                })
                .OrderByDescending(x => x.SoLuong)
                .ToList();

            result.CoCauTrangThaiPhong = nhomTrangThai;

            // 3. Lấy các hóa đơn "Đã thanh toán" trong khoảng ngày (Dùng cho Biểu đồ Cột)
            var hoaDons = (await _unitOfWork.HoaDonRepository.FindAsync(hd =>
                hd.TrangthaiThanhtoan == "Đã thanh toán" &&
                hd.Ngaylap >= startDateTime &&
                hd.Ngaylap <= endDateTime)).ToList();

            decimal tongDoanhThu = 0;
            decimal tongTienPhong = 0;
            decimal tongTienDichVu = 0;

            // Tạo danh sách tất cả các ngày trong khoảng lọc
            int soNgay = (denNgay - tuNgay).Days + 1;
            var danhSachNgay = Enumerable.Range(0, soNgay)
                .Select(offset => tuNgay.AddDays(offset))
                .ToList();

            var doanhThuTheoNgayList = new List<DoanhThuTheoNgayDto>();

            foreach (var d in danhSachNgay)
            {
                DateTime nextD = d.AddDays(1);
                var hoaDonsTrongNgay = hoaDons.Where(h => h.Ngaylap >= d && h.Ngaylap < nextD).ToList();

                decimal tp = hoaDonsTrongNgay.Sum(h => h.Tienphong);
                decimal tdv = hoaDonsTrongNgay.Sum(h => h.Tiendichvu);
                decimal tong = hoaDonsTrongNgay.Sum(h => h.Tonggia ?? ((h.Tienphong + h.Tiendichvu) - h.Giamgia));

                tongTienPhong += tp;
                tongTienDichVu += tdv;
                tongDoanhThu += tong;

                doanhThuTheoNgayList.Add(new DoanhThuTheoNgayDto
                {
                    Ngay = d,
                    NhanNgay = d.ToString("dd/MM"),
                    TienPhong = tp,
                    TienDichVu = tdv,
                    TongDoanhThu = tong
                });
            }
            result.DoanhThuTheoNgay = doanhThuTheoNgayList;

            // 4. Tính toán Tỷ lệ lấp đầy theo từng ngày (Dùng cho Biểu đồ Đường)
            // Lấy các booking hợp lệ giao nhau với khoảng thời gian lọc
            var bookingsGiaoKy = (await _unitOfWork.DatPhongRepository.FindAsync(dp =>
                dp.Trangthai != "Đã hủy" &&
                dp.NgaynhanDukien <= endDateOnly &&
                dp.NgaytraDukien >= startDateOnly)).ToList();

            // Nạp chi tiết đầy đủ để biết số lượng phòng trong mỗi booking
            var bookingsChiTiet = new List<DatPhong>();
            foreach (var b in bookingsGiaoKy)
            {
                var full = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(b.IdDatphong);
                if (full != null) bookingsChiTiet.Add(full);
            }

            var xuHuongLapDayList = new List<TyLeLapDayTheoNgayDto>();
            double tongTyLeCacNgay = 0;

            foreach (var d in danhSachNgay)
            {
                DateOnly curDateOnly = DateOnly.FromDateTime(d);

                // Khách lưu trú tại ngày d: Ngày nhận <= d < Ngày trả (ngày trả là ngày check-out)
                // Riêng trường hợp nhận và trả cùng ngày (ở trong ngày) thì tính nếu curDateOnly == NgaynhanDukien
                var bookingsTrongNgay = bookingsChiTiet.Where(dp =>
                    (dp.NgaynhanDukien <= curDateOnly && dp.NgaytraDukien > curDateOnly) ||
                    (dp.NgaynhanDukien == dp.NgaytraDukien && dp.NgaynhanDukien == curDateOnly)
                ).ToList();

                int soPhongCoKhach = bookingsTrongNgay.Sum(dp => dp.ChiTietDatPhongs.Count);
                if (soPhongCoKhach > tongSoPhong) soPhongCoKhach = tongSoPhong; // Giới hạn logic tối đa 100%

                double tyLeNgay = tongSoPhong > 0
                    ? Math.Min(100.0, Math.Round((double)soPhongCoKhach / tongSoPhong * 100, 1))
                    : 0;

                tongTyLeCacNgay += tyLeNgay;

                xuHuongLapDayList.Add(new TyLeLapDayTheoNgayDto
                {
                    Ngay = d,
                    NhanNgay = d.ToString("dd/MM"),
                    TongSoPhong = tongSoPhong,
                    SoPhongCoKhach = soPhongCoKhach,
                    TyLeLapDayPhanTram = tyLeNgay
                });
            }
            result.XuHuongLapDayTheoNgay = xuHuongLapDayList;

            // 5. Tổng hợp KPIs
            int soPhongDangO = danhSachPhong.Count(p => p.Trangthai != null && p.Trangthai.Contains("Đang ở", StringComparison.OrdinalIgnoreCase));
            int soPhongTrong = danhSachPhong.Count(p => p.Trangthai != null && p.Trangthai.Contains("Trống", StringComparison.OrdinalIgnoreCase));

            result.Kpi = new DashboardKpiDto
            {
                TongDoanhThu = tongDoanhThu,
                TongTienPhong = tongTienPhong,
                TongTienDichVu = tongTienDichVu,
                TyLeLapDayTrungBinh = danhSachNgay.Count > 0 ? Math.Round(tongTyLeCacNgay / danhSachNgay.Count, 1) : 0,
                TongSoLuotDat = bookingsChiTiet.Count,
                TongSoPhong = tongSoPhong,
                SoPhongDangO = soPhongDangO,
                SoPhongTrong = soPhongTrong
            };

            return result;
        }
    }
}
