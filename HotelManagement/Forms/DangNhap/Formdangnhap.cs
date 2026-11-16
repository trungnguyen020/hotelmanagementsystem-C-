using System;
using System.Diagnostics;
using System.Windows.Forms;
using HotelManagement.BLL.Services;
using HotelManagement.Localization;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement
{
    public partial class Formdangnhap : Form
    {
        public Formdangnhap()
        {
            InitializeComponent();
            CapNhatNgonNgu();
        }

        private async void BtnDangNhap_Click(object? sender, EventArgs e)
        {
            lblThongBao.Text = string.Empty;
            btnDangNhap.Enabled = false;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var ketQua = await authService.DangNhapAsync(txtTenDangNhap.Text, txtMatKhau.Text);

                if (ketQua.TrangThai != TrangThaiDangNhap.ThanhCong)
                {
                    lblThongBao.Text = ketQua.ThongBao;
                    return;
                }

                DinhTuyenDangNhap(ketQua.TaiKhoan!);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "[FormDangNhap] Ngoại lệ khi đăng nhập tài khoản '{Username}': {Message}", txtTenDangNhap.Text.Trim(), ex.Message);
                lblThongBao.Text = LanguageManager.GetString("FormDangNhap_ErrGeneral");
            }
            finally
            {
                btnDangNhap.Enabled = true;
            }
        }

        private void DinhTuyenDangNhap(TaiKhoan taiKhoan)
        {
            string tenVaiTro = taiKhoan.IdVaitroNavigation?.TenVaitro ?? string.Empty;

            if (string.Equals(tenVaiTro, "KhachHang", StringComparison.OrdinalIgnoreCase))
            {
                MoFormKhachHang(taiKhoan);
            }
            else
            {
                // "Admin", "LeTan" hoặc các vai trò nội bộ khác -> mở FormChinh
                MoFormChinh(taiKhoan);
            }
        }

        private void MoFormKhachHang(TaiKhoan taiKhoan)
        {
            txtTenDangNhap.Text = string.Empty;
            txtMatKhau.Text = string.Empty;
            lblThongBao.Text = string.Empty;
            Hide();

            var formPortal = new HotelManagement.Forms.FormKhachHangPortal(taiKhoan);

            // Đăng xuất từ FormKhachHangPortal: hiện lại màn hình đăng nhập, KHÔNG thoát ứng dụng
            formPortal.YeuCauDangXuat += (s, e) => Show();

            // Form portal bị đóng mà KHÔNG qua đăng xuất (bấm nút X) => thoát hẳn ứng dụng
            formPortal.FormClosed += (s, e) =>
            {
                if (!Visible)
                    Close();
            };

            formPortal.Show();
        }

        private void MoFormChinh(TaiKhoan taiKhoan)
        {
            txtTenDangNhap.Text = string.Empty;
            txtMatKhau.Text = string.Empty;
            lblThongBao.Text = string.Empty;
            Hide();

            var formChinh = new Formchinh(taiKhoan);

            // Đăng xuất từ FormChinh: hiện lại màn hình đăng nhập, KHÔNG thoát ứng dụng.
            formChinh.YeuCauDangXuat += (s, e) => Show();

            // FormChinh bị đóng mà KHÔNG qua đăng xuất (bấm nút X) => thoát hẳn ứng dụng.
            // Nếu đã đăng xuất, FormDangNhap đã được Show() lại ở trên nên Visible sẽ là true, bỏ qua.
            formChinh.FormClosed += (s, e) =>
            {
                if (!Visible)
                    Close();
            };

            formChinh.Show();
        }
    }
}
