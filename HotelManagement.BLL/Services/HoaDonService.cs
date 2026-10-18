using System;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ tính toán và quản lý hóa đơn khách sạn (DCKLTTQ-75).
    /// Tuân thủ nguyên tắc không tự tính đè computed column, tái sử dụng IUnitOfWork/IRepository.
    /// </summary>
    public class HoaDonService : IHoaDonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HoaDonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HoaDonDto?> LayHoaDonTheoMaBookingAsync(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return null;

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp == null)
                return null;

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong == null)
                return null;

            int soDem = Math.Max(1, datPhong.NgaytraDukien.DayNumber - datPhong.NgaynhanDukien.DayNumber);

            // Tái sử dụng đối tượng HoaDon đã được include sẵn từ GetChiTietDayDuAsync
            var hoaDon = datPhong.HoaDon;

            if (hoaDon != null)
            {
                return MapToDto(datPhong, hoaDon, soDem);
            }

            // Nếu chưa có hóa đơn chính thức, trả về dữ liệu tạm tính (Preview)
            decimal tienPhong = datPhong.ChiTietDatPhongs.Sum(ct => ct.Dongia) * soDem;
            decimal tienDichVu = datPhong.ChiTietDichVus.Sum(dv => dv.Soluong * dv.Dongia);

            return new HoaDonDto
            {
                IdHoaDon = 0,
                IdDatPhong = datPhong.IdDatphong,
                MaBooking = datPhong.Mabooking,
                TenKhachHang = datPhong.IdKhachhangNavigation?.Ten ?? string.Empty,
                SdtKhachHang = datPhong.IdKhachhangNavigation?.Sdt ?? string.Empty,
                CccdKhachHang = datPhong.IdKhachhangNavigation?.Cccd ?? string.Empty,
                EmailKhachHang = datPhong.IdKhachhangNavigation?.Email ?? string.Empty,
                NgayLap = DateTime.Now,
                TienPhong = tienPhong,
                TienDichVu = tienDichVu,
                GiamGia = 0,
                TongGia = tienPhong + tienDichVu, // Tạm tính cho màn hình xem trước
                PhuongThucThanhtoan = null,
                TrangThaiThanhtoan = "Chưa thanh toán",
                NgayNhanDuKien = datPhong.NgaynhanDukien,
                NgayTraDuKien = datPhong.NgaytraDukien,
                NgayNhanThucTe = datPhong.NgaynhanThucte,
                NgayTraThucTe = datPhong.NgaytraThucte,
                SoDemLuuTru = soDem,
                DanhSachPhong = datPhong.ChiTietDatPhongs.Select(ct => new ChiTietPhongDatDto
                {
                    IdPhong = ct.IdPhong,
                    SoPhong = ct.IdPhongNavigation?.Sophong ?? string.Empty,
                    TenLoaiPhong = ct.IdPhongNavigation?.IdLoaiphongNavigation?.Ten ?? string.Empty,
                    DonGia = ct.Dongia,
                    TrangThaiPhongHienTai = ct.IdPhongNavigation?.Trangthai ?? string.Empty
                }).ToList(),
                DanhSachDichVu = datPhong.ChiTietDichVus.Select(dv => new ChiTietDichVuDto
                {
                    Id = dv.Id,
                    IdDatPhong = dv.IdDatphong,
                    IdDichVu = dv.IdDichvu,
                    TenDichVu = dv.IdDichvuNavigation?.TenDichvu ?? string.Empty,
                    SoLuong = dv.Soluong,
                    DonGia = dv.Dongia,
                    ThoiGian = dv.Thoigian
                }).OrderBy(dv => dv.ThoiGian).ToList()
            };
        }

        public async Task<(bool Success, string Message, HoaDonDto? Data)> TinhVaLuuHoaDonAsync(string maBooking, decimal giamGia = 0)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return (false, "Vui lòng nhập mã booking hợp lệ.", null);

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp == null)
                return (false, $"Không tìm thấy thông tin đặt phòng với mã booking '{maBooking}'.", null);

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong == null)
                return (false, "Không thể tải chi tiết đặt phòng.", null);

            if (datPhong.Trangthai == "Đã hủy")
                return (false, "Booking này đã bị hủy, không thể lập hóa đơn.", null);

            int soDem = Math.Max(1, datPhong.NgaytraDukien.DayNumber - datPhong.NgaynhanDukien.DayNumber);

            // 1. Tính tổng tiền phòng và tổng tiền dịch vụ phát sinh
            decimal tienPhong = datPhong.ChiTietDatPhongs.Sum(ct => ct.Dongia) * soDem;
            decimal tienDichVu = datPhong.ChiTietDichVus.Sum(dv => dv.Soluong * dv.Dongia);

            // Validate giảm giá
            if (giamGia < 0)
                return (false, "Số tiền giảm giá không được là số âm.", null);

            if (giamGia > (tienPhong + tienDichVu))
                return (false, "Số tiền giảm giá không được vượt quá tổng tiền phòng và dịch vụ.", null);

            // 2. Sử dụng trực tiếp đối tượng HoaDon từ datPhong (đã được ChangeTracker theo dõi)
            // Tuyệt đối không dùng FindAsync AsNoTracking để tránh xung đột "another instance is already being tracked"
            var hoaDon = datPhong.HoaDon;

            if (hoaDon != null && hoaDon.TrangthaiThanhtoan == "Đã thanh toán")
            {
                return (false, $"Hóa đơn của booking '{maBooking}' đã được thanh toán hoàn tất, không thể chỉnh sửa số tiền.", MapToDto(datPhong, hoaDon, soDem));
            }

            try
            {
                if (hoaDon == null)
                {
                    // Tạo mới bản ghi hóa đơn
                    // Lưu ý: Tuyệt đối KHÔNG gán giá trị cho Tonggia vì cột này có ràng buộc COMPUTED COLUMN trong SQL Server.
                    hoaDon = new HoaDon
                    {
                        IdDatphong = datPhong.IdDatphong,
                        Ngaylap = DateTime.Now,
                        Tienphong = tienPhong,
                        Tiendichvu = tienDichVu,
                        Giamgia = giamGia,
                        TrangthaiThanhtoan = "Chưa thanh toán"
                    };

                    await _unitOfWork.HoaDonRepository.AddAsync(hoaDon);
                    datPhong.HoaDon = hoaDon;
                }
                else
                {
                    // Cập nhật lại số tiền cho hóa đơn hiện có
                    hoaDon.Tienphong = tienPhong;
                    hoaDon.Tiendichvu = tienDichVu;
                    hoaDon.Giamgia = giamGia;
                    hoaDon.Ngaylap = DateTime.Now;

                    _unitOfWork.HoaDonRepository.Update(hoaDon);
                }

                // Lưu dữ liệu xuống SQL Server.
                // SQL Server sẽ tự động tính toán giá trị computed column: tonggia = (tienphong + tiendichvu) - giamgia.
                await _unitOfWork.SaveChangesAsync();

                var dto = MapToDto(datPhong, hoaDon, soDem);
                return (true, "Tính và lưu hóa đơn thành công.", dto);
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? $"{ex.Message} ({ex.InnerException.Message})" : ex.Message;
                return (false, "Lỗi khi lưu hóa đơn: " + msg, null);
            }
        }

        public async Task<(bool Success, string Message)> CapNhatThanhToanAsync(
            string maBooking,
            string phuongThucThanhToan,
            string trangThaiThanhToan = "Đã thanh toán")
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return (false, "Vui lòng nhập mã booking hợp lệ.");

            if (string.IsNullOrWhiteSpace(phuongThucThanhToan))
                return (false, "Vui lòng chọn hình thức thanh toán.");

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp == null)
                return (false, $"Không tìm thấy thông tin đặt phòng với mã booking '{maBooking}'.");

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong == null)
                return (false, "Không thể tải chi tiết đặt phòng.");

            var hoaDon = datPhong.HoaDon;

            if (hoaDon == null)
            {
                // Nếu chưa có hóa đơn, tự động tính và lưu hóa đơn trước
                var (success, message, data) = await TinhVaLuuHoaDonAsync(maBooking, 0);
                if (!success || data == null)
                    return (false, "Không thể lập hóa đơn trước khi thanh toán: " + message);

                hoaDon = datPhong.HoaDon;
                if (hoaDon == null)
                {
                    var reload = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
                    hoaDon = reload?.HoaDon;
                }

                if (hoaDon == null)
                    return (false, "Không thể tìm thấy hóa đơn vừa lập.");
            }

            try
            {
                hoaDon.PhuongthucThanhtoan = phuongThucThanhToan.Trim();
                hoaDon.TrangthaiThanhtoan = trangThaiThanhToan.Trim();

                _unitOfWork.HoaDonRepository.Update(hoaDon);
                await _unitOfWork.SaveChangesAsync();

                return (true, $"Thanh toán thành công cho booking '{maBooking}' qua hình thức '{phuongThucThanhToan}'.");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? $"{ex.Message} ({ex.InnerException.Message})" : ex.Message;
                return (false, "Lỗi khi cập nhật trạng thái thanh toán: " + msg);
            }
        }

        private static HoaDonDto MapToDto(DatPhong datPhong, HoaDon hoaDon, int soDem)
        {
            return new HoaDonDto
            {
                IdHoaDon = hoaDon.IdHoadon,
                IdDatPhong = hoaDon.IdDatphong,
                MaBooking = datPhong.Mabooking,
                TenKhachHang = datPhong.IdKhachhangNavigation?.Ten ?? string.Empty,
                SdtKhachHang = datPhong.IdKhachhangNavigation?.Sdt ?? string.Empty,
                CccdKhachHang = datPhong.IdKhachhangNavigation?.Cccd ?? string.Empty,
                EmailKhachHang = datPhong.IdKhachhangNavigation?.Email ?? string.Empty,
                NgayLap = hoaDon.Ngaylap,
                TienPhong = hoaDon.Tienphong,
                TienDichVu = hoaDon.Tiendichvu,
                GiamGia = hoaDon.Giamgia,
                TongGia = hoaDon.Tonggia ?? ((hoaDon.Tienphong + hoaDon.Tiendichvu) - hoaDon.Giamgia),
                PhuongThucThanhtoan = hoaDon.PhuongthucThanhtoan,
                TrangThaiThanhtoan = hoaDon.TrangthaiThanhtoan,
                NgayNhanDuKien = datPhong.NgaynhanDukien,
                NgayTraDuKien = datPhong.NgaytraDukien,
                NgayNhanThucTe = datPhong.NgaynhanThucte,
                NgayTraThucTe = datPhong.NgaytraThucte,
                SoDemLuuTru = soDem,
                DanhSachPhong = datPhong.ChiTietDatPhongs.Select(ct => new ChiTietPhongDatDto
                {
                    IdPhong = ct.IdPhong,
                    SoPhong = ct.IdPhongNavigation?.Sophong ?? string.Empty,
                    TenLoaiPhong = ct.IdPhongNavigation?.IdLoaiphongNavigation?.Ten ?? string.Empty,
                    DonGia = ct.Dongia,
                    TrangThaiPhongHienTai = ct.IdPhongNavigation?.Trangthai ?? string.Empty
                }).ToList(),
                DanhSachDichVu = datPhong.ChiTietDichVus.Select(dv => new ChiTietDichVuDto
                {
                    Id = dv.Id,
                    IdDatPhong = dv.IdDatphong,
                    IdDichVu = dv.IdDichvu,
                    TenDichVu = dv.IdDichvuNavigation?.TenDichvu ?? string.Empty,
                    SoLuong = dv.Soluong,
                    DonGia = dv.Dongia,
                    ThoiGian = dv.Thoigian
                }).OrderBy(dv => dv.ThoiGian).ToList()
            };
        }
    }
}
