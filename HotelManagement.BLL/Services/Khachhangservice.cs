using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Nghiệp vụ CRUD Khách hàng (DCKLTTQ-67), dùng IUnitOfWork/IRepository sẵn có.
    /// Validate: Ten/Cccd/Email bắt buộc, CCCD 12 chữ số UNIQUE, email đúng định dạng.
    /// </summary>
    public class KhachHangService : IKhachHangService
    {
        private readonly IUnitOfWork _unitOfWork;

        public KhachHangService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<KhachHangDto>> GetAllAsync()
        {
            var list = await _unitOfWork.KhachHangRepository.GetAllAsync();
            var dsTaiKhoan = await _unitOfWork.TaiKhoanRepository.GetAllAsync();
            var dictTaiKhoan = dsTaiKhoan.Where(t => t.IdKhachhang.HasValue).ToDictionary(t => t.IdKhachhang!.Value);

            return list.Select(kh => MapToDto(kh, dictTaiKhoan.GetValueOrDefault(kh.IdKhachhang))).OrderBy(x => x.Ten).ToList();
        }

        public async Task<KhachHangDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(id);
            if (entity is null) return null;

            var dsTaiKhoan = await _unitOfWork.TaiKhoanRepository.FindAsync(t => t.IdKhachhang == id);
            return MapToDto(entity, dsTaiKhoan.FirstOrDefault());
        }

        public async Task<List<KhachHangDto>> TimKiemAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await GetAllAsync();

            var ketQua = await _unitOfWork.KhachHangRepository.FindAsync(
                kh => kh.Ten.Contains(tuKhoa) || kh.Cccd.Contains(tuKhoa) || kh.Sdt.Contains(tuKhoa));

            var dsTaiKhoan = await _unitOfWork.TaiKhoanRepository.GetAllAsync();
            var dictTaiKhoan = dsTaiKhoan.Where(t => t.IdKhachhang.HasValue).ToDictionary(t => t.IdKhachhang!.Value);

            return ketQua.Select(kh => MapToDto(kh, dictTaiKhoan.GetValueOrDefault(kh.IdKhachhang))).OrderBy(x => x.Ten).ToList();
        }

        public async Task<(bool Success, string Message)> AddAsync(KhachHangDto dto)
        {
            var (isValid, message) = await ValidateAsync(dto, isAdd: true);
            if (!isValid)
                return (false, message);

            var entity = new KhachHang
            {
                Ten = dto.Ten.Trim(),
                Cccd = dto.Cccd.Trim(),
                Email = dto.Email.Trim(),
                Sdt = dto.Sdt.Trim(),
                Gioitinh = dto.GioiTinh?.Trim(),
                Ngaysinh = dto.NgaySinh
            };

            await _unitOfWork.KhachHangRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Thêm khách hàng thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(KhachHangDto dto)
        {
            var (isValid, message) = await ValidateAsync(dto, isAdd: false);
            if (!isValid)
                return (false, message);

            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(dto.IdKhachHang);
            if (entity is null)
                return (false, "Không tìm thấy khách hàng cần cập nhật.");

            entity.Ten = dto.Ten.Trim();
            entity.Cccd = dto.Cccd.Trim();
            entity.Email = dto.Email.Trim();
            entity.Sdt = dto.Sdt.Trim();
            entity.Gioitinh = dto.GioiTinh?.Trim();
            entity.Ngaysinh = dto.NgaySinh;

            _unitOfWork.KhachHangRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Cập nhật khách hàng thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(id);
            if (entity is null)
                return (false, "Không tìm thấy khách hàng cần xóa.");

            // Chặn xóa nếu khách hàng có đặt phòng liên quan
            if (entity.DatPhongs.Any())
                return (false, "Không thể xóa: khách hàng còn đơn đặt phòng. Hãy xóa đơn đặt phòng trước.");

            _unitOfWork.KhachHangRepository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Xóa khách hàng thành công.");
        }

        private async Task<(bool IsValid, string Message)> ValidateAsync(KhachHangDto dto, bool isAdd)
        {
            if (string.IsNullOrWhiteSpace(dto.Ten))
                return (false, "Tên khách hàng không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Cccd))
                return (false, "CCCD không được để trống.");

            if (!Regex.IsMatch(dto.Cccd.Trim(), @"^\d{12}$"))
                return (false, "CCCD phải gồm đúng 12 chữ số.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return (false, "Email là bắt buộc.");

            if (!Regex.IsMatch(dto.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Email không đúng định dạng.");

            if (string.IsNullOrWhiteSpace(dto.Sdt))
                return (false, "Số điện thoại không được để trống.");

            // Kiểm tra CCCD unique
            var all = await _unitOfWork.KhachHangRepository.GetAllAsync();
            bool trungCccd = all.Any(kh =>
                kh.Cccd.Trim().Equals(dto.Cccd.Trim(), StringComparison.OrdinalIgnoreCase)
                && (isAdd || kh.IdKhachhang != dto.IdKhachHang));

            if (trungCccd)
                return (false, "CCCD đã tồn tại, vui lòng kiểm tra lại.");

            return (true, string.Empty);
        }

        public async Task<(bool Success, string Message)> ResetMatKhauKhachHangAsync(int idKhachHang, string matKhauMoi = "123456")
        {
            var dsTaiKhoan = await _unitOfWork.TaiKhoanRepository.FindAsync(t => t.IdKhachhang == idKhachHang);
            var taiKhoan = dsTaiKhoan.FirstOrDefault();
            if (taiKhoan == null)
                return (false, "Khách hàng này chưa có tài khoản hệ thống.");

            taiKhoan.MatkhauHash = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);
            taiKhoan.SolanSaimatkhau = 0;
            taiKhoan.Trangthai = "Hoạt động";

            _unitOfWork.TaiKhoanRepository.Update(taiKhoan);
            await _unitOfWork.SaveChangesAsync();

            return (true, $"Đặt lại mật khẩu thành công! Mật khẩu mới là '{matKhauMoi}', tài khoản đã được mở khóa.");
        }

        public async Task<(bool Success, string Message)> ChuyenTrangThaiTaiKhoanAsync(int idKhachHang)
        {
            var dsTaiKhoan = await _unitOfWork.TaiKhoanRepository.FindAsync(t => t.IdKhachhang == idKhachHang);
            var taiKhoan = dsTaiKhoan.FirstOrDefault();
            if (taiKhoan == null)
                return (false, "Khách hàng này chưa có tài khoản hệ thống.");

            if (taiKhoan.Trangthai == "Khóa")
            {
                taiKhoan.Trangthai = "Hoạt động";
                taiKhoan.SolanSaimatkhau = 0;
            }
            else
            {
                taiKhoan.Trangthai = "Khóa";
            }

            _unitOfWork.TaiKhoanRepository.Update(taiKhoan);
            await _unitOfWork.SaveChangesAsync();

            return (true, $"Đã cập nhật trạng thái tài khoản sang '{taiKhoan.Trangthai}'.");
        }

        public async Task<(bool Success, string Message, KhachHangDto? UpdatedDto)> CapNhatThongTinCaNhanAsync(
            int idKhachHang, string hoTen, string sdt, string email, string? gioiTinh, DateOnly? ngaySinh)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
                return (false, "Họ và tên không được để trống.", null);

            if (string.IsNullOrWhiteSpace(sdt))
                return (false, "Số điện thoại không được để trống.", null);

            string sdtChuan = sdt.Trim();
            if (!Regex.IsMatch(sdtChuan, @"^(0|\+84)\d{9,10}$|^\d{10}$"))
                return (false, "Số điện thoại không đúng định dạng.", null);

            if (string.IsNullOrWhiteSpace(email))
                return (false, "Địa chỉ email không được để trống.", null);

            string emailChuan = email.Trim();
            if (!Regex.IsMatch(emailChuan, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Địa chỉ email không đúng định dạng.", null);

            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(idKhachHang);
            if (entity is null)
                return (false, "Không tìm thấy hồ sơ khách hàng cần cập nhật.", null);

            // Kiểm tra trùng lặp email với khách hàng khác (DCKLTTQ-110)
            var dsKhachHangCungEmail = await _unitOfWork.KhachHangRepository.FindAsync(
                k => k.Email.ToLower() == emailChuan.ToLower() && k.IdKhachhang != idKhachHang);
            if (dsKhachHangCungEmail.Any())
            {
                return (false, "Địa chỉ email này đã được sử dụng bởi một khách hàng khác. Vui lòng chọn email khác.", null);
            }

            // Cập nhật thông tin, giữ nguyên số CCCD
            entity.Ten = hoTen.Trim();
            entity.Sdt = sdtChuan;
            entity.Email = emailChuan;
            entity.Gioitinh = string.IsNullOrWhiteSpace(gioiTinh) ? null : gioiTinh.Trim();
            entity.Ngaysinh = ngaySinh;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.KhachHangRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var taiKhoan = (await _unitOfWork.TaiKhoanRepository.FindAsync(tk => tk.IdKhachhang == idKhachHang)).FirstOrDefault();
            return (true, "Cập nhật thông tin cá nhân thành công!", MapToDto(entity, taiKhoan));
        }

        private static KhachHangDto MapToDto(KhachHang entity, TaiKhoan? taiKhoan = null) => new()
        {
            IdKhachHang = entity.IdKhachhang,
            Ten = entity.Ten,
            Cccd = entity.Cccd,
            Email = entity.Email,
            Sdt = entity.Sdt,
            GioiTinh = entity.Gioitinh,
            NgaySinh = entity.Ngaysinh,
            TenTaiKhoan = taiKhoan?.TenDangnhap,
            TrangThaiTaiKhoan = taiKhoan?.Trangthai,
            SoLanSaiMatKhau = taiKhoan?.SolanSaimatkhau ?? 0
        };
    }
}