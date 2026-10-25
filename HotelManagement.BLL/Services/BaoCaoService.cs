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
    /// Triển khai dịch vụ tổng hợp báo cáo doanh thu và tỷ lệ lấp đầy phòng (DCKLTTQ-78).
    /// Tái sử dụng IUnitOfWork/Repository hiện có, lấy tổng tiền từ computed column của HoaDon.
    /// </summary>
    public class BaoCaoService : IBaoCaoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaoCaoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LoaiPhongDto>> LayDanhSachLoaiPhongAsync()
        {
            var ds = await _unitOfWork.LoaiPhongRepository.GetAllAsync();
            return ds.Select(lp => new LoaiPhongDto
            {
                Id = lp.IdLoaiphong,
                TenLoaiPhong = lp.Ten,
                Gia = lp.Mucgia,
                SucChua = lp.Succhua,
                MoTa = lp.Mota
            }).OrderBy(lp => lp.TenLoaiPhong).ToList();
        }

        public async Task<BaoCaoDoanhThuDto> LayBaoCaoDoanhThuAsync(BaoCaoFilterDto filter, string nguoiLap = "Admin")
        {
            DateTime start = filter.TuNgay.Date;
            DateTime end = filter.DenNgay.Date.AddDays(1).AddTicks(-1);
            DateOnly startDateOnly = DateOnly.FromDateTime(filter.TuNgay.Date);
            DateOnly endDateOnly = DateOnly.FromDateTime(filter.DenNgay.Date);

            // 1. Lấy danh sách hóa đơn "Đã thanh toán" nằm trong khoảng ngày lập
            var hoaDons = await _unitOfWork.HoaDonRepository.FindAsync(hd =>
                hd.TrangthaiThanhtoan == "Đã thanh toán" &&
                hd.Ngaylap >= start &&
                hd.Ngaylap <= end);

            var tatCaPhong = (await _unitOfWork.PhongRepository.GetAllAsync()).ToList();
            var tatCaLoaiPhong = (await _unitOfWork.LoaiPhongRepository.GetAllAsync()).ToList();

            var danhSachHoaDonChiTiet = new List<ChiTietHoaDonBaoCaoDto>();
            var danhSachDatPhongTrongKy = new List<DatPhong>();

            foreach (var hd in hoaDons.OrderByDescending(h => h.Ngaylap))
            {
                var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(hd.IdDatphong);
                if (datPhong == null) continue;

                // Lọc theo loại phòng nếu người dùng chỉ định
                if (filter.IdLoaiPhong.HasValue)
                {
                    bool coPhongThuocLoai = datPhong.ChiTietDatPhongs
                        .Any(ct => ct.IdPhongNavigation?.IdLoaiphong == filter.IdLoaiPhong.Value);

                    if (!coPhongThuocLoai) continue;
                }

                danhSachDatPhongTrongKy.Add(datPhong);

                // Lưu ý: Giá trị tổng tiền lấy trực tiếp từ computed column Tonggia của SQL Server
                decimal tongTien = hd.Tonggia ?? ((hd.Tienphong + hd.Tiendichvu) - hd.Giamgia);

                string danhSachPhongStr = string.Join(", ", datPhong.ChiTietDatPhongs
                    .Select(ct => ct.IdPhongNavigation?.Sophong)
                    .Where(sp => !string.IsNullOrEmpty(sp)));

                danhSachHoaDonChiTiet.Add(new ChiTietHoaDonBaoCaoDto
                {
                    IdHoaDon = hd.IdHoadon,
                    MaBooking = datPhong.Mabooking,
                    TenKhachHang = datPhong.IdKhachhangNavigation?.Ten ?? "Khách vãng lai",
                    DanhSachPhong = danhSachPhongStr,
                    NgayThanhToan = hd.Ngaylap,
                    TienPhong = hd.Tienphong,
                    TienDichVu = hd.Tiendichvu,
                    GiamGia = hd.Giamgia,
                    TongTien = tongTien,
                    PhuongThucThanhToan = hd.PhuongthucThanhtoan ?? "Tiền mặt"
                });
            }

            // 2. Tổng hợp doanh thu
            decimal tongDoanhThu = danhSachHoaDonChiTiet.Sum(h => h.TongTien);
            decimal tongTienPhong = danhSachHoaDonChiTiet.Sum(h => h.TienPhong);
            decimal tongTienDichVu = danhSachHoaDonChiTiet.Sum(h => h.TienDichVu);
            decimal tongGiamGia = danhSachHoaDonChiTiet.Sum(h => h.GiamGia);
            int tongSoBooking = danhSachDatPhongTrongKy.Count;

            // 3. Breakdown doanh thu theo từng loại phòng
            var danhSachBreakdown = new List<DoanhThuTheoLoaiPhongDto>();
            var loaiPhongsXet = filter.IdLoaiPhong.HasValue
                ? tatCaLoaiPhong.Where(lp => lp.IdLoaiphong == filter.IdLoaiPhong.Value).ToList()
                : tatCaLoaiPhong;

            foreach (var lp in loaiPhongsXet)
            {
                int soPhongCuaLoai = tatCaPhong.Count(p => p.IdLoaiphong == lp.IdLoaiphong);

                // Lọc các booking trong kỳ có phòng thuộc loại này
                var datPhongsCuaLoai = danhSachDatPhongTrongKy
                    .Where(dp => dp.ChiTietDatPhongs.Any(ct => ct.IdPhongNavigation?.IdLoaiphong == lp.IdLoaiphong))
                    .ToList();

                int soLuotDat = datPhongsCuaLoai.Count;
                int tongSoDemCuaLoai = 0;
                decimal doanhThuLoai = 0;

                foreach (var dp in datPhongsCuaLoai)
                {
                    int soDem = Math.Max(1, dp.NgaytraDukien.DayNumber - dp.NgaynhanDukien.DayNumber);
                    var chiTietLoai = dp.ChiTietDatPhongs
                        .Where(ct => ct.IdPhongNavigation?.IdLoaiphong == lp.IdLoaiphong)
                        .ToList();

                    tongSoDemCuaLoai += soDem * chiTietLoai.Count;
                    doanhThuLoai += chiTietLoai.Sum(ct => ct.Dongia) * soDem;
                }

                double tyTrong = tongTienPhong > 0 ? (double)(doanhThuLoai / tongTienPhong) * 100 : 0;

                danhSachBreakdown.Add(new DoanhThuTheoLoaiPhongDto
                {
                    IdLoaiPhong = lp.IdLoaiphong,
                    TenLoaiPhong = lp.Ten,
                    SoLuongPhong = soPhongCuaLoai,
                    SoLuotDat = soLuotDat,
                    TongSoDem = tongSoDemCuaLoai,
                    DoanhThuPhong = doanhThuLoai,
                    TyTrongPhanTram = Math.Round(tyTrong, 1)
                });
            }

            // 4. Tính toán Tỷ lệ lấp đầy phòng (Occupancy Rate)
            int soNgayTrongKy = Math.Max(1, (filter.DenNgay.Date - filter.TuNgay.Date).Days + 1);
            int tongSoPhongKhachSan = filter.IdLoaiPhong.HasValue
                ? tatCaPhong.Count(p => p.IdLoaiphong == filter.IdLoaiPhong.Value)
                : tatCaPhong.Count;

            int tongDemKhaDung = tongSoPhongKhachSan * soNgayTrongKy;

            // Lấy các booking hợp lệ giao nhau với khoảng thời gian lọc để tính số đêm thực tế đã ở
            var bookingLuuTru = await _unitOfWork.DatPhongRepository.FindAsync(dp =>
                dp.Trangthai != "Đã hủy" &&
                dp.NgaynhanDukien <= endDateOnly &&
                dp.NgaytraDukien >= startDateOnly);

            int tongDemDaPhucVu = 0;
            foreach (var dp in bookingLuuTru)
            {
                var dpChiTiet = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
                if (dpChiTiet == null) continue;

                var phongsThuocLoai = filter.IdLoaiPhong.HasValue
                    ? dpChiTiet.ChiTietDatPhongs.Where(ct => ct.IdPhongNavigation?.IdLoaiphong == filter.IdLoaiPhong.Value).ToList()
                    : dpChiTiet.ChiTietDatPhongs.ToList();

                if (!phongsThuocLoai.Any()) continue;

                DateOnly nhan = dp.NgaynhanDukien > startDateOnly ? dp.NgaynhanDukien : startDateOnly;
                DateOnly tra = dp.NgaytraDukien < endDateOnly ? dp.NgaytraDukien : endDateOnly;
                int soDemGiao = Math.Max(0, tra.DayNumber - nhan.DayNumber);

                tongDemDaPhucVu += soDemGiao * phongsThuocLoai.Count;
            }

            double tyLeLapDay = tongDemKhaDung > 0
                ? Math.Min(100.0, Math.Round(((double)tongDemDaPhucVu / tongDemKhaDung) * 100, 1))
                : 0;

            int tongDemLuuTru = danhSachBreakdown.Sum(b => b.TongSoDem);

            return new BaoCaoDoanhThuDto
            {
                TuNgay = filter.TuNgay,
                DenNgay = filter.DenNgay,
                TenLoaiPhongLoc = filter.TenLoaiPhong,
                NgayXuatBaoCao = DateTime.Now,
                NguoiLapBaoCao = nguoiLap,
                TongSoBooking = tongSoBooking,
                TongSoDemLuuTru = tongDemLuuTru,
                TongDoanhThu = tongDoanhThu,
                TongTienPhong = tongTienPhong,
                TongTienDichVu = tongTienDichVu,
                TongGiamGia = tongGiamGia,
                TongSoPhongKhachSan = tongSoPhongKhachSan,
                TongSoDemKhaDung = tongDemKhaDung,
                TongSoDemDaPhucVu = tongDemDaPhucVu,
                TyLeLapDayPhanTram = tyLeLapDay,
                DanhSachLoaiPhong = danhSachBreakdown,
                DanhSachHoaDon = danhSachHoaDonChiTiet
            };
        }
    }
}
