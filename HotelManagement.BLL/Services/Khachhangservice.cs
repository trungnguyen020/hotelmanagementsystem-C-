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
            return list.Select(MapToDto).OrderBy(x => x.Ten).ToList();
        }

        public async Task<KhachHangDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.KhachHangRepository.GetByIdAsync(id);
            return entity is null ? null : MapToDto(entity);
        }

        public async Task<List<KhachHangDto>> TimKiemAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await GetAllAsync();

            var ketQua = await _unitOfWork.KhachHangRepository.FindAsync(
                kh => kh.Ten.Contains(tuKhoa) || kh.Cccd.Contains(tuKhoa) || kh.Sdt.Contains(tuKhoa));

            return ketQua.Select(MapToDto).OrderBy(x => x.Ten).ToList();
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

        private static KhachHangDto MapToDto(KhachHang entity) => new()
        {
            IdKhachHang = entity.IdKhachhang,
            Ten = entity.Ten,
            Cccd = entity.Cccd,
            Email = entity.Email,
            Sdt = entity.Sdt,
            GioiTinh = entity.Gioitinh,
            NgaySinh = entity.Ngaysinh
        };
    }
}