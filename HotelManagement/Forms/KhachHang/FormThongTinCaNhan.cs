using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement.Forms
{
    public partial class FormThongTinCaNhan : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;

        /// <summary>
        /// Sự kiện phát ra khi khách hàng lưu thay đổi họ tên, báo cho FormKhachHangPortal cập nhật lại TopBar.
        /// </summary>
        public event EventHandler<string>? ThongTinKhachHangDaThayDoi;

        public FormThongTinCaNhan(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap ?? throw new ArgumentNullException(nameof(taiKhoanDangNhap));
            InitializeComponent();

            this.Load += async (_, _) => await TaiThongTinHienTaiAsync();
        }

        private async Task TaiThongTinHienTaiAsync()
        {
            txtTenDangNhap.Text = _taiKhoanDangNhap.TenDangnhap;

            if (!_taiKhoanDangNhap.IdKhachhang.HasValue)
            {
                lblThongBaoHoSo.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoHoSo.Text = "⚠️ Tài khoản này chưa liên kết với hồ sơ khách hàng.";
                btnLuuThongTin.Enabled = false;
                return;
            }

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var khachHangService = scope.ServiceProvider.GetRequiredService<IKhachHangService>();
                var khachHang = await khachHangService.GetByIdAsync(_taiKhoanDangNhap.IdKhachhang.Value);

                if (khachHang != null)
                {
                    txtHoTen.Text = khachHang.Ten;
                    txtCccd.Text = khachHang.Cccd;
                    txtSdt.Text = khachHang.Sdt;
                    txtEmail.Text = khachHang.Email;

                    int idxGioiTinh = cboGioiTinh.Items.IndexOf(khachHang.GioiTinh ?? "");
                    cboGioiTinh.SelectedIndex = idxGioiTinh >= 0 ? idxGioiTinh : 0;

                    if (khachHang.NgaySinh.HasValue)
                    {
                        chkCoNgaySinh.Checked = true;
                        dtpNgaySinh.Value = khachHang.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
                    }
                    else
                    {
                        chkCoNgaySinh.Checked = false;
                        dtpNgaySinh.Value = DateTime.Today.AddYears(-20);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormThongTinCaNhan] Lỗi khi tải thông tin khách hàng ID {IdKhachHang}: {Message}",
                    _taiKhoanDangNhap.IdKhachhang, ex.Message);
                lblThongBaoHoSo.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoHoSo.Text = "Không thể tải hồ sơ khách hàng: " + ex.Message;
            }
        }

        private async Task LuuThongTinAsync()
        {
            if (!_taiKhoanDangNhap.IdKhachhang.HasValue)
            {
                MessageBox.Show("Tài khoản chưa liên kết với hồ sơ khách hàng.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string hoTen = txtHoTen.Text.Trim();
            string sdt = txtSdt.Text.Trim();
            string email = txtEmail.Text.Trim();
            string? gioiTinh = cboGioiTinh.SelectedItem?.ToString();
            DateOnly? ngaySinh = chkCoNgaySinh.Checked ? DateOnly.FromDateTime(dtpNgaySinh.Value) : null;

            lblThongBaoHoSo.Text = string.Empty;
            btnLuuThongTin.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var khachHangService = scope.ServiceProvider.GetRequiredService<IKhachHangService>();

                var (success, message, updatedDto) = await khachHangService.CapNhatThongTinCaNhanAsync(
                    _taiKhoanDangNhap.IdKhachhang.Value, hoTen, sdt, email, gioiTinh, ngaySinh);

                if (success)
                {
                    lblThongBaoHoSo.ForeColor = Color.FromArgb(46, 125, 50); // Dark Green
                    lblThongBaoHoSo.Text = "✓ " + message;

                    MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Cập nhật navigation entity nếu đang được giữ trong session
                    if (_taiKhoanDangNhap.IdKhachhangNavigation != null && updatedDto != null)
                    {
                        _taiKhoanDangNhap.IdKhachhangNavigation.Ten = updatedDto.Ten;
                        _taiKhoanDangNhap.IdKhachhangNavigation.Sdt = updatedDto.Sdt;
                        _taiKhoanDangNhap.IdKhachhangNavigation.Email = updatedDto.Email;
                        _taiKhoanDangNhap.IdKhachhangNavigation.Gioitinh = updatedDto.GioiTinh;
                        _taiKhoanDangNhap.IdKhachhangNavigation.Ngaysinh = updatedDto.NgaySinh;
                    }

                    // Phát sự kiện cập nhật tên khách lên TopBar
                    ThongTinKhachHangDaThayDoi?.Invoke(this, hoTen);
                }
                else
                {
                    lblThongBaoHoSo.ForeColor = AppTheme.StatusDangerText;
                    lblThongBaoHoSo.Text = "⚠️ " + message;
                    MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormThongTinCaNhan] Ngoại lệ khi cập nhật thông tin cá nhân: {Message}", ex.Message);
                lblThongBaoHoSo.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoHoSo.Text = "Có lỗi xảy ra: " + ex.Message;
                MessageBox.Show("Có lỗi xảy ra khi lưu thông tin: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLuuThongTin.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task DoiMatKhauAsync()
        {
            string matKhauCu = txtMatKhauCu.Text;
            string matKhauMoi = txtMatKhauMoi.Text;
            string xacNhanMatKhau = txtXacNhanMatKhau.Text;

            lblThongBaoBaoMat.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(matKhauCu))
            {
                lblThongBaoBaoMat.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoBaoMat.Text = "⚠️ Vui lòng nhập mật khẩu hiện tại.";
                txtMatKhauCu.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(matKhauMoi))
            {
                lblThongBaoBaoMat.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoBaoMat.Text = "⚠️ Vui lòng nhập mật khẩu mới.";
                txtMatKhauMoi.Focus();
                return;
            }

            if (matKhauMoi.Length < 6)
            {
                lblThongBaoBaoMat.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoBaoMat.Text = "⚠️ Mật khẩu mới phải có độ dài tối thiểu 6 ký tự.";
                txtMatKhauMoi.Focus();
                return;
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                lblThongBaoBaoMat.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoBaoMat.Text = "⚠️ Mật khẩu mới và xác nhận mật khẩu không khớp.";
                txtXacNhanMatKhau.Focus();
                return;
            }

            btnDoiMatKhau.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var (success, message) = await authService.DoiMatKhauAsync(
                    _taiKhoanDangNhap.IdTaikhoan, matKhauCu, matKhauMoi);

                if (success)
                {
                    txtMatKhauCu.Clear();
                    txtMatKhauMoi.Clear();
                    txtXacNhanMatKhau.Clear();

                    lblThongBaoBaoMat.ForeColor = Color.FromArgb(46, 125, 50); // Dark Green
                    lblThongBaoBaoMat.Text = "✓ Đổi mật khẩu thành công! Bạn có thể tiếp tục sử dụng phiên này bình thường.";

                    MessageBox.Show(
                        "Đổi mật khẩu tài khoản thành công!\nMật khẩu mới sẽ áp dụng từ lần đăng nhập tiếp theo.\nPhiên làm việc hiện tại vẫn tiếp tục bình thường.",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    lblThongBaoBaoMat.ForeColor = AppTheme.StatusDangerText;
                    lblThongBaoBaoMat.Text = "⚠️ " + message;
                    MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormThongTinCaNhan] Ngoại lệ khi đổi mật khẩu: {Message}", ex.Message);
                lblThongBaoBaoMat.ForeColor = AppTheme.StatusDangerText;
                lblThongBaoBaoMat.Text = "Có lỗi xảy ra: " + ex.Message;
                MessageBox.Show("Có lỗi xảy ra khi đổi mật khẩu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDoiMatKhau.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}
