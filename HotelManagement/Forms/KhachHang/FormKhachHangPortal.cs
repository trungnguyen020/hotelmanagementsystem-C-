using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement.Forms
{
    public partial class FormKhachHangPortal : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;

        /// <summary>
        /// Sự kiện phát ra khi khách hàng chọn Đăng xuất, báo cho FormDangNhap hiển thị lại
        /// tương tự pattern YeuCauDangXuat của FormChinh.
        /// </summary>
        public event EventHandler? YeuCauDangXuat;

        public FormKhachHangPortal(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap ?? throw new ArgumentNullException(nameof(taiKhoanDangNhap));
            InitializeComponent();
            ChuyenTab(0); // Mặc định mở Tab Đặt phòng
        }

        private async void BtnDangXuat_Click(object? sender, EventArgs e)
        {
            var xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                await authService.DangXuatAsync(_taiKhoanDangNhap.IdTaikhoan);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[FormKhachHangPortal] Gặp lỗi khi ghi log đăng xuất cho tài khoản '{Username}': {Message}",
                    _taiKhoanDangNhap.TenDangnhap, ex.Message);
            }

            YeuCauDangXuat?.Invoke(this, EventArgs.Empty);
            this.Close();
        }
    }
}

