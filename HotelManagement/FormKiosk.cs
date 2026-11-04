using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Kiosk tra cứu thông tin đặt phòng tự phục vụ tại sảnh khách sạn (DCKLTTQ-85).
    /// Chạy ở chế độ Toàn màn hình (Full-screen, không thanh tiêu đề, không menu quản trị).
    /// Bắt buộc nhập đồng thời Mã Booking và Số điện thoại để bảo mật thông tin khách hàng.
    /// Hỗ trợ đa ngôn ngữ Việt / Anh tức thì (DCKLTTQ-86).
    /// </summary>
    public partial class FormKiosk : Form
    {
        private readonly IServiceScope _scope;
        private readonly IKioskService _kioskService;
        private readonly IAuthService _authService;
        private readonly IChatbotService _chatbotService;

        private System.Windows.Forms.Timer? _clockTimer;
        private System.Windows.Forms.Timer? _autoClearTimer;
        private int _secondsRemaining = 60;
        private KioskBookingDto? _currentBooking;

        // Quản lý trạng thái Chatbot AI (DCKLTTQ-88)
        private readonly List<ChatMessageDto> _chatHistory = new();
        private bool _isAiReplying = false;

        public FormKiosk()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _kioskService = _scope.ServiceProvider.GetRequiredService<IKioskService>();
            _authService = _scope.ServiceProvider.GetRequiredService<IAuthService>();
            _chatbotService = _scope.ServiceProvider.GetRequiredService<IChatbotService>();

            InitializeComponent();

            KhoiTaoSuKienVaDongHo();
            KhoiTaoChatbot();
            CapNhatNgonNgu();
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
                    _lblAutoClearCount.Text = string.Format(LanguageManager.GetString("FormKiosk_AutoClearCount"), _secondsRemaining);
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

            // 5. Sự kiện Chatbot AI Gemini (DCKLTTQ-88)
            _btnMoChatbot.Click += (_, _) => BatTatChatDrawer();
            _btnChatClose.Click += (_, _) => BatTatChatDrawer(false);
            _btnChatReset.Click += (_, _) => LamMoiHoiThoai();

            _btnChatSend.Click += async (_, _) => await ThucHienGuiTinNhanAsync();
            _txtChatInput.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await ThucHienGuiTinNhanAsync();
                }
            };

            // Gợi ý câu hỏi nhanh (Chips)
            _btnChipGiaPhong.Click += async (_, _) => await GuiCauHoiNhanhAsync("Giá các loại phòng hiện tại là bao nhiêu?");
            _btnChipCheckIn.Click += async (_, _) => await GuiCauHoiNhanhAsync("Quy định giờ check-in và check-out như thế nào?");
            _btnChipPhongTrong.Click += async (_, _) => await GuiCauHoiNhanhAsync("Hôm nay khách sạn còn những phòng nào trống?");
            _btnChipChinhSachHuy.Click += async (_, _) => await GuiCauHoiNhanhAsync("Chính sách hủy phòng của khách sạn như thế nào?");
        }

        private async Task ThucHienTraCuuAsync()
        {
            string maBooking = _txtMaBooking.Text.Trim();
            string sdt = _txtSdt.Text.Trim();

            // Validate cả 2 trường bắt buộc
            if (string.IsNullOrEmpty(maBooking) || string.IsNullOrEmpty(sdt))
            {
                HienThiThongBaoLoi(LanguageManager.GetString("FormKiosk_ErrEmpty"));
                return;
            }

            try
            {
                _btnTraCuu.Enabled = false;
                _btnTraCuu.Text = "⏳  " + LanguageManager.GetString("FormKiosk_Searching");

                // Gọi tầng BLL xác thực cả 2 yếu tố
                var booking = await _kioskService.TraCuuBookingAsync(maBooking, sdt);

                if (booking == null)
                {
                    // Thông báo chung chung để tránh dò thông tin khách khác
                    HienThiThongBaoLoi(LanguageManager.GetString("FormKiosk_ErrNotFound"));
                    HienThiTrangThaiCho();
                }
                else
                {
                    AnThongBaoLoi();
                    _currentBooking = booking;
                    HienThiKetQuaBooking(booking);
                }
            }
            catch (Exception ex)
            {
                HienThiThongBaoLoi(ex.Message);
            }
            finally
            {
                _btnTraCuu.Enabled = true;
                _btnTraCuu.Text = LanguageManager.GetString("FormKiosk_BtnTraCuu");
            }
        }

        private void HienThiKetQuaBooking(KioskBookingDto res)
        {
            _pnlResultPlaceholder.Visible = false;
            _pnlResultContent.Visible = true;

            _lblResMaBooking.Text = $"{LanguageManager.GetString("FormKiosk_Res_MaBooking")} {res.MaBooking}";
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

            string nightWord = LanguageManager.CurrentLanguage == AppLanguage.English ? "nights" : "đêm";
            string guestWord = LanguageManager.CurrentLanguage == AppLanguage.English ? "guests" : "khách";

            _lblResKhachHang.Text = $"{LanguageManager.GetString("FormKiosk_Res_KhachHang")} {res.TenKhachHang}";
            _lblResThoiGian.Text = $"{LanguageManager.GetString("FormKiosk_Res_ThoiGian")} {res.NgayNhanDuKien:dd/MM/yyyy}  ➔  {res.NgayTraDuKien:dd/MM/yyyy}  ({res.SoDem} {nightWord})";
            _lblResSoKhach.Text = $"{LanguageManager.GetString("FormKiosk_Res_SoKhach")} {res.SoNguoi} {guestWord}";

            // Danh sách phòng
            _lstPhong.Items.Clear();
            if (res.DanhSachPhong != null && res.DanhSachPhong.Any())
            {
                string roomWord = LanguageManager.CurrentLanguage == AppLanguage.English ? "Room" : "Phòng";
                foreach (var p in res.DanhSachPhong)
                {
                    _lstPhong.Items.Add($"  🏠 {roomWord} {p.SoPhong}  •  {p.TenLoaiPhong}");
                }
            }
            else
            {
                _lstPhong.Items.Add(LanguageManager.CurrentLanguage == AppLanguage.English
                    ? "  (No specific room assigned yet)"
                    : "  (Chưa chỉ định phòng cụ thể)");
            }

            // Dịch vụ
            string dichVuText = res.DanhSachDichVu != null && res.DanhSachDichVu.Any()
                ? string.Join(", ", res.DanhSachDichVu)
                : LanguageManager.GetString("FormKiosk_Res_None");
            _lblResDichVu.Text = $"{LanguageManager.GetString("FormKiosk_Res_DichVu")} {dichVuText}";

            // Thanh toán
            string paymentStatus = res.TrangThaiThanhToan.Contains("Đã thanh toán", StringComparison.OrdinalIgnoreCase)
                ? LanguageManager.GetString("FormKiosk_Res_Paid")
                : LanguageManager.GetString("FormKiosk_Res_Unpaid");

            _lblResThanhToan.Text = $"{LanguageManager.GetString("FormKiosk_Res_ThanhToan")} {paymentStatus} ({LanguageManager.GetString("FormKiosk_Res_TongTien")} {res.TongTien:N0} đ)";
            _lblResThanhToan.ForeColor = res.TrangThaiThanhToan.Contains("Đã thanh toán", StringComparison.OrdinalIgnoreCase)
                ? AppTheme.StatusSuccessText
                : AppTheme.StatusWarningText;

            // Khởi động bộ đếm 60 giây tự động xóa
            _secondsRemaining = 60;
            _lblAutoClearCount.Text = string.Format(LanguageManager.GetString("FormKiosk_AutoClearCount"), 60);
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
            _currentBooking = null;
            _txtMaBooking.Clear();
            _txtSdt.Clear();
            AnThongBaoLoi();
            HienThiTrangThaiCho();
            LamMoiHoiThoai();
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
            dlg.Text = LanguageManager.GetString("FormKiosk_Exit_Title");
            dlg.Size = new Size(420, 240);
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.MinimizeBox = false;
            dlg.BackColor = AppTheme.Surface;
            dlg.KeyPreview = true;

            var lblPrompt = new Label
            {
                Text = LanguageManager.GetString("FormKiosk_Exit_Prompt"),
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
                Text = LanguageManager.GetString("FormKiosk_Exit_BtnOk"),
                Location = new Point(155, 135),
                Size = new Size(120, 38),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnOk);

            var btnCancel = new Button
            {
                Text = LanguageManager.GetString("FormKiosk_Exit_BtnCancel"),
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
                    lblError.Text = LanguageManager.GetString("FormKiosk_Exit_ErrEmpty");
                    lblError.Visible = true;
                    txtPass.Focus();
                    return;
                }

                btnOk.Enabled = false;
                btnCancel.Enabled = false;
                btnOk.Text = LanguageManager.GetString("FormKiosk_Exit_Checking");

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
                        lblError.Text = LanguageManager.GetString("FormKiosk_Exit_ErrWrong");
                        lblError.Visible = true;
                        txtPass.Clear();
                        txtPass.Focus();
                        btnOk.Enabled = true;
                        btnCancel.Enabled = true;
                        btnOk.Text = LanguageManager.GetString("FormKiosk_Exit_BtnOk");
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    lblError.Visible = true;
                    btnOk.Enabled = true;
                    btnCancel.Enabled = true;
                    btnOk.Text = LanguageManager.GetString("FormKiosk_Exit_BtnOk");
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

        #region Chatbot Gemini Methods (DCKLTTQ-88)

        private void KhoiTaoChatbot()
        {
            LamMoiHoiThoai();
        }

        private void BatTatChatDrawer(bool? hienThi = null)
        {
            bool newState = hienThi ?? !_pnlChatDrawer.Visible;
            _pnlChatDrawer.Visible = newState;
            if (newState)
            {
                _txtChatInput.Focus();
                _secondsRemaining = 60;
            }
        }

        private void LamMoiHoiThoai()
        {
            _chatHistory.Clear();
            _flpChatMessages.Controls.Clear();
            string welcomeMsg = LanguageManager.GetString(
                "FormKiosk_ChatWelcome",
                "Xin chào Quý khách! 🛎️ Em là trợ lý ảo của khách sạn. Em có thể giải đáp cho Quý khách về các loại phòng, mức giá, số phòng trống hôm nay và chính sách nhận/trả phòng. Quý khách cần em tư vấn thông tin gì ạ?");
            ThemBongBongTinNhan("model", welcomeMsg);
        }

        private async Task GuiCauHoiNhanhAsync(string cauHoi)
        {
            _txtChatInput.Text = cauHoi;
            await ThucHienGuiTinNhanAsync();
        }

        private async Task ThucHienGuiTinNhanAsync()
        {
            string cauHoi = _txtChatInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(cauHoi) || _isAiReplying)
                return;

            _secondsRemaining = 60;
            _txtChatInput.Clear();

            // 1. Thêm tin nhắn của User lên giao diện và lịch sử
            ThemBongBongTinNhan("user", cauHoi);
            _chatHistory.Add(ChatMessageDto.CreateUser(cauHoi));

            // 2. Khóa nút gửi, hiện trạng thái đang suy nghĩ
            _isAiReplying = true;
            _lblChatStatus.Visible = true;
            _btnChatSend.Enabled = false;

            try
            {
                // 3. Gửi câu hỏi kèm lịch sử phiên hiện tại tới Gemini API
                var phanHoi = await _chatbotService.GuiTinNhanAsync(cauHoi, _chatHistory);
                ThemBongBongTinNhan(phanHoi.Role, phanHoi.Content, phanHoi.IsError);
                _chatHistory.Add(phanHoi);
            }
            catch (Exception)
            {
                // Fallback an toàn tuyệt đối, không crash ứng dụng
                string fallback = LanguageManager.CurrentLanguage == AppLanguage.English
                    ? "Sorry, the virtual assistant is currently unavailable. Please contact the front desk for direct assistance."
                    : "Xin lỗi, hệ thống tư vấn hiện không khả dụng, quý khách vui lòng liên hệ lễ tân để được hỗ trợ trực tiếp.";
                ThemBongBongTinNhan("model", fallback, isError: true);
            }
            finally
            {
                _isAiReplying = false;
                _lblChatStatus.Visible = false;
                _btnChatSend.Enabled = true;
                _txtChatInput.Focus();
                _secondsRemaining = 60;
            }
        }

        private void ThemBongBongTinNhan(string role, string noiDung, bool isError = false)
        {
            bool isUser = role.Equals("user", StringComparison.OrdinalIgnoreCase);

            int panelWidth = Math.Max(_flpChatMessages.ClientSize.Width - 28, 320);
            var row = new Panel
            {
                Width = panelWidth,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 10)
            };

            var bubble = new Panel
            {
                Padding = new Padding(12, 10, 12, 10),
                BackColor = isUser
                    ? AppTheme.PrimaryLight
                    : (isError ? AppTheme.StatusWarningBg : Color.White)
            };
            AppTheme.ApplyCardPanel(bubble);

            var lblText = new Label
            {
                Text = noiDung,
                Font = AppTheme.FontBody,
                ForeColor = isError ? AppTheme.StatusWarningText : AppTheme.TextPrimary,
                AutoSize = true,
                MaximumSize = new Size(330, 0),
                Location = new Point(12, 10)
            };

            var lblTime = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize = true
            };

            bubble.Controls.Add(lblText);
            bubble.Controls.Add(lblTime);

            int bubbleWidth = Math.Min(Math.Max(lblText.PreferredWidth + 26, 120), 360);
            lblTime.Location = new Point(12, lblText.PreferredHeight + 14);
            bubble.Size = new Size(bubbleWidth, lblText.PreferredHeight + lblTime.PreferredHeight + 22);

            if (isUser)
            {
                bubble.Location = new Point(row.Width - bubble.Width - 4, 0);
                bubble.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }
            else
            {
                bubble.Location = new Point(4, 0);
                bubble.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            row.Height = bubble.Height + 4;
            row.Controls.Add(bubble);

            _flpChatMessages.Controls.Add(row);
            _flpChatMessages.ScrollControlIntoView(row);
        }

        #endregion
    }
}
