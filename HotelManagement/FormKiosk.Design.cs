using System.Drawing;
using System.Windows.Forms;

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

        // Bottom Footer
        private Panel _pnlFooter = null!;
        private Label _lblHelpText = null!;
        private Button _btnThoatKiosk = null!;

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
                Location = new Point(36, 16)
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
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _lblClock.Location = new Point(_pnlHeader.Width - 280, 28);
            _pnlHeader.Resize += (s, e) =>
            {
                _lblClock.Location = new Point(_pnlHeader.ClientSize.Width - _lblClock.Width - 36, 28);
            };

            _pnlHeader.Controls.AddRange(new Control[] { _lblHeaderHotel, _lblHeaderSub, _lblClock });

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
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnThoatKiosk.FlatAppearance.BorderSize = 0;
            _btnThoatKiosk.Location = new Point(_pnlFooter.Width - 60, 11);
            _pnlFooter.Resize += (s, e) =>
            {
                _btnThoatKiosk.Location = new Point(_pnlFooter.ClientSize.Width - 60, 11);
            };

            _pnlFooter.Controls.AddRange(new Control[] { _lblHelpText, _btnThoatKiosk });

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
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                PlaceholderText = "VD: 0987654321"
            };

            _btnTraCuu = new Button
            {
                Text = "🔍  TRA CỨU THÔNG TIN PHÒNG",
                Location = new Point(0, 185),
                Width = 420,
                Height = 54,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
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
                Font = new Font("Segoe UI", 48F),
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
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
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
            pnlContainer.Controls.Add(_tlpMain);

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

