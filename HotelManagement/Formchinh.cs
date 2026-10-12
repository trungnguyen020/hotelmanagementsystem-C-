using System.Diagnostics;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement
{
    public partial class Formchinh : Form
    {
        /// <summary>
        /// Báo cho FormDangNhap biết người dùng chủ động đăng xuất - để phân biệt với việc
        /// đóng ứng dụng bằng nút X (hai trường hợp cần xử lý khác nhau).
        /// </summary>
        public event EventHandler? YeuCauDangXuat;

        private readonly TaiKhoan _taiKhoanDangNhap;

        public Formchinh(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            InitializeComponent();
            KhoiTaoGiaoDien();
        }

        private void KhoiTaoGiaoDien()
        {
            // TODO: đối chiếu lại nếu sau này chuyển sang MDI - hiện đang là Form độc lập (non-MDI)
            bool laAdmin = _taiKhoanDangNhap.IdVaitroNavigation.TenVaitro == "Admin";
            mnuQuanLyNhanSu.Visible = laAdmin;
            mnuBaoCao.Visible = laAdmin;
            mnuDoiMatKhau.Visible = !laAdmin; // Không cho Admin đổi mật khẩu

            Text = $"Hệ thống quản lý khách sạn - Xin chào {_taiKhoanDangNhap.IdNhansuNavigation.Hoten} " +
                   $"({_taiKhoanDangNhap.IdVaitroNavigation.TenVaitro})";
        }

        private async void MnuDangXuat_Click(object? sender, EventArgs e)
        {
            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                await authService.DangXuatAsync(_taiKhoanDangNhap.IdTaikhoan);
            }
            catch (Exception ex)
            {
                // Lỗi ghi log không nên chặn người dùng đăng xuất - chỉ ghi nhận để debug sau.
                Debug.WriteLine(ex);
            }

            YeuCauDangXuat?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}