using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement.Forms
{
    public partial class FormDangKyKhachHang : Form
    {
        public FormDangKyKhachHang()
        {
            InitializeComponent();
        }

        private async void BtnDangKy_Click(object? sender, EventArgs e)
        {
            lblThongBao.Text = string.Empty;

            // 1. Validate Form phía UI
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtCccd.Text) ||
                string.IsNullOrWhiteSpace(txtSdt.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtTenDangNhap.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Text) ||
                string.IsNullOrWhiteSpace(txtXacNhanMatKhau.Text))
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Vui lòng điền đầy đủ các thông tin bắt buộc (*).";
                return;
            }

            string hoTen = txtHoTen.Text.Trim();
            string cccd = txtCccd.Text.Trim();
            string sdt = txtSdt.Text.Trim();
            string email = txtEmail.Text.Trim();
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;
            string xacNhanMatKhau = txtXacNhanMatKhau.Text;

            // CCCD 12 chữ số
            if (!Regex.IsMatch(cccd, @"^\d{12}$"))
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "CCCD không hợp lệ (phải gồm đúng 12 chữ số).";
                txtCccd.Focus();
                return;
            }

            // Email
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Định dạng email không đúng (ví dụ: email@domain.com).";
                txtEmail.Focus();
                return;
            }

            // Độ mạnh mật khẩu (ít nhất 6 ký tự)
            if (matKhau.Length < 6)
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Mật khẩu phải có độ dài tối thiểu 6 ký tự.";
                txtMatKhau.Focus();
                return;
            }

            // Khớp mật khẩu
            if (matKhau != xacNhanMatKhau)
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Mật khẩu xác nhận không trùng khớp.";
                txtXacNhanMatKhau.Focus();
                return;
            }

            var dto = new KhachHangDangKyDto
            {
                HoTen = hoTen,
                Cccd = cccd,
                Sdt = sdt,
                Email = email,
                TenDangNhap = tenDangNhap,
                MatKhau = matKhau,
                XacNhanMatKhau = xacNhanMatKhau
            };

            btnDangKy.Enabled = false;
            btnHuy.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var (success, message) = await authService.DangKyKhachHangAsync(dto);

                if (!success)
                {
                    lblThongBao.ForeColor = AppTheme.StatusDangerText;
                    lblThongBao.Text = message;
                    return;
                }

                MessageBox.Show(
                    "Đăng ký tài khoản khách hàng thành công!\nBạn có thể dùng tài khoản này để đăng nhập hệ thống hoặc Kiosk.",
                    "Đăng ký thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormDangKyKhachHang] Lỗi ngoại lệ khi đăng ký tài khoản khách hàng: {Message}", ex.Message);
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Đã xảy ra sự cố ngoài dự kiến trong quá trình đăng ký. Vui lòng thử lại.";
            }
            finally
            {
                btnDangKy.Enabled = true;
                btnHuy.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}

