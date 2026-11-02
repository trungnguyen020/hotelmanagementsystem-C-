using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ tra cứu thông tin đặt phòng tại Kiosk công cộng (DCKLTTQ-85).
    /// </summary>
    public class KioskService : IKioskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public KioskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<KioskBookingDto?> TraCuuBookingAsync(string maBooking, string sdt)
        {
            if (string.IsNullOrWhiteSpace(maBooking) || string.IsNullOrWhiteSpace(sdt))
            {
                return null;
            }

            string cleanMaBooking = maBooking.Trim().ToUpperInvariant();

            // 1. Tìm đơn đặt phòng theo mã booking
            var datPhongSoBo = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(cleanMaBooking);
            if (datPhongSoBo == null)
            {
                return null; // Không tìm thấy mã booking
            }

            // 2. Nạp chi tiết đầy đủ của đơn đặt phòng (kèm Khách hàng, Phòng, Dịch vụ, Hóa đơn)
            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(datPhongSoBo.IdDatphong);
            if (datPhong == null || datPhong.IdKhachhangNavigation == null)
            {
                return null;
            }

            // 3. Xác thực số điện thoại (BẮT BUỘC TRÙNG KHỚP để bảo mật dữ liệu khách khác)
            string sdtKhach = ChuanHoaSoDienThoai(datPhong.IdKhachhangNavigation.Sdt);
            string sdtNhap = ChuanHoaSoDienThoai(sdt);

            if (!string.Equals(sdtKhach, sdtNhap, StringComparison.OrdinalIgnoreCase))
            {
                return null; // Số điện thoại không khớp với booking này
            }

            // 4. Đóng gói DTO an toàn (Loại bỏ toàn bộ CCCD, thông tin nhân sự, ghi chú nội bộ)
            int soDem = Math.Max(1, datPhong.NgaytraDukien.DayNumber - datPhong.NgaynhanDukien.DayNumber);

            var danhSachPhong = datPhong.ChiTietDatPhongs
                .Select(ct => new KioskPhongDto
                {
                    SoPhong = ct.IdPhongNavigation?.Sophong ?? "Chưa chỉ định",
                    TenLoaiPhong = ct.IdPhongNavigation?.IdLoaiphongNavigation?.Ten ?? "Phòng tiêu chuẩn"
                })
                .ToList();

            var danhSachDichVu = datPhong.ChiTietDichVus
                .Select(dv => dv.IdDichvuNavigation?.TenDichvu ?? "Dịch vụ")
                .Where(ten => !string.IsNullOrEmpty(ten))
                .Distinct()
                .ToList();

            decimal tongTien = datPhong.HoaDon?.Tonggia
                ?? (datPhong.ChiTietDatPhongs.Sum(ct => ct.Dongia) * soDem + datPhong.ChiTietDichVus.Sum(dv => dv.Dongia));

            string trangThaiThanhToan = datPhong.HoaDon?.TrangthaiThanhtoan ?? "Chưa thanh toán";

            return new KioskBookingDto
            {
                MaBooking = datPhong.Mabooking,
                TenKhachHang = datPhong.IdKhachhangNavigation.Ten,
                TrangThaiBooking = datPhong.Trangthai,
                NgayNhanDuKien = datPhong.NgaynhanDukien,
                NgayTraDuKien = datPhong.NgaytraDukien,
                SoDem = soDem,
                SoNguoi = datPhong.Songuoi,
                DanhSachPhong = danhSachPhong,
                DanhSachDichVu = danhSachDichVu,
                TrangThaiThanhToan = trangThaiThanhToan,
                TongTien = tongTien
            };
        }

        private static string ChuanHoaSoDienThoai(string? sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return string.Empty;
            return Regex.Replace(sdt.Trim(), @"[^\d]", ""); // Chỉ giữ lại các chữ số
        }
    }
}

