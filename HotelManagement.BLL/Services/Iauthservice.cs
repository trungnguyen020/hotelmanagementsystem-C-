using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    /// <summary>Lý do/kết quả của 1 lần thử đăng nhập - Form đăng nhập dựa vào đây để hiển thị thông báo phù hợp.</summary>
    public enum TrangThaiDangNhap
    {
        ThanhCong,
        SaiTenDangNhapHoacMatKhau,
        TaiKhoanBiKhoa
    }

    /// <summary>Kết quả trả về cho Form đăng nhập sau khi gọi IAuthService.DangNhapAsync.</summary>
    public class KetQuaDangNhap
    {
        public TrangThaiDangNhap TrangThai { get; set; }

        /// <summary>Chỉ có giá trị khi TrangThai == ThanhCong. Đã Include sẵn NhanSu + VaiTro để Form dùng ngay
        /// cho việc ẩn/hiện menu theo vai trò, không cần query lại.</summary>
        public TaiKhoan? TaiKhoan { get; set; }

        public string ThongBao { get; set; } = string.Empty;
    }

    public interface IAuthService
    {
        /// <summary>
        /// Xác thực tên đăng nhập/mật khẩu. Tự động tăng SolanSaimatkhau khi sai và khóa tài khoản
        /// khi đạt ngưỡng tối đa; tự reset về 0 khi đăng nhập thành công.
        /// </summary>
        Task<KetQuaDangNhap> DangNhapAsync(string tenDangNhap, string matKhau);

        /// <summary>Băm mật khẩu bằng BCrypt - dùng khi Admin tạo tài khoản mới hoặc đổi mật khẩu.</summary>
        string HashMatKhau(string matKhauTho);

        /// <summary>Ghi nhật ký "Đăng xuất" cho tài khoản - gọi khi Form chính xử lý sự kiện đăng xuất.</summary>
        Task DangXuatAsync(int idTaiKhoan);

        /// <summary>Người dùng tự đổi mật khẩu của mình.</summary>
        Task<(bool Success, string Message)> DoiMatKhauAsync(int idTaiKhoan, string matKhauCu, string matKhauMoi);
    }
}