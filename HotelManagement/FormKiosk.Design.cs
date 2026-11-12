using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    partial class FormKiosk
    {
        private System.ComponentModel.IContainer components = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Controls

        // Top Header
        private Panel _pnlHeader = null!;
        private Label _lblHeaderHotel = null!;
        private Label _lblHeaderSub = null!;
        private Label _lblClock = null!;
        private Button _btnLangVi = null!;
        private Button _btnLangEn = null!;

        // Main Layout Container
        private TableLayoutPanel _tlpMain = null!;

        // Left Panel - Tra cứu
        private Panel _pnlSearchCard = null!;
        private Label _lblSearchTitle = null!;
        private Label _lblSearchSub = null!;
        private Label _lblMaBookingTitle = null!;
        private TextBox _txtMaBooking = null!;
        private Label _lblSdtTitle = null!;
        private TextBox _txtSdt = null!;
        private Button _btnTraCuu = null!;
        private Button _btnXoaTrang = null!;
        private Panel _pnlThongBao = null!;
        private Label _lblThongBao = null!;

        // Right Panel - Kết quả
        private Panel _pnlResultCard = null!;
        private Label _lblResultTitle = null!;
        private Panel _pnlResultPlaceholder = null!;
        private Label _lblPlaceholderIcon = null!;
        private Label _lblPlaceholderText = null!;

        private Panel _pnlResultContent = null!;
        private Label _lblResMaBooking = null!;
        private Label _lblResTrangThai = null!;
        private Label _lblResKhachHang = null!;
        private Label _lblResThoiGian = null!;
        private Label _lblResSoKhach = null!;
        private Label _lblResThanhToan = null!;
        private Label _lblResDichVu = null!;
        private ListBox _lstPhong = null!;
        private Button _btnXoaKetQua = null!;
        private Label _lblAutoClearCount = null!;

        // Tab chính: Tra cứu booking / Giới thiệu khách sạn (DCKLTTQ-103)
        private TabControl _tabMain = null!;
        private TabPage _tabTraCuu = null!;
        private TabPage _tabGioiThieu = null!;

        // Tab Giới thiệu khách sạn
        private Panel _pnlIntroCard = null!;
        private Label _lblIntroTitle = null!;
        private Label _lblIntroContent = null!;
        private Label _lblIntroHoursTitle = null!;
        private Label _lblIntroHours = null!;
        private Label _lblIntroContactTitle = null!;
        private Label _lblIntroContact = null!;
        private Panel _pnlDichVuCard = null!;
        private Label _lblDichVuTitle = null!;
        private Label _lblDichVuTrangThai = null!;
        private FlowLayoutPanel _flpDichVu = null!;

        // Bottom Footer
        private Panel _pnlFooter = null!;
        private Label _lblHelpText = null!;
        private Button _btnMoChatbot = null!;
        private Button _btnThoatKiosk = null!;

        // Chatbot Panel Controls (DCKLTTQ-88)
        private Panel _pnlChatDrawer = null!;
        private Panel _pnlChatHeader = null!;
        private Label _lblChatBotIcon = null!;
        private Label _lblChatBotTitle = null!;
        private Label _lblChatBotSub = null!;
        private Button _btnChatReset = null!;
        private Button _btnChatClose = null!;
        private FlowLayoutPanel _flpChatMessages = null!;
        private Panel _pnlChatBottom = null!;
        private Label _lblChatStatus = null!;
        private FlowLayoutPanel _flpSuggestions = null!;
        private Button _btnChipGiaPhong = null!;
        private Button _btnChipCheckIn = null!;
        private Button _btnChipPhongTrong = null!;
        private Button _btnChipChinhSachHuy = null!;
        private Panel _pnlChatInputRow = null!;
        private TextBox _txtChatInput = null!;
        private Button _btnChatSend = null!;

        #endregion

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "RESORT & HOTEL MANAGEMENT - KIOSK";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = AppTheme.Background;
            Font = AppTheme.FontBody;
            ForeColor = AppTheme.TextPrimary;
            KeyPreview = true;
            DoubleBuffered = true;

            // ================= 1. Top Header Banner =================
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 95,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(36, 14, 36, 14)
            };

            _lblHeaderHotel = new Label
            {
                Text = "🛎️ RESORT & HOTEL MANAGEMENT  •  KIOSK TRA CỨU ĐẶT PHÒNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                AutoSize = true,
                Location = new Point(36, 16),
                UseMnemonic = false
            };

            _lblHeaderSub = new Label
            {
                Text = "Hệ thống tự phục vụ dành cho Quý khách hàng (Tra cứu thông tin nhận phòng bằng Mã Booking và Số điện thoại).",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(38, 52)
            };

            _lblClock = new Label
            {
                Text = "00:00:00",
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                AutoSize = true
            };

            // Nút chuyển đổi ngôn ngữ Việt / Anh ngay trên banner Kiosk (DCKLTTQ-86)
            _btnLangVi = new Button
            {
                Text = "Tiếng Việt",
                Size = new Size(100, 36),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(_btnLangVi);
            _btnLangVi.Click += (s, e) => HotelManagement.Localization.LanguageManager.SetLanguage(HotelManagement.Localization.AppLanguage.Vietnamese);

            _btnLangEn = new Button
            {
                Text = "English",
                Size = new Size(90, 36),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnLangEn);
            _btnLangEn.Click += (s, e) => HotelManagement.Localization.LanguageManager.SetLanguage(HotelManagement.Localization.AppLanguage.English);

            void CanChinhHeaderKiosk()
            {
                int right = _pnlHeader.ClientSize.Width - 36;
                _btnLangEn.Location = new Point(right - _btnLangEn.Width, 24);
                _btnLangVi.Location = new Point(_btnLangEn.Left - _btnLangVi.Width - 10, 24);
                _lblClock.Location = new Point(_btnLangVi.Left - _lblClock.Width - 28, 30);
            }

            _pnlHeader.Resize += (s, e) => CanChinhHeaderKiosk();
            CanChinhHeaderKiosk();

            _pnlHeader.Controls.AddRange(new Control[] { _lblHeaderHotel, _lblHeaderSub, _lblClock, _btnLangVi, _btnLangEn });

            // ================= 2. Bottom Footer Banner =================
            _pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(36, 10, 36, 10)
            };

            _lblHelpText = new Label
            {
                Text = "📞 Quý khách cần hỗ trợ nhận phòng / check-in sớm? Vui lòng liên hệ trực tiếp quầy Lễ tân sảnh chính để được phục vụ chu đáo.",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(36, 18)
            };

            // Nút thoát Kiosk bí mật dành cho nhân viên (chỉ hiện ổ khóa nhỏ góc phải)
            _btnThoatKiosk = new Button
            {
                Text = "🔒",
                Size = new Size(42, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = AppTheme.TextMuted,
                Font = new Font(AppTheme.DefaultFontFamily, 12F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnThoatKiosk.FlatAppearance.BorderSize = 0;
            _btnThoatKiosk.Location = new Point(_pnlFooter.Width - 60, 11);
            _btnMoChatbot = new Button
            {
                Text = "💬  Trợ lý ảo tư vấn phòng",
                Size = new Size(250, 40),
                Font = AppTheme.FontBodyBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(_btnMoChatbot);

            void CanChinhFooter()
            {
                _btnThoatKiosk.Location = new Point(_pnlFooter.ClientSize.Width - 60, 11);
                _btnMoChatbot.Location = new Point(_btnThoatKiosk.Left - _btnMoChatbot.Width - 16, 10);
            }

            _pnlFooter.Resize += (s, e) => CanChinhFooter();
            CanChinhFooter();

            _pnlFooter.Controls.AddRange(new Control[] { _lblHelpText, _btnMoChatbot, _btnThoatKiosk });

            // ================= 3. Main Center Area (TableLayoutPanel 2 Columns) =================
            var pnlContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(36, 24, 36, 20),
                BackColor = AppTheme.Background
            };

            _tlpMain = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); // Cột trái: Nhập liệu
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F)); // Cột phải: Kết quả

            // ---------------- 3.1 Cột Trái: Thẻ Nhập Liệu Tra Cứu ----------------
            _pnlSearchCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(32, 28, 32, 28),
                Margin = new Padding(0, 0, 16, 0)
            };
            AppTheme.ApplyCardPanel(_pnlSearchCard);

            _lblSearchTitle = new Label
            {
                Text = "1. XÁC THỰC THÔNG TIN ĐẶT PHÒNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 36
            };

            _lblSearchSub = new Label
            {
                Text = "Vì lý do bảo mật quyền riêng tư của Quý khách, vui lòng nhập chính xác cả Mã Booking và Số điện thoại đăng ký:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Top,
                Height = 44
            };

            var pnlInputGroup = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            _lblMaBookingTitle = new Label
            {
                Text = "MÃ ĐẶT PHÒNG (BOOKING CODE):",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(0, 12),
                AutoSize = true
            };

            _txtMaBooking = new TextBox
            {
                Location = new Point(0, 40),
                Width = 420,
                Font = new Font(AppTheme.DefaultFontFamily, 15F, FontStyle.Bold),
                CharacterCasing = CharacterCasing.Upper,
                PlaceholderText = "VD: BK261030134850"
            };

            _lblSdtTitle = new Label
            {
                Text = "SỐ ĐIỆN THOẠI ĐẶT PHÒNG:",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(0, 96),
                AutoSize = true
            };

            _txtSdt = new TextBox
            {
                Location = new Point(0, 124),
                Width = 420,
                Font = new Font(AppTheme.DefaultFontFamily, 15F, FontStyle.Bold),
                PlaceholderText = "VD: 0987654321"
            };

            _btnTraCuu = new Button
            {
                Text = "🔍  TRA CỨU THÔNG TIN PHÒNG",
                Location = new Point(0, 185),
                Width = 420,
                Height = 54,
                Font = new Font(AppTheme.DefaultFontFamily, 12F, FontStyle.Bold)
            };
            AppTheme.ApplyPrimaryButton(_btnTraCuu);

            _btnXoaTrang = new Button
            {
                Text = "🔄  Nhập lại thông tin",
                Location = new Point(0, 250),
                Width = 420,
                Height = 42,
                Font = AppTheme.FontBody
            };
            AppTheme.ApplySecondaryButton(_btnXoaTrang);

            // Banner thông báo lỗi / trạng thái
            _pnlThongBao = new Panel
            {
                Location = new Point(0, 305),
                Width = 420,
                Height = 65,
                BackColor = AppTheme.StatusDangerBg,
                Padding = new Padding(12),
                Visible = false
            };
            AppTheme.ApplyCardPanel(_pnlThongBao);

            _lblThongBao = new Label
            {
                Text = "",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _pnlThongBao.Controls.Add(_lblThongBao);

            pnlInputGroup.Controls.AddRange(new Control[]
            {
                _lblMaBookingTitle, _txtMaBooking,
                _lblSdtTitle, _txtSdt,
                _btnTraCuu, _btnXoaTrang,
                _pnlThongBao
            });

            _pnlSearchCard.Controls.Add(pnlInputGroup);
            _pnlSearchCard.Controls.Add(_lblSearchSub);
            _pnlSearchCard.Controls.Add(_lblSearchTitle);

            // ---------------- 3.2 Cột Phải: Thẻ Hiển Thị Kết Quả ----------------
            _pnlResultCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(32, 28, 32, 28),
                Margin = new Padding(16, 0, 0, 0)
            };
            AppTheme.ApplyCardPanel(_pnlResultCard);

            _lblResultTitle = new Label
            {
                Text = "2. THÔNG TIN PHÒNG NGHỈ CỦA BẠN",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 36
            };

            // Trạng thái chờ: Placeholder khi chưa tra cứu
            _pnlResultPlaceholder = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            _lblPlaceholderIcon = new Label
            {
                Text = "🏨",
                Font = new Font(AppTheme.DefaultFontFamily, 48F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 120
            };

            _lblPlaceholderText = new Label
            {
                Text = "Vui lòng nhập Mã Booking và Số điện thoại bên trái rồi bấm nút 'TRA CỨU THÔNG TIN'.\r\n\r\n🔒 Vì an toàn thông tin, hệ thống chỉ hiển thị chi tiết khi cả hai thông tin trùng khớp hoàn toàn.",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 0, 24, 0)
            };

            _pnlResultPlaceholder.Controls.Add(_lblPlaceholderText);
            _pnlResultPlaceholder.Controls.Add(_lblPlaceholderIcon);

            // Trạng thái thành công: Panel chi tiết kết quả
            _pnlResultContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Visible = false
            };

            _lblResMaBooking = new Label
            {
                Text = "MÃ BOOKING: BK...",
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(0, 10),
                AutoSize = true
            };

            _lblResTrangThai = new Label
            {
                Text = "ĐÃ XÁC NHẬN",
                Font = AppTheme.FontSmallBold,
                BackColor = AppTheme.StatusSuccessBg,
                ForeColor = AppTheme.StatusSuccessText,
                Padding = new Padding(10, 4, 10, 4),
                Location = new Point(340, 8),
                AutoSize = true
            };

            _lblResKhachHang = new Label
            {
                Text = "Quý khách: Nguyễn Văn A",
                Font = new Font(AppTheme.DefaultFontFamily, 12F, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(0, 48),
                AutoSize = true
            };

            _lblResThoiGian = new Label
            {
                Text = "Thời gian lưu trú: 10/10/2026 ➔ 12/10/2026 (2 đêm)",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(0, 80),
                AutoSize = true
            };

            _lblResSoKhach = new Label
            {
                Text = "Số lượng khách: 2 người",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(0, 106),
                AutoSize = true
            };

            var lblPhongTitle = new Label
            {
                Text = "DANH SÁCH PHÒNG ĐÃ ĐẶT:",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(0, 140),
                AutoSize = true
            };

            _lstPhong = new ListBox
            {
                Location = new Point(0, 166),
                Width = 520,
                Height = 100,
                Font = AppTheme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                ItemHeight = 24
            };

            _lblResDichVu = new Label
            {
                Text = "Dịch vụ đi kèm: (Không có)",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(0, 276),
                AutoSize = true
            };

            _lblResThanhToan = new Label
            {
                Text = "Thanh toán: Đã thanh toán (1.500.000 đ)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.StatusSuccessText,
                Location = new Point(0, 306),
                AutoSize = true
            };

            _btnXoaKetQua = new Button
            {
                Text = "❌  Xóa thông tin & Hoàn tất tra cứu",
                Location = new Point(0, 350),
                Width = 340,
                Height = 44,
                Font = AppTheme.FontBodyBold
            };
            AppTheme.ApplySecondaryButton(_btnXoaKetQua);

            _lblAutoClearCount = new Label
            {
                Text = "⏱️ Tự động xóa sau 60 giây",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(350, 362),
                AutoSize = true
            };

            _pnlResultContent.Controls.AddRange(new Control[]
            {
                _lblResMaBooking, _lblResTrangThai,
                _lblResKhachHang, _lblResThoiGian, _lblResSoKhach,
                lblPhongTitle, _lstPhong,
                _lblResDichVu, _lblResThanhToan,
                _btnXoaKetQua, _lblAutoClearCount
            });

            _pnlResultCard.Controls.Add(_pnlResultContent);
            _pnlResultCard.Controls.Add(_pnlResultPlaceholder);
            _pnlResultCard.Controls.Add(_lblResultTitle);

            // Ghép 2 cột vào TableLayoutPanel
            _tlpMain.Controls.Add(_pnlSearchCard, 0, 0);
            _tlpMain.Controls.Add(_pnlResultCard, 1, 0);

            // ================= 3.3 Tab chính: Tra cứu booking | Giới thiệu khách sạn (DCKLTTQ-103) =================
            _tabMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.FontBodyBold,
                SizeMode = TabSizeMode.Fixed,
                ItemSize = new Size(230, 36)
            };

            _tabTraCuu = new TabPage
            {
                Text = "🔍  Tra cứu đặt phòng",
                BackColor = AppTheme.Background,
                Padding = new Padding(10, 10, 10, 0)
            };

            _tabGioiThieu = new TabPage
            {
                Text = "🏨  Giới thiệu khách sạn",
                BackColor = AppTheme.Background,
                Padding = new Padding(0)
            };

            // --- 3.3.1 Tab Giới thiệu: thẻ nội dung bên trái ---
            var tlpGioiThieu = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(16, 16, 16, 16)
            };
            tlpGioiThieu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            tlpGioiThieu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));

            _pnlIntroCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(30, 24, 30, 24),
                Margin = new Padding(0, 0, 12, 0)
            };
            AppTheme.ApplyCardPanel(_pnlIntroCard);

            _lblIntroTitle = new Label
            {
                Text = "VỀ KHÁCH SẠN",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 32,
                UseMnemonic = false
            };

            _lblIntroContent = new Label
            {
                Text = "Resort & Hotel Management là khách sạn nghỉ dưỡng tọa lạc tại trung tâm thành phố, mang đến không gian yên tĩnh và dịch vụ tận tâm. Khách sạn có hệ thống phòng nghỉ tiện nghi cùng nhiều tiện ích như hồ bơi, nhà hàng và spa, phù hợp cho cả kỳ nghỉ gia đình lẫn chuyến công tác.",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Top,
                Height = 90,
                UseMnemonic = false
            };

            var pnlIntroGap1 = new Panel { Dock = DockStyle.Top, Height = 14, BackColor = Color.Transparent };

            _lblIntroHoursTitle = new Label
            {
                Text = "GIỜ NHẬN & TRẢ PHÒNG",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 24,
                UseMnemonic = false
            };

            _lblIntroHours = new Label
            {
                Text = "Nhận phòng từ 14:00 — Trả phòng trước 12:00",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 28,
                UseMnemonic = false
            };

            var pnlIntroGap2 = new Panel { Dock = DockStyle.Top, Height = 20, BackColor = Color.Transparent };

            _lblIntroContactTitle = new Label
            {
                Text = "LIÊN HỆ LỄ TÂN",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 24,
                UseMnemonic = false
            };

            _lblIntroContact = new Label
            {
                Text = "Hotline: 1900 6868 — Phục vụ 24/7",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 28,
                UseMnemonic = false
            };

            var pnlIntroLogo = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                BackColor = Color.Transparent
            };
            var picIntroLogo = new PictureBox
            {
                Image = EmbeddedImages.Logo,
                Size = new Size(110, 110),
                Location = new Point(0, 36),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            pnlIntroLogo.Controls.Add(picIntroLogo);

            _pnlIntroCard.Controls.AddRange(new Control[]
            {
                pnlIntroLogo,
                _lblIntroContact,
                _lblIntroContactTitle,
                pnlIntroGap2,
                _lblIntroHours,
                _lblIntroHoursTitle,
                pnlIntroGap1,
                _lblIntroContent,
                _lblIntroTitle
            });

            // --- 3.3.2 Tab Giới thiệu: thẻ danh sách dịch vụ bên phải ---
            _pnlDichVuCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(20, 20, 20, 20),
                Margin = new Padding(12, 0, 0, 0)
            };
            AppTheme.ApplyCardPanel(_pnlDichVuCard);

            _lblDichVuTitle = new Label
            {
                Text = "DỊCH VỤ & TIỆN ÍCH",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 32,
                UseMnemonic = false
            };

            _lblDichVuTrangThai = new Label
            {
                Text = "Đang tải danh sách dịch vụ...",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                Dock = DockStyle.Top,
                Height = 24,
                Visible = false,
                UseMnemonic = false
            };

            _flpDichVu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = AppTheme.Surface,
                Padding = new Padding(0, 10, 0, 0)
            };

            _pnlDichVuCard.Controls.AddRange(new Control[]
            {
                _flpDichVu,
                _lblDichVuTrangThai,
                _lblDichVuTitle
            });

            tlpGioiThieu.Controls.Add(_pnlIntroCard, 0, 0);
            tlpGioiThieu.Controls.Add(_pnlDichVuCard, 1, 0);

            _tabTraCuu.Controls.Add(_tlpMain);
            _tabGioiThieu.Controls.Add(tlpGioiThieu);
            _tabMain.TabPages.Add(_tabTraCuu);
            _tabMain.TabPages.Add(_tabGioiThieu);
            _tabMain.SelectedIndex = 0;

            // ================= 4. Khung Chatbot AI Drawer (DCKLTTQ-88) =================
            _pnlChatDrawer = new Panel
            {
                Width = 470,
                Dock = DockStyle.Right,
                Visible = false,
                BackColor = AppTheme.Surface,
                Padding = new Padding(16, 12, 16, 12)
            };
            AppTheme.ApplyCardPanel(_pnlChatDrawer);

            // 4.1 Header của Drawer
            _pnlChatHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.Transparent
            };

            _lblChatBotIcon = new Label
            {
                Text = "🤖",
                Font = new Font("Segoe UI Emoji", 16F, FontStyle.Regular),
                Location = new Point(4, 8),
                AutoSize = true
            };

            _lblChatBotTitle = new Label
            {
                Text = "TRỢ LÝ ẢO KIOSK",
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(44, 8),
                AutoSize = true
            };

            _lblChatBotSub = new Label
            {
                Text = "Tư vấn phòng & chính sách khách sạn",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(45, 30),
                AutoSize = true
            };

            _btnChatReset = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(96, 32),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnChatReset);

            _btnChatClose = new Button
            {
                Text = "✕",
                Size = new Size(36, 32),
                Font = new Font(AppTheme.DefaultFontFamily, 11F, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnChatClose.FlatAppearance.BorderSize = 0;

            void CanChinhChatHeader()
            {
                _btnChatClose.Location = new Point(_pnlChatHeader.ClientSize.Width - 40, 10);
                _btnChatReset.Location = new Point(_btnChatClose.Left - 104, 10);
            }
            _pnlChatHeader.Resize += (s, e) => CanChinhChatHeader();
            CanChinhChatHeader();

            _pnlChatHeader.Controls.AddRange(new Control[]
            {
                _lblChatBotIcon, _lblChatBotTitle, _lblChatBotSub,
                _btnChatReset, _btnChatClose
            });

            // 4.2 Bottom Input & Gợi ý câu hỏi nhanh
            _pnlChatBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 142,
                BackColor = Color.Transparent
            };

            _lblChatStatus = new Label
            {
                Text = "⏳ Trợ lý ảo đang phản hồi...",
                Dock = DockStyle.Top,
                Height = 20,
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.Primary,
                Visible = false
            };

            _flpSuggestions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 66,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 4, 0, 4),
                BackColor = Color.Transparent
            };

            Button TaoChip(string text)
            {
                var btn = new Button
                {
                    Text = text,
                    Height = 28,
                    AutoSize = true,
                    Font = AppTheme.FontSmall,
                    Cursor = Cursors.Hand,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppTheme.SurfaceSecondary,
                    ForeColor = AppTheme.TextPrimary,
                    Margin = new Padding(0, 0, 6, 4)
                };
                btn.FlatAppearance.BorderColor = AppTheme.Border;
                btn.FlatAppearance.BorderSize = 1;
                return btn;
            }

            _btnChipGiaPhong = TaoChip("💰 Giá phòng");
            _btnChipCheckIn = TaoChip("⏰ Giờ check-in/out");
            _btnChipPhongTrong = TaoChip("📅 Còn phòng không?");
            _btnChipChinhSachHuy = TaoChip("❌ Chính sách hủy");

            _flpSuggestions.Controls.AddRange(new Control[]
            {
                _btnChipGiaPhong, _btnChipCheckIn,
                _btnChipPhongTrong, _btnChipChinhSachHuy
            });

            _pnlChatInputRow = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = Color.Transparent
            };

            _txtChatInput = new TextBox
            {
                Font = AppTheme.FontBody,
                PlaceholderText = "Nhập câu hỏi của quý khách...",
                Location = new Point(0, 4),
                Width = 350,
                Height = 36,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            _btnChatSend = new Button
            {
                Text = "Gửi ➤",
                Size = new Size(82, 34),
                Font = AppTheme.FontSmallBold,
                Location = new Point(356, 4),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(_btnChatSend);

            void CanChinhInputRow()
            {
                _btnChatSend.Location = new Point(_pnlChatInputRow.ClientSize.Width - 84, 4);
                _txtChatInput.Width = _btnChatSend.Left - 8;
            }
            _pnlChatInputRow.Resize += (s, e) => CanChinhInputRow();
            CanChinhInputRow();

            _pnlChatInputRow.Controls.AddRange(new Control[] { _txtChatInput, _btnChatSend });

            _pnlChatBottom.Controls.AddRange(new Control[]
            {
                _pnlChatInputRow,
                _flpSuggestions,
                _lblChatStatus
            });

            // 4.3 Khung cuộn tin nhắn
            _flpChatMessages = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = AppTheme.SurfaceAlt,
                Padding = new Padding(8, 8, 8, 8)
            };

            _pnlChatDrawer.Controls.Add(_flpChatMessages);
            _pnlChatDrawer.Controls.Add(_pnlChatBottom);
            _pnlChatDrawer.Controls.Add(_pnlChatHeader);

            pnlContainer.Controls.Add(_tabMain);
            pnlContainer.Controls.Add(_pnlChatDrawer);

            // ================= Ráp toàn bộ Form =================
            Controls.AddRange(new Control[]
            {
                pnlContainer,
                _pnlFooter,
                _pnlHeader
            });
        }
    }
}

