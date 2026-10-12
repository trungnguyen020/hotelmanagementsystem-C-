using System;
using System.Collections.Generic;
using System.Text;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;

namespace HotelManagement.BLL.Services
{
    public class DatPhongService : IDatPhongService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DatPhongService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<Phongtrongdto>> TimPhongTrongAsync(
            DateOnly ngayNhan, DateOnly ngayTra, int? idLoaiPhong, int? soKhach)
        {
            if (ngayNhan >= ngayTra)
                throw new ArgumentException("Ngày nhận phòng phải trước ngày trả phòng.");

            if (ngayNhan < DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException("Ngày nhận phòng không được ở quá khứ.");

            if (soKhach.HasValue && soKhach.Value <= 0)
                throw new ArgumentException("Số khách phải lớn hơn 0.");

            var danhSachPhong = await _unitOfWork.DatPhongRepository
                .GetPhongTrongAsync(ngayNhan, ngayTra, idLoaiPhong, soKhach);

            return danhSachPhong.Select(p => new Phongtrongdto
            {
                IdPhong = p.IdPhong,
                SoPhong = p.Sophong,
                IdLoaiPhong = p.IdLoaiphong,
                TenLoaiPhong = p.IdLoaiphongNavigation.Ten,
                Gia = p.IdLoaiphongNavigation.Mucgia,
                SucChua = p.IdLoaiphongNavigation.Succhua
            }).ToList();
        }

        public async Task<(bool Success, string Message)> DatPhongAsync(
            int idKhachHang, int idNhanSu, DateOnly ngayNhan, DateOnly ngayTra, int soNguoi, List<int> danhSachIdPhong)
        {
            if (danhSachIdPhong == null || !danhSachIdPhong.Any())
                return (false, "Vui lòng chọn ít nhất 1 phòng để đặt.");

            if (ngayNhan >= ngayTra)
                return (false, "Ngày nhận phòng phải trước ngày trả phòng.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Kiểm tra lại lần cuối xem phòng còn trống không (tránh race condition)
                foreach (var idPhong in danhSachIdPhong)
                {
                    bool isTrong = await _unitOfWork.DatPhongRepository.IsPhongTrongAsync(idPhong, ngayNhan, ngayTra);
                    if (!isTrong)
                    {
                        var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                        await _unitOfWork.RollbackTransactionAsync();
                        return (false, $"Phòng {phong?.Sophong} đã được đặt trong khoảng thời gian này.");
                    }
                }

                // Sinh mã booking ngẫu nhiên / unique
                string maBooking = "BK" + DateTime.Now.ToString("yyMMddHHmmss");

                var datPhong = new HotelManagement.Models.Entities.DatPhong
                {
                    IdKhachhang = idKhachHang,
                    IdNhansu = idNhanSu,
                    NgaynhanDukien = ngayNhan,
                    NgaytraDukien = ngayTra,
                    Songuoi = soNguoi,
                    Mabooking = maBooking,
                    Trangthai = "Đã xác nhận",
                    // Ngaydat sẽ được tự động tạo do DF_DatPhong_NgayDat
                };

                await _unitOfWork.DatPhongRepository.AddAsync(datPhong);
                await _unitOfWork.SaveChangesAsync(); // Lưu để lấy IdDatphong

                // Thêm chi tiết đặt phòng
                foreach (var idPhong in danhSachIdPhong)
                {
                    var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                    // Lưu ý: Cần join sang LoaiPhong để lấy đúng giá phòng hiện tại,
                    // do GetByIdAsync không tự Include, ta có thể tự Get LoaiPhong
                    decimal donGia = 0;
                    if (phong != null)
                    {
                        var loaiPhong = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(phong.IdLoaiphong);
                        donGia = loaiPhong?.Mucgia ?? 0;
                    }

                    var ctDatPhong = new HotelManagement.Models.Entities.ChiTietDatPhong
                    {
                        IdDatphong = datPhong.IdDatphong,
                        IdPhong = idPhong,
                        Dongia = donGia
                    };

                    // Add bằng reflection hoặc DbContext nếu không có repository riêng
                    // Ta chưa viết repository riêng cho ChiTietDatPhong nên dùng DbContext thông qua Repository Generic
                    // May thay ta không khai báo DbSet cho ChiTietDatPhong trong IUnitOfWork?
                    // Không sao, Entity Framework tự track navigation property ChiTietDatPhongs
                    datPhong.ChiTietDatPhongs.Add(ctDatPhong);
                }

                // Lưu các chi tiết đặt phòng vừa thêm qua navigation property
                await _unitOfWork.SaveChangesAsync();
                
                await _unitOfWork.CommitTransactionAsync();
                
                return (true, $"Đặt phòng thành công! Mã booking: {maBooking}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Có lỗi xảy ra khi đặt phòng: " + ex.Message);
            }
        }

        public async Task<DatPhongChiTietDto?> LayChiTietTheoMaBookingAsync(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return null;

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp is null)
                return null;

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong is null)
                return null;

            return new DatPhongChiTietDto
            {
                IdDatPhong = datPhong.IdDatphong,
                MaBooking = datPhong.Mabooking,
                TrangThai = datPhong.Trangthai,
                NgayDat = datPhong.Ngaydat,
                NgayNhanDuKien = datPhong.NgaynhanDukien,
                NgayTraDuKien = datPhong.NgaytraDukien,
                NgayNhanThucTe = datPhong.NgaynhanThucte,
                NgayTraThucTe = datPhong.NgaytraThucte,
                SoNguoi = datPhong.Songuoi,
                IdKhachHang = datPhong.IdKhachhang,
                TenKhachHang = datPhong.IdKhachhangNavigation?.Ten ?? string.Empty,
                SdtKhachHang = datPhong.IdKhachhangNavigation?.Sdt ?? string.Empty,
                CccdKhachHang = datPhong.IdKhachhangNavigation?.Cccd ?? string.Empty,
                EmailKhachHang = datPhong.IdKhachhangNavigation?.Email ?? string.Empty,
                DanhSachPhong = datPhong.ChiTietDatPhongs.Select(ct => new ChiTietPhongDatDto
                {
                    IdPhong = ct.IdPhong,
                    SoPhong = ct.IdPhongNavigation?.Sophong ?? string.Empty,
                    TenLoaiPhong = ct.IdPhongNavigation?.IdLoaiphongNavigation?.Ten ?? string.Empty,
                    DonGia = ct.Dongia,
                    TrangThaiPhongHienTai = ct.IdPhongNavigation?.Trangthai ?? string.Empty
                }).ToList(),
                DanhSachDichVu = datPhong.ChiTietDichVus.Select(cd => new ChiTietDichVuDto
                {
                    Id = cd.Id,
                    IdDatPhong = cd.IdDatphong,
                    IdDichVu = cd.IdDichvu,
                    TenDichVu = cd.IdDichvuNavigation?.TenDichvu ?? string.Empty,
                    SoLuong = cd.Soluong,
                    DonGia = cd.Dongia,
                    ThoiGian = cd.Thoigian
                }).OrderBy(cd => cd.ThoiGian).ToList()
            };
        }

        public async Task<(bool Success, string Message)> CheckInAsync(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return (false, "Vui lòng nhập mã booking.");

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp is null)
                return (false, $"Không tìm thấy thông tin đặt phòng với mã booking '{maBooking}'.");

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong is null)
                return (false, "Không thể tải chi tiết đặt phòng.");

            // Kiểm tra trạng thái booking
            if (datPhong.Trangthai == "Đã check-in")
            {
                string thoiGian = datPhong.NgaynhanThucte.HasValue
                    ? datPhong.NgaynhanThucte.Value.ToString("dd/MM/yyyy HH:mm")
                    : "trước đó";
                return (false, $"Mã booking này đã được check-in vào lúc {thoiGian}.");
            }

            if (datPhong.Trangthai == "Đã check-out")
                return (false, "Mã booking này đã hoàn tất check-out.");

            if (datPhong.Trangthai == "Đã hủy")
                return (false, "Mã booking này đã bị hủy, không thể check-in.");

            if (datPhong.Trangthai != "Đã xác nhận")
                return (false, $"Trạng thái booking ({datPhong.Trangthai}) không hợp lệ để check-in.");

            if (datPhong.ChiTietDatPhongs == null || !datPhong.ChiTietDatPhongs.Any())
                return (false, "Booking không có thông tin chi tiết phòng.");

            // Kiểm tra trạng thái hiện tại của từng phòng
            foreach (var ct in datPhong.ChiTietDatPhongs)
            {
                var phong = ct.IdPhongNavigation ?? await _unitOfWork.PhongRepository.GetByIdAsync(ct.IdPhong);
                if (phong == null)
                    return (false, $"Không tìm thấy thông tin phòng (ID: {ct.IdPhong}).");

                if (phong.Trangthai == "Bảo trì")
                    return (false, $"Phòng {phong.Sophong} hiện đang ở trạng thái 'Bảo trì', không thể check-in.");

                if (phong.Trangthai == "Đang ở")
                    return (false, $"Phòng {phong.Sophong} hiện đang có khách lưu trú ('Đang ở'). Vui lòng kiểm tra lại sơ đồ phòng.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Cập nhật trạng thái đặt phòng và ngày nhận thực tế
                datPhong.Trangthai = "Đã check-in";
                datPhong.NgaynhanThucte = DateTime.Now;
                _unitOfWork.DatPhongRepository.Update(datPhong);

                // Cập nhật trạng thái từng phòng sang "Đang ở"
                foreach (var ct in datPhong.ChiTietDatPhongs)
                {
                    var phong = ct.IdPhongNavigation ?? await _unitOfWork.PhongRepository.GetByIdAsync(ct.IdPhong);
                    if (phong != null)
                    {
                        phong.Trangthai = "Đang ở";
                        _unitOfWork.PhongRepository.Update(phong);
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                return (true, $"Check-in thành công cho mã booking {datPhong.Mabooking}!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Có lỗi xảy ra khi thực hiện check-in: " + ex.Message);
            }
        }

        public async Task<List<DichVuDto>> LayDanhSachDichVuAsync()
        {
            var ds = await _unitOfWork.DichVuRepository.GetAllAsync();
            return ds.Select(d => new DichVuDto
            {
                IdDichVu = d.IdDichvu,
                TenDichVu = d.TenDichvu,
                MucGia = d.Mucgia
            }).OrderBy(d => d.TenDichVu).ToList();
        }

        public async Task<(bool Success, string Message)> ThemDichVuPhatSinhAsync(string maBooking, int idDichVu, int soLuong)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return (false, "Vui lòng nhập mã booking.");

            if (soLuong <= 0)
                return (false, "Số lượng dịch vụ phải lớn hơn 0.");

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp is null)
                return (false, $"Không tìm thấy thông tin đặt phòng với mã booking '{maBooking}'.");

            if (dp.Trangthai != "Đã check-in")
                return (false, $"Chỉ có thể thêm dịch vụ phát sinh khi khách đang lưu trú ('Đã check-in'). Trạng thái hiện tại: {dp.Trangthai}.");

            var dichVu = await _unitOfWork.DichVuRepository.GetByIdAsync(idDichVu);
            if (dichVu is null)
                return (false, "Dịch vụ được chọn không tồn tại.");

            try
            {
                var ctDichVu = new HotelManagement.Models.Entities.ChiTietDichVu
                {
                    IdDatphong = dp.IdDatphong,
                    IdDichvu = idDichVu,
                    Soluong = soLuong,
                    Dongia = dichVu.Mucgia, // Đơn giá chốt tại thời điểm dùng
                    Thoigian = DateTime.Now
                };

                await _unitOfWork.ChiTietDichVuRepository.AddAsync(ctDichVu);
                await _unitOfWork.SaveChangesAsync();

                return (true, $"Đã thêm thành công {soLuong} x {dichVu.TenDichvu} vào booking {dp.Mabooking}.");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi khi thêm dịch vụ phát sinh: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> XoaDichVuPhatSinhAsync(int idChiTietDichVu)
        {
            var ct = await _unitOfWork.ChiTietDichVuRepository.GetByIdAsync(idChiTietDichVu);
            if (ct is null)
                return (false, "Không tìm thấy chi tiết dịch vụ cần xóa.");

            var datPhong = await _unitOfWork.DatPhongRepository.GetByIdAsync(ct.IdDatphong);
            if (datPhong != null && datPhong.Trangthai != "Đã check-in")
                return (false, "Không thể xóa dịch vụ khi booking không ở trạng thái 'Đã check-in'.");

            try
            {
                _unitOfWork.ChiTietDichVuRepository.Remove(ct);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Đã xóa dịch vụ thành công.");
            }
            catch (Exception ex)
            {
                return (false, "Lỗi khi xóa dịch vụ: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CheckOutAsync(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return (false, "Vui lòng nhập mã booking.");

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp is null)
                return (false, $"Không tìm thấy thông tin đặt phòng với mã booking '{maBooking}'.");

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong is null)
                return (false, "Không thể tải chi tiết đặt phòng.");

            // Kiểm tra trạng thái booking - chỉ cho phép khi "Đã check-in"
            if (datPhong.Trangthai == "Đã check-out")
            {
                string thoiGian = datPhong.NgaytraThucte.HasValue
                    ? datPhong.NgaytraThucte.Value.ToString("dd/MM/yyyy HH:mm")
                    : "trước đó";
                return (false, $"Mã booking này đã hoàn tất check-out vào lúc {thoiGian}.");
            }

            if (datPhong.Trangthai == "Đã xác nhận")
                return (false, "Booking chưa thực hiện check-in, không thể thực hiện check-out.");

            if (datPhong.Trangthai == "Đã hủy")
                return (false, "Booking này đã bị hủy, không thể check-out.");

            if (datPhong.Trangthai != "Đã check-in")
                return (false, $"Trạng thái booking ({datPhong.Trangthai}) không hợp lệ để check-out.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 1. Cập nhật trạng thái booking và ngày trả thực tế
                datPhong.Trangthai = "Đã check-out";
                datPhong.NgaytraThucte = DateTime.Now;
                _unitOfWork.DatPhongRepository.Update(datPhong);

                // 2. Cập nhật trạng thái các phòng liên quan sang "Đang dọn"
                foreach (var ct in datPhong.ChiTietDatPhongs)
                {
                    var phong = ct.IdPhongNavigation ?? await _unitOfWork.PhongRepository.GetByIdAsync(ct.IdPhong);
                    if (phong != null)
                    {
                        phong.Trangthai = "Đang dọn";
                        _unitOfWork.PhongRepository.Update(phong);
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                return (true, $"Check-out thành công cho mã booking {datPhong.Mabooking}! Các phòng đã được chuyển sang trạng thái 'Đang dọn'.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Có lỗi xảy ra khi thực hiện check-out: " + ex.Message);
            }
        }
    }
}
