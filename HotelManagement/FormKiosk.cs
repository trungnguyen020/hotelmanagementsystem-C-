using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Kiosk tra cứu thông tin đặt phòng tự phục vụ tại sảnh khách sạn (DCKLTTQ-85).
    /// Chạy ở chế độ Toàn màn hình (Full-screen, không thanh tiêu đề, không menu quản trị).
    /// Bắt buộc nhập đồng thời Mã Booking và Số điện thoại để bảo mật thông tin khách hàng.
    /// </summary>
    public partial class FormKiosk : Form
    {
        private readonly IServiceScope _scope;
        private readonly IKioskService _kioskService;
        private readonly IAuthService _authService;

        private System.Windows.Forms.Timer? _clockTimer;
        private System.Windows.Forms.Timer? _autoClearTimer;
        private int _secondsRemaining = 60;

        public FormKiosk()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _kioskService = _scope.ServiceProvider.GetRequiredService<IKioskService>();
            _authService = _scope.ServiceProvider.GetRequiredService<IAuthService>();

            InitializeComponent();

            KhoiTaoSuKienVaDongHo();
        }

        private void KhoiTaoSuKienVaDongHo()
        {
            FormClosed += (_, _) =>
            {
                _clockTimer?.Stop();
                _clockTimer?.Dispose();
                _autoClearTimer?.Stop();
                _autoClearTimer?.Dispose();
                _scope.Dispose();
            };

            // 1. Đồng hồ thời gian thực ở góc trên màn hình
            _lblClock.Text = DateTime.Now.ToString("HH:mm:ss  •  dd/MM/yyyy");
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (_, _) =>
            {
                _lblClock.Text = DateTime.Now.ToString("HH:mm:ss  •  dd/MM/yyyy");
            };
            _clockTimer.Start();

            // 2. Timer tự động xóa thông tin sau 60 giây để bảo vệ quyền riêng tư cho khách tiếp theo
            _autoClearTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _autoClearTimer.Tick += (_, _) =>
            {
                _secondsRemaining--;
                if (_secondsRemaining <= 0)
                {
                    _autoClearTimer.Stop();
                    XoaTrangTatCa();
                }
                else
                {
                    _lblAutoClearCount.Text = $"⏱️ Tự động xóa sau: {_secondsRemaining}s";
                }
            };

            // 3. Sự kiện tra cứu
            _btnTraCuu.Click += async (_, _) => await ThucHienTraCuuAsync();
            _btnXoaTrang.Click += (_, _) => XoaTrangTatCa();
            _btnXoaKetQua.Click += (_, _) => XoaTrangTatCa();

            // Nhấn Enter trên ô nhập để tra cứu ngay
            _txtMaBooking.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    _txtSdt.Focus();
                }
            };

            _txtSdt.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await ThucHienTraCuuAsync();
                }
            };

            // 4. Cơ chế thoát an toàn (Phím tắt Ctrl+Alt+Q hoặc nút icon ổ khóa bí mật)
            KeyDown += async (_, e) =>
            {
                if (e.Control && e.Alt && e.KeyCode == Keys.Q)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    await XacThucThoatKioskAsync();
                }
            };

            _btnThoatKiosk.Click += async (_, _) => await XacThucThoatKioskAsync();
        }

        private async Task ThucHienTraCuuAsync()
        {
            string maBooking = _txtMaBooking.Text.Trim();
            string sdt = _txtSdt.Text.Trim();

            // Validate cả 2 trường bắt buộc
            if (string.IsNullOrEmpty(maBooking) || string.IsNullOrEmpty(sdt))
            {
                HienThiThongBaoLoi("Vui lòng nhập đầy đủ cả Mã đặt phòng và Số điện thoại.");
                return;
            }

            try
            {
                _btnTraCuu.Enabled = false;
                _btnTraCuu.Text = "⏳  ĐANG TRA CỨU HỆ THỐNG...";

                // Gọi tầng BLL xác thực cả 2 yếu tố
                var booking = await _kioskService.TraCuuBookingAsync(maBooking, sdt);

                if (booking == null)
                {
                    // Thông báo chung chung để tránh dò thông tin khách khác
                    HienThiThongBaoLoi("Không tìm thấy thông tin đặt phòng.\nVui lòng kiểm tra lại mã booking và số điện thoại.");
                    HienThiTrangThaiCho();
                }
                else
                {
                    AnThongBaoLoi();
                    HienThiKetQuaBooking(booking);
                }
            }
            catch (Exception ex)
            {
                HienThiThongBaoLoi("Hệ thống tạm thời bận. Vui lòng liên hệ nhân viên lễ tân: " + ex.Message);
            }
            finally
            {
                _btnTraCuu.Enabled = true;
                _btnTraCuu.Text = "🔍  TRA CỨU THÔNG TIN PHÒNG";
            }
        }

        private void HienThiKetQuaBooking(KioskBookingDto res)
        {
            _pnlResultPlaceholder.Visible = false;
            _pnlResultContent.Visible = true;

            _lblResMaBooking.Text = $"MÃ BOOKING: {res.MaBooking}";
            _lblResTrangThai.Text = res.TrangThaiBooking.ToUpperInvariant();

            // Màu badge trạng thái
            if (res.TrangThaiBooking.Contains("xác nhận", StringComparison.OrdinalIgnoreCase) ||
                res.TrangThaiBooking.Contains("check-in", StringComparison.OrdinalIgnoreCase))
            {
                _lblResTrangThai.BackColor = AppTheme.StatusSuccessBg;
                _lblResTrangThai.ForeColor = AppTheme.StatusSuccessText;
            }
            else
            {
                _lblResTrangThai.BackColor = AppTheme.StatusWarningBg;
                _lblResTrangThai.ForeColor = AppTheme.StatusWarningText;
            }

            _lblResKhachHang.Text = $"Quý khách: {res.TenKhachHang}";
            _lblResThoiGian.Text = $"Thời gian lưu trú: {res.NgayNhanDuKien:dd/MM/yyyy}  ➔  {res.NgayTraDuKien:dd/MM/yyyy}  ({res.SoDem} đêm)";
            _lblResSoKhach.Text = $"Số lượng khách: {res.SoNguoi} khách";

            // Danh sách phòng
            _lstPhong.Items.Clear();
            if (res.DanhSachPhong != null && res.DanhSachPhong.Any())
            {
                foreach (var p in res.DanhSachPhong)
                {
                    _lstPhong.Items.Add($"  🏠 Phòng {p.SoPhong}  •  {p.TenLoaiPhong}");
                }
            }
            else
            {
                _lstPhong.Items.Add("  (Chưa chỉ định phòng cụ thể)");
            }

            // Dịch vụ
            _lblResDichVu.Text = res.DanhSachDichVu != null && res.DanhSachDichVu.Any()
                ? "Dịch vụ đi kèm: " + string.Join(", ", res.DanhSachDichVu)
                : "Dịch vụ đi kèm: (Không có)";

            // Thanh toán
            _lblResThanhToan.Text = $"Thanh toán: {res.TrangThaiThanhToan} (Tổng cộng: {res.TongTien:N0} đ)";
            _lblResThanhToan.ForeColor = res.TrangThaiThanhToan.Contains("Đã thanh toán", StringComparison.OrdinalIgnoreCase)
                ? AppTheme.StatusSuccessText
                : AppTheme.StatusWarningText;

            // Khởi động bộ đếm 60 giây tự động xóa
            _secondsRemaining = 60;
            _lblAutoClearCount.Text = "⏱️ Tự động xóa sau: 60s";
            _autoClearTimer?.Stop();
            _autoClearTimer?.Start();
        }

        private void HienThiTrangThaiCho()
        {
            _pnlResultPlaceholder.Visible = true;
            _pnlResultContent.Visible = false;
            _autoClearTimer?.Stop();
        }

        private void XoaTrangTatCa()
        {
            _txtMaBooking.Clear();
            _txtSdt.Clear();
            AnThongBaoLoi();
            HienThiTrangThaiCho();
            _txtMaBooking.Focus();
        }

        private void HienThiThongBaoLoi(string message)
        {
            _lblThongBao.Text = message;
            _pnlThongBao.Visible = true;
        }

        private void AnThongBaoLoi()
        {
            _lblThongBao.Text = string.Empty;
            _pnlThongBao.Visible = false;
        }

        /// <summary>
        /// Yêu cầu mật khẩu Admin để thoát khỏi chế độ Kiosk toàn màn hình (async, không block UI thread).
        /// </summary>
        private async Task XacThucThoatKioskAsync()
        {
            using var dlg = new Form();
            dlg.Text = "Xác thực Quản trị viên (Admin)";
            dlg.Size = new Size(420, 240);
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.MinimizeBox = false;
            dlg.BackColor = AppTheme.Surface;
            dlg.KeyPreview = true;

            var lblPrompt = new Label
            {
                Text = "Nhập mật khẩu Admin để thoát chế độ Kiosk:",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(24, 18),
                AutoSize = true
            };

            var txtPass = new TextBox
            {
                Location = new Point(24, 48),
                Width = 356,
                Font = new Font("Segoe UI", 12F),
                UseSystemPasswordChar = true
            };

            var lblError = new Label
            {
                Text = string.Empty,
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.StatusDangerText,
                Location = new Point(24, 85),
                Size = new Size(356, 36),
                Visible = false
            };

            var btnOk = new Button
            {
                Text = "Thoát Kiosk",
                Location = new Point(155, 135),
                Size = new Size(120, 38),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnOk);

            var btnCancel = new Button
            {
                Text = "Hủy",
                DialogResult = DialogResult.Cancel,
                Location = new Point(285, 135),
                Size = new Size(95, 38),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(btnCancel);

            bool authenticated = false;

            btnOk.Click += async (s, e) =>
            {
                string pass = txtPass.Text;
                if (string.IsNullOrWhiteSpace(pass))
                {
                    lblError.Text = "Vui lòng nhập mật khẩu Quản trị viên.";
                    lblError.Visible = true;
                    txtPass.Focus();
                    return;
                }

                btnOk.Enabled = false;
                btnCancel.Enabled = false;
                btnOk.Text = "Đang kiểm tra...";

                try
                {
                    var ketQua = await _authService.DangNhapAsync("admin", pass);
                    if (ketQua.TrangThai == TrangThaiDangNhap.ThanhCong &&
                        ketQua.TaiKhoan?.IdVaitroNavigation?.TenVaitro == "Admin")
                    {
                        authenticated = true;
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                    }
                    else
                    {
                        lblError.Text = "Mật khẩu Admin không đúng hoặc tài khoản bị khóa.";
                        lblError.Visible = true;
                        txtPass.Clear();
                        txtPass.Focus();
                        btnOk.Enabled = true;
                        btnCancel.Enabled = true;
                        btnOk.Text = "Thoát Kiosk";
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Lỗi xác thực: " + ex.Message;
                    lblError.Visible = true;
                    btnOk.Enabled = true;
                    btnCancel.Enabled = true;
                    btnOk.Text = "Thoát Kiosk";
                }
            };

            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;
            dlg.Controls.AddRange(new Control[] { lblPrompt, txtPass, lblError, btnOk, btnCancel });

            dlg.ShowDialog(this);

            if (authenticated)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

