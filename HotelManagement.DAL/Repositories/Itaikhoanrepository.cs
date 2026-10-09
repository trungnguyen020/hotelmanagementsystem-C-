using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    public interface ITaiKhoanRepository : IRepository<TaiKhoan>
    {
        /// <summary>Tìm tài khoản theo tên đăng nhập (dùng để kiểm tra trùng khi tạo mới).</summary>
        Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap);

        /// <summary>
        /// Tìm tài khoản theo tên đăng nhập, kèm Include NhanSu + VaiTro -
        /// dùng trực tiếp cho luồng đăng nhập (IAuthService cần VaiTro để ẩn/hiện menu).
        /// </summary>
        Task<TaiKhoan?> GetByTenDangNhapKemVaiTroAsync(string tenDangNhap);
    }
}