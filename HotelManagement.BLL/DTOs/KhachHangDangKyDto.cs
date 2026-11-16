namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO đăng ký tài khoản khách hàng mới (DCKLTTQ-106).
    /// Gồm 2 nhóm dữ liệu: KhachHang (Họ tên, CCCD, SĐT, Email) + TaiKhoan (Tên đăng nhập, Mật khẩu, Xác nhận mật khẩu).
    /// </summary>
    public class KhachHangDangKyDto
    {
        // Thông tin Khách hàng
        public string HoTen { get; set; } = string.Empty;
        public string Cccd { get; set; } = string.Empty;
        public string Sdt { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Thông tin Tài khoản
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }
}

