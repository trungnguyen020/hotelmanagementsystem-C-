using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface IKhachHangService
    {
        Task<List<KhachHangDto>> GetAllAsync();
        Task<KhachHangDto?> GetByIdAsync(int id);
        Task<List<KhachHangDto>> TimKiemAsync(string tuKhoa);
        Task<(bool Success, string Message)> AddAsync(KhachHangDto dto);
        Task<(bool Success, string Message)> UpdateAsync(KhachHangDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);

        /// <summary>
        /// Admin quản trị tài khoản khách hàng: Đặt lại mật khẩu mặc định (ví dụ: "123456") và mở khóa tài khoản.
        /// </summary>
        Task<(bool Success, string Message)> ResetMatKhauKhachHangAsync(int idKhachHang, string matKhauMoi = "123456");

        /// <summary>
        /// Admin quản trị tài khoản khách hàng: Khóa hoặc Mở khóa tài khoản đăng nhập của khách hàng.
        /// </summary>
        Task<(bool Success, string Message)> ChuyenTrangThaiTaiKhoanAsync(int idKhachHang);

        /// <summary>
        /// Khách hàng tự cập nhật thông tin cá nhân (DCKLTTQ-110).
        /// Giữ nguyên số CCCD định danh gốc, kiểm tra định dạng và trùng lặp email với khách hàng khác.
        /// </summary>
        Task<(bool Success, string Message, KhachHangDto? UpdatedDto)> CapNhatThongTinCaNhanAsync(
            int idKhachHang, string hoTen, string sdt, string email, string? gioiTinh, DateOnly? ngaySinh);
    }
}