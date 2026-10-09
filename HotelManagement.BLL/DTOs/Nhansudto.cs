namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO cho Nhân sự (DCKLTTQ-68).
    /// Mapping đúng với entity NhanSu: IdNhansu, Hoten, Ngaysinh, Cccd, Sdt.
    /// Quyết định thay đổi so với v4: cho phép tạo tài khoản lễ tân ngay khi thêm nhân sự
    /// (checkbox trên Form) — tài khoản sẽ có VaiTro = "Lễ tân", mật khẩu mặc định.
    /// </summary>
    public class NhanSuDto
    {
        public int IdNhanSu { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Cccd { get; set; } = string.Empty;
        public string Sdt { get; set; } = string.Empty;
        public DateOnly NgaySinh { get; set; }

        // --- Thông tin tài khoản (chỉ dùng khi tạo mới/hiển thị) ---

        /// <summary>Nếu true, sẽ tạo TaiKhoan "Lễ tân" luôn khi thêm nhân sự.</summary>
        public bool TaoTaiKhoanLeTan { get; set; }

        /// <summary>Tên đăng nhập muốn tạo (bắt buộc nếu TaoTaiKhoanLeTan = true).</summary>
        public string? TenDangNhap { get; set; }

        /// <summary>Mật khẩu (plain text) — Service sẽ hash trước khi lưu.</summary>
        public string? MatKhau { get; set; }

        /// <summary>Chỉ để hiển thị tên tài khoản đã gán (nếu có), không sửa qua Form CRUD này.</summary>
        public string? TenTaiKhoanHienThi { get; set; }

        /// <summary>Trạng thái tài khoản hiện tại (nếu có).</summary>
        public string? TrangThaiTaiKhoan { get; set; }
    }
}