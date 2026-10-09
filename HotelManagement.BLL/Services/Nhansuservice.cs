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
    /// Nghiệp vụ CRUD Nhân sự (DCKLTTQ-68), dùng IUnitOfWork/IRepository sẵn có.
    /// Validate: Hoten/Cccd/Sdt bắt buộc, CCCD 12 chữ số UNIQUE.
    /// Tính năng bổ sung: khi thêm nhân sự, có thể tạo luôn TaiKhoan "Lễ tân" —
    /// dùng IAuthService.HashMatKhau để hash mật khẩu, ITaiKhoanRepository để kiểm tra trùng
    /// tên đăng nhập, UnitOfWork transaction để đảm bảo atomic (NhanSu + TaiKhoan cùng commit).
    /// </summary>
    public class NhanSuService : INhanSuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        /// <summary>Tên vai trò "Lễ tân" đúng giá trị trong bảng VaiTro.</summary>
        private const string VaiTroLeTan = "LeTan";
        private const string TrangThaiHoatDong = "Hoạt động";

        public NhanSuService(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<List<NhanSuDto>> GetAllAsync()
        {
            var list = await _unitOfWork.NhanSuRepository.GetAllAsync();
            var dtos = new List<NhanSuDto>();

            foreach (var ns in list.OrderBy(x => x.Hoten))
            {
                var dto = MapToDto(ns);
                // Tìm tài khoản liên kết (nếu có) — NhanSu → TaiKhoan là 1-1 optional
                var taiKhoans = await _unitOfWork.TaiKhoanRepository.FindAsync(
                    tk => tk.IdNhansu == ns.IdNhansu);
                var tk = taiKhoans.FirstOrDefault();
                if (tk != null)
                {
                    dto.TenTaiKhoanHienThi = tk.TenDangnhap;
                    dto.TrangThaiTaiKhoan = tk.Trangthai;
                }
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<NhanSuDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.NhanSuRepository.GetByIdAsync(id);
            if (entity is null) return null;

            var dto = MapToDto(entity);
            var taiKhoans = await _unitOfWork.TaiKhoanRepository.FindAsync(
                tk => tk.IdNhansu == entity.IdNhansu);
            var tk = taiKhoans.FirstOrDefault();
            if (tk != null)
            {
                dto.TenTaiKhoanHienThi = tk.TenDangnhap;
                dto.TrangThaiTaiKhoan = tk.Trangthai;
            }
            return dto;
        }

        public async Task<List<NhanSuDto>> TimKiemAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await GetAllAsync();

            var ketQua = await _unitOfWork.NhanSuRepository.FindAsync(
                ns => ns.Hoten.Contains(tuKhoa) || ns.Cccd.Contains(tuKhoa) || ns.Sdt.Contains(tuKhoa));

            var dtos = new List<NhanSuDto>();
            foreach (var ns in ketQua.OrderBy(x => x.Hoten))
            {
                var dto = MapToDto(ns);
                var taiKhoans = await _unitOfWork.TaiKhoanRepository.FindAsync(
                    tk => tk.IdNhansu == ns.IdNhansu);
                var tk = taiKhoans.FirstOrDefault();
                if (tk != null)
                {
                    dto.TenTaiKhoanHienThi = tk.TenDangnhap;
                    dto.TrangThaiTaiKhoan = tk.Trangthai;
                }
                dtos.Add(dto);
            }
            return dtos;
        }

        public async Task<(bool Success, string Message)> AddAsync(NhanSuDto dto)
        {
            var (isValid, message) = await ValidateAsync(dto, isAdd: true);
            if (!isValid)
                return (false, message);

            // Validate tài khoản nếu muốn tạo
            if (dto.TaoTaiKhoanLeTan)
            {
                var (tkValid, tkMsg) = await ValidateTaiKhoanAsync(dto);
                if (!tkValid)
                    return (false, tkMsg);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var entity = new NhanSu
                {
                    Hoten = dto.HoTen.Trim(),
                    Cccd = dto.Cccd.Trim(),
                    Sdt = dto.Sdt.Trim(),
                    Ngaysinh = dto.NgaySinh
                };

                await _unitOfWork.NhanSuRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Tạo tài khoản lễ tân nếu được yêu cầu
                if (dto.TaoTaiKhoanLeTan)
                {
                    // Tìm IdVaitro của "Lễ tân"
                    int idVaiTroLeTan = await TimIdVaiTroAsync(VaiTroLeTan);

                    var taiKhoan = new TaiKhoan
                    {
                        TenDangnhap = dto.TenDangNhap!.Trim(),
                        MatkhauHash = _authService.HashMatKhau(dto.MatKhau!),
                        IdNhansu = entity.IdNhansu,
                        IdVaitro = idVaiTroLeTan,
                        Trangthai = TrangThaiHoatDong,
                        SolanSaimatkhau = 0
                    };

                    await _unitOfWork.TaiKhoanRepository.AddAsync(taiKhoan);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();
                return (true, dto.TaoTaiKhoanLeTan
                    ? "Thêm nhân sự và tạo tài khoản lễ tân thành công."
                    : "Thêm nhân sự thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, $"Lỗi khi thêm nhân sự: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(NhanSuDto dto)
        {
            var (isValid, message) = await ValidateAsync(dto, isAdd: false);
            if (!isValid)
                return (false, message);

            var entity = await _unitOfWork.NhanSuRepository.GetByIdAsync(dto.IdNhanSu);
            if (entity is null)
                return (false, "Không tìm thấy nhân sự cần cập nhật.");

            entity.Hoten = dto.HoTen.Trim();
            entity.Cccd = dto.Cccd.Trim();
            entity.Sdt = dto.Sdt.Trim();
            entity.Ngaysinh = dto.NgaySinh;

            _unitOfWork.NhanSuRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Cập nhật nhân sự thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.NhanSuRepository.GetByIdAsync(id);
            if (entity is null)
                return (false, "Không tìm thấy nhân sự cần xóa.");

            // Chặn xóa nếu nhân sự có đặt phòng liên quan
            if (entity.DatPhongs.Any())
                return (false, "Không thể xóa: nhân sự còn đơn đặt phòng liên quan.");

            // Chặn xóa nếu nhân sự đang có tài khoản — phải xóa tài khoản trước
            var taiKhoans = await _unitOfWork.TaiKhoanRepository.FindAsync(
                tk => tk.IdNhansu == id);
            if (taiKhoans.Any())
                return (false, "Không thể xóa: nhân sự đang có tài khoản đăng nhập. Hãy xóa tài khoản trước.");

            _unitOfWork.NhanSuRepository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return (true, "Xóa nhân sự thành công.");
        }

        public async Task<(bool Success, string Message)> DatLaiMatKhauTaiKhoanAsync(int idNhanSu, string matKhauMoi)
        {
            var taiKhoans = await _unitOfWork.TaiKhoanRepository.FindAsync(tk => tk.IdNhansu == idNhanSu);
            var tk = taiKhoans.FirstOrDefault();
            if (tk == null) return (false, "Nhân sự này chưa có tài khoản.");

            tk.MatkhauHash = _authService.HashMatKhau(matKhauMoi);
            
            // Tự động mở khóa tài khoản nếu đang bị khóa và reset số lần sai
            tk.Trangthai = TrangThaiHoatDong;
            tk.SolanSaimatkhau = 0;

            // BẮT BUỘC CÓ vì FindAsync trả về AsNoTracking
            _unitOfWork.TaiKhoanRepository.Update(tk); 
            await _unitOfWork.SaveChangesAsync();
            return (true, "Đặt lại mật khẩu thành công và đã mở khóa tài khoản.");
        }

        private async Task<(bool IsValid, string Message)> ValidateAsync(NhanSuDto dto, bool isAdd)
        {
            if (string.IsNullOrWhiteSpace(dto.HoTen))
                return (false, "Họ tên không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Cccd))
                return (false, "CCCD không được để trống.");

            if (!Regex.IsMatch(dto.Cccd.Trim(), @"^\d{12}$"))
                return (false, "CCCD phải gồm đúng 12 chữ số.");

            if (string.IsNullOrWhiteSpace(dto.Sdt))
                return (false, "Số điện thoại không được để trống.");

            // CCCD unique
            var all = await _unitOfWork.NhanSuRepository.GetAllAsync();
            bool trungCccd = all.Any(ns =>
                ns.Cccd.Trim().Equals(dto.Cccd.Trim(), StringComparison.OrdinalIgnoreCase)
                && (isAdd || ns.IdNhansu != dto.IdNhanSu));

            if (trungCccd)
                return (false, "CCCD đã tồn tại, vui lòng kiểm tra lại.");

            return (true, string.Empty);
        }

        private async Task<(bool IsValid, string Message)> ValidateTaiKhoanAsync(NhanSuDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenDangNhap))
                return (false, "Tên đăng nhập không được để trống khi tạo tài khoản.");

            if (string.IsNullOrWhiteSpace(dto.MatKhau))
                return (false, "Mật khẩu không được để trống khi tạo tài khoản.");

            if (dto.MatKhau.Length < 6)
                return (false, "Mật khẩu phải có ít nhất 6 ký tự.");

            // Kiểm tra tên đăng nhập trùng
            var trung = await _unitOfWork.TaiKhoanRepository.GetByTenDangNhapAsync(dto.TenDangNhap.Trim());
            if (trung != null)
                return (false, "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");

            return (true, string.Empty);
        }

        private async Task<int> TimIdVaiTroAsync(string tenVaiTro)
        {
            // Tìm VaiTro theo tên — dùng generic repository
            var danhSachVaiTro = await _unitOfWork.VaiTroRepository.GetAllAsync();
            var vaiTro = danhSachVaiTro.FirstOrDefault(
                vt => vt.TenVaitro.Equals(tenVaiTro, StringComparison.OrdinalIgnoreCase));

            if (vaiTro is null)
                throw new InvalidOperationException(
                    $"Không tìm thấy vai trò \"{tenVaiTro}\" trong CSDL. " +
                    "Vui lòng kiểm tra bảng VaiTro.");

            return vaiTro.IdVaitro;
        }

        private static NhanSuDto MapToDto(NhanSu entity) => new()
        {
            IdNhanSu = entity.IdNhansu,
            HoTen = entity.Hoten,
            Cccd = entity.Cccd,
            Sdt = entity.Sdt,
            NgaySinh = entity.Ngaysinh
        };
    }
}
