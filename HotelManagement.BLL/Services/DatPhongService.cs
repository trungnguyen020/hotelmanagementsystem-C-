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

        public async Task<(bool Success, string Message, string? MaBooking)> DatPhongAsync(
            int idKhachHang, int idNhanSu, DateOnly ngayNhan, DateOnly ngayTra, int soNguoi, List<int> danhSachIdPhong)
        {
            if (danhSachIdPhong == null || !danhSachIdPhong.Any())
                return (false, "Vui lòng chọn ít nhất 1 phòng để đặt.", null);

            if (danhSachIdPhong.Distinct().Count() != danhSachIdPhong.Count)
                return (false, "Danh sách phòng đặt bị trùng, vui lòng kiểm tra lại.", null);

            if (idKhachHang <= 0)
                return (false, "Vui lòng chọn khách hàng trước khi đặt phòng.", null);

            if (soNguoi <= 0)
                return (false, "Số người phải lớn hơn 0.", null);

            if (ngayNhan >= ngayTra)
                return (false, "Ngày nhận phòng phải trước ngày trả phòng.", null);

            if (ngayNhan < DateOnly.FromDateTime(DateTime.Now))
                return (false, "Ngày nhận phòng không được trước thời điểm hiện tại.", null);

            // Khách đang lưu trú (đã check-in) chỉ được đặt phòng mới sau khi check-out thành công
            var bookingDangLuuTru = await _unitOfWork.DatPhongRepository
                .FindAsync(dp => dp.IdKhachhang == idKhachHang && dp.Trangthai == "Đã check-in");

            var bookingHienTai = bookingDangLuuTru.FirstOrDefault();
            if (bookingHienTai != null)
                return (false, $"Khách hàng đang lưu trú (booking {bookingHienTai.Mabooking}) và chưa check-out. Vui lòng check-out trước khi đặt phòng mới.", null);

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
                        return (false, $"Phòng {phong?.Sophong} đã được đặt trong khoảng thời gian này.", null);
                    }
                }

                // Tổng sức chứa của các phòng đã chọn phải đủ cho số khách
                int tongSucChua = 0;
                foreach (var idPhong in danhSachIdPhong)
                {
                    var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                    if (phong == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return (false, $"Không tìm thấy phòng có mã {idPhong}.", null);
                    }

                    var loaiPhong = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(phong.IdLoaiphong);
                    tongSucChua += loaiPhong?.Succhua ?? 0;
                }

                if (tongSucChua < soNguoi)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return (false, $"Tổng sức chứa của các phòng đã chọn ({tongSucChua} người) không đủ cho {soNguoi} khách.", null);
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
                
                return (true, $"Đặt phòng thành công! Mã booking: {maBooking}", maBooking);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Có lỗi xảy ra khi đặt phòng: " + ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, string? MaBooking)> KhachHangDatPhongAsync(
            int idKhachHang, DateOnly ngayNhan, DateOnly ngayTra, int soNguoi, List<int> danhSachIdPhong,
            List<(int IdDichVu, int SoLuong)>? danhSachDichVu = null)
        {
            if (danhSachIdPhong == null || !danhSachIdPhong.Any())
                return (false, "Vui lòng chọn ít nhất 1 phòng để đặt.", null);

            if (danhSachIdPhong.Distinct().Count() != danhSachIdPhong.Count)
                return (false, "Danh sách phòng đặt bị trùng, vui lòng kiểm tra lại.", null);

            if (idKhachHang <= 0)
                return (false, "Tài khoản khách hàng không hợp lệ.", null);

            if (soNguoi <= 0)
                return (false, "Số người phải lớn hơn 0.", null);

            if (ngayNhan >= ngayTra)
                return (false, "Ngày nhận phòng phải trước ngày trả phòng.", null);

            if (ngayNhan < DateOnly.FromDateTime(DateTime.Now))
                return (false, "Ngày nhận phòng không được trước thời điểm hiện tại.", null);

            // Kiểm tra xem khách có đang lưu trú không
            var bookingDangLuuTru = await _unitOfWork.DatPhongRepository
                .FindAsync(dp => dp.IdKhachhang == idKhachHang && dp.Trangthai == "Đã check-in");

            var bookingHienTai = bookingDangLuuTru.FirstOrDefault();
            if (bookingHienTai != null)
                return (false, $"Quý khách đang lưu trú tại khách sạn (đơn #{bookingHienTai.Mabooking}) và chưa hoàn tất check-out.", null);

            // Tìm một nhân sự hệ thống mặc định để gắn vào cột id_nhansu bắt buộc
            var dsNhanSu = await _unitOfWork.NhanSuRepository.GetAllAsync();
            var nhanSuMacDinh = dsNhanSu.FirstOrDefault();
            if (nhanSuMacDinh == null)
                return (false, "Hệ thống chưa thiết lập nhân sự tiếp nhận. Vui lòng liên hệ bộ phận hỗ trợ.", null);

            int idNhanSu = nhanSuMacDinh.IdNhansu;

            try
            {
                // DCKLTTQ-108: Sử dụng mức cô lập Serializable để triệt tiêu race condition khi nhiều khách cùng đặt
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                // Kiểm tra lại lần cuối trong transaction xem phòng còn trống không
                foreach (var idPhong in danhSachIdPhong)
                {
                    bool isTrong = await _unitOfWork.DatPhongRepository.IsPhongTrongAsync(idPhong, ngayNhan, ngayTra);
                    if (!isTrong)
                    {
                        var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                        await _unitOfWork.RollbackTransactionAsync();
                        return (false, $"Rất tiếc, phòng {phong?.Sophong} vừa được đặt bởi người khác hoặc không còn trống trong khoảng thời gian này. Vui lòng chọn phòng khác.", null);
                    }
                }

                // Kiểm tra tổng sức chứa
                int tongSucChua = 0;
                foreach (var idPhong in danhSachIdPhong)
                {
                    var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                    if (phong == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return (false, $"Không tìm thấy thông tin phòng {idPhong}.", null);
                    }

                    var loaiPhong = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(phong.IdLoaiphong);
                    tongSucChua += loaiPhong?.Succhua ?? 0;
                }

                if (tongSucChua < soNguoi)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return (false, $"Tổng sức chứa các phòng đã chọn ({tongSucChua} người) không đủ cho {soNguoi} khách.", null);
                }

                // Sinh mã booking theo đúng chuẩn đã có ở Epic 3
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
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.DatPhongRepository.AddAsync(datPhong);
                await _unitOfWork.SaveChangesAsync();

                // Thêm chi tiết đặt phòng
                foreach (var idPhong in danhSachIdPhong)
                {
                    var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
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

                    datPhong.ChiTietDatPhongs.Add(ctDatPhong);
                }

                // Thêm các dịch vụ kèm theo mà khách hàng đã chọn (nếu có)
                if (danhSachDichVu != null && danhSachDichVu.Any())
                {
                    foreach (var dvChon in danhSachDichVu)
                    {
                        if (dvChon.IdDichVu <= 0 || dvChon.SoLuong <= 0) continue;
                        var dvEntity = await _unitOfWork.DichVuRepository.GetByIdAsync(dvChon.IdDichVu);
                        if (dvEntity != null)
                        {
                            var ctDichVu = new HotelManagement.Models.Entities.ChiTietDichVu
                            {
                                IdDatphong = datPhong.IdDatphong,
                                IdDichvu = dvChon.IdDichVu,
                                Soluong = dvChon.SoLuong,
                                Dongia = dvEntity.Mucgia,
                                Thoigian = DateTime.Now,
                                CreatedAt = DateTime.Now
                            };
                            await _unitOfWork.ChiTietDichVuRepository.AddAsync(ctDichVu);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return (true, $"Đặt phòng thành công! Mã booking của quý khách là: {maBooking}", maBooking);
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 1205) // Deadlock / Lock contention
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Rất tiếc, phòng vừa được đặt bởi người khác trong lúc xử lý. Quý khách vui lòng chọn phòng hoặc thời gian khác.", null);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException innerSql && innerSql.Number == 1205)
                {
                    return (false, "Rất tiếc, phòng vừa được đặt bởi người khác trong lúc xử lý. Quý khách vui lòng chọn phòng hoặc thời gian khác.", null);
                }
                return (false, "Không thể hoàn tất đặt phòng do sự cố đường truyền: " + ex.Message, null);
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

        public async Task<(bool Success, string Message)> HuyDatPhongAsync(string maBooking, string? lyDoHuy = null)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                return (false, "Mã booking không được để trống.");

            var dp = await _unitOfWork.DatPhongRepository.GetByMaBookingAsync(maBooking.Trim());
            if (dp is null)
                return (false, $"Không tìm thấy đơn đặt phòng với mã booking '{maBooking}'.");

            var datPhong = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
            if (datPhong is null)
                return (false, $"Không thể lấy chi tiết đặt phòng cho mã booking '{maBooking}'.");

            if (datPhong.Trangthai == "Đã hủy")
                return (false, "Đơn đặt phòng này đã bị hủy trước đó.");

            if (datPhong.Trangthai == "Đã check-in")
                return (false, "Khách hàng đã nhận phòng (Đã check-in), không thể thực hiện hủy phòng. Vui lòng thực hiện quy trình Check-out.");

            if (datPhong.Trangthai == "Đã check-out")
                return (false, "Đơn đặt phòng này đã hoàn tất check-out, không thể hủy.");

            if (datPhong.Trangthai != "Đã xác nhận")
                return (false, $"Trạng thái booking ({datPhong.Trangthai}) không hợp lệ để hủy đặt phòng.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Cập nhật trạng thái đặt phòng sang 'Đã hủy'
                datPhong.Trangthai = "Đã hủy";
                datPhong.UpdatedAt = DateTime.Now;
                _unitOfWork.DatPhongRepository.Update(datPhong);

                await _unitOfWork.CommitTransactionAsync();
                return (true, $"Đã hủy thành công đơn đặt phòng {datPhong.Mabooking}!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Có lỗi xảy ra khi thực hiện hủy đặt phòng: " + ex.Message);
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

        public async Task<string?> LayMaBookingHienTaiCuaPhongAsync(int idPhong)
        {
            try
            {
                var danhSachDangO = await _unitOfWork.DatPhongRepository.FindAsync(dp => dp.Trangthai == "Đã check-in");
                foreach (var dp in danhSachDangO)
                {
                    var dayDu = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
                    if (dayDu != null && dayDu.ChiTietDatPhongs.Any(ct => ct.IdPhong == idPhong))
                    {
                        return dayDu.Mabooking;
                    }
                }
            }
            catch
            {
                // Fallback nếu có lỗi xảy ra
            }
            return null;
        }

        public async Task<string?> LayMaBookingChoCheckInCuaPhongAsync(int idPhong)
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var danhSachCho = await _unitOfWork.DatPhongRepository.FindAsync(dp =>
                    dp.Trangthai == "Đã xác nhận" && dp.NgaynhanDukien <= today && today < dp.NgaytraDukien);
                foreach (var dp in danhSachCho)
                {
                    var dayDu = await _unitOfWork.DatPhongRepository.GetChiTietDayDuAsync(dp.IdDatphong);
                    if (dayDu != null && dayDu.ChiTietDatPhongs.Any(ct => ct.IdPhong == idPhong))
                    {
                        return dayDu.Mabooking;
                    }
                }
            }
            catch
            {
                // Fallback nếu có lỗi xảy ra
            }
            return null;
        }

        public async Task<List<BookingTimKiemDto>> TimKiemBookingAsync(
            string? tenKhachHang = null,
            string? sdt = null,
            string? cccd = null,
            string? soPhong = null,
            string? trangThai = null)
        {
            var danhSach = await _unitOfWork.DatPhongRepository.TimKiemBookingAsync(
                tenKhachHang, sdt, cccd, soPhong, trangThai);

            return danhSach.Select(dp => new BookingTimKiemDto
            {
                IdDatPhong = dp.IdDatphong,
                MaBooking = dp.Mabooking,
                TrangThai = dp.Trangthai,
                TenKhachHang = dp.IdKhachhangNavigation?.Ten ?? string.Empty,
                SdtKhachHang = dp.IdKhachhangNavigation?.Sdt ?? string.Empty,
                CccdKhachHang = dp.IdKhachhangNavigation?.Cccd ?? string.Empty,
                NgayDat = dp.Ngaydat,
                NgayNhanDuKien = dp.NgaynhanDukien,
                NgayTraDuKien = dp.NgaytraDukien,
                DanhSachSoPhong = dp.ChiTietDatPhongs != null
                    ? string.Join(", ", dp.ChiTietDatPhongs
                        .Where(ct => ct.IdPhongNavigation != null)
                        .Select(ct => ct.IdPhongNavigation.Sophong))
                    : string.Empty,
                SoPhong = dp.ChiTietDatPhongs?.Count ?? 0,
                SoNguoi = dp.Songuoi
            }).ToList();
        }

        public async Task<List<DatPhongLichSuDto>> LayLichSuKhachHangAsync(int idKhachHang, string? trangThai = null)
        {
            var ds = await _unitOfWork.DatPhongRepository.GetLichSuKhachHangAsync(idKhachHang, trangThai);

            return ds.Select(dp =>
            {
                int soDem = Math.Max(1, dp.NgaytraDukien.DayNumber - dp.NgaynhanDukien.DayNumber);
                decimal tienPhong = dp.ChiTietDatPhongs.Sum(ct => ct.Dongia) * soDem;
                decimal tienDichVu = dp.ChiTietDichVus.Sum(dv => dv.Dongia);
                decimal tongTien = dp.HoaDon?.Tonggia ?? (tienPhong + tienDichVu);

                string dsPhong = dp.ChiTietDatPhongs.Any()
                    ? string.Join(", ", dp.ChiTietDatPhongs.Select(ct =>
                        $"P.{ct.IdPhongNavigation?.Sophong ?? ct.IdPhong.ToString()} ({(ct.IdPhongNavigation?.IdLoaiphongNavigation?.Ten ?? "Tiêu chuẩn")})"))
                    : "Chưa phân phòng";

                return new DatPhongLichSuDto
                {
                    IdDatPhong = dp.IdDatphong,
                    MaBooking = dp.Mabooking,
                    NgayDat = dp.Ngaydat,
                    NgayNhan = dp.NgaynhanDukien,
                    NgayTra = dp.NgaytraDukien,
                    SoNguoi = dp.Songuoi,
                    DanhSachPhong = dsPhong,
                    TrangThaiBooking = dp.Trangthai,
                    CoHoaDon = dp.HoaDon != null,
                    TrangThaiThanhToan = dp.HoaDon?.TrangthaiThanhtoan ?? (dp.Trangthai == "Đã check-out" ? "Đã thanh toán" : "Chưa thanh toán"),
                    TongTien = tongTien
                };
            }).ToList();
        }
    }
}
