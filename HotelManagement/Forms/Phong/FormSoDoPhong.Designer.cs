using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormSoDoPhong
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _debounceTimer?.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // Panels
        private Panel _pnlTop = null!;
        private FlowLayoutPanel _pnlThongKe = null!;
        private Panel _pnlLeftFilter = null!;
        private FlowLayoutPanel _flpLeftContent = null!;
        private Panel _pnlRight = null!;
        private Panel _pnlCanvasContainer = null!;
        private RoomMapCanvas _mapCanvas = null!;

        // Controls Thống kê
        private Label _lblStatTong = null!;
        private Label _lblStatTrong = null!;
        private Label _lblStatDangO = null!;
        private Label _lblStatDangDon = null!;
        private Label _lblStatBaoTri = null!;
        private Label _lblStatTyLe = null!;
        private Button _btnLamMoi = null!;

        // Controls Bộ lọc bên trái (Phần A)
        private Label _lblFilterHeader = null!;
        private Label _lblTimKiemHeader = null!;
        private TextBox _txtTimKiem = null!;

        private GroupBox _grpLocTrangThai = null!;
        private FlowLayoutPanel _flpTrangThai = null!;
        private RadioButton _radStatusAll = null!;
        private RadioButton _radStatusTrong = null!;
        private RadioButton _radStatusDangO = null!;
        private RadioButton _radStatusDangDon = null!;
        private RadioButton _radStatusBaoTri = null!;

        private GroupBox _grpLocLoaiPhong = null!;
        private FlowLayoutPanel _flpLoaiPhong = null!;
        private RadioButton _radLoaiPhongAll = null!;

        private GroupBox _grpCheDoXem = null!;
        private FlowLayoutPanel _flpCheDoXem = null!;
        private RadioButton _radXemTheoTang = null!;
        private RadioButton _radXemLuoi = null!;

        private Button _btnDatLaiBoLoc = null!;

        // Controls Sidebar Chi tiết & Thao tác nhanh
        private Label _lblChiTietHeader = null!;
        private Panel _pnlCardPhong = null!;
        private Label _lblPhongSoPhong = null!;
        private Label _lblPhongLoaiPhong = null!;
        private Label _lblPhongTrangThaiBadge = null!;
        private Label _lblThongTinPhu = null!;
        private Button _btnThaoTacChinh = null!;
        private Button _btnDoiSangTrong = null!;
        private Button _btnDoiSangDangDon = null!;
        private Button _btnDoiSangBaoTri = null!;
        private Button _btnMoFormPhong = null!;
        private GroupBox _grpChuThich = null!;
        private Label _lblHuongDanThaoTac = null!;

        // Context Menu
        private ContextMenuStrip _ctxMenuPhong = null!;
        private ToolStripMenuItem _mnuCtxCheckIn = null!;
        private ToolStripMenuItem _mnuCtxCheckOut = null!;
        private ToolStripMenuItem _mnuCtxDatPhong = null!;
        private ToolStripSeparator _mnuCtxSep1 = null!;
        private ToolStripMenuItem _mnuCtxDoiTrangThai = null!;
        private ToolStripMenuItem _mnuCtxSangTrong = null!;
        private ToolStripMenuItem _mnuCtxSangDangDon = null!;
        private ToolStripMenuItem _mnuCtxSangBaoTri = null!;
        private ToolStripSeparator _mnuCtxSep2 = null!;
        private ToolStripMenuItem _mnuCtxQuanLyPhong = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Form cấu hình
            AppTheme.ApplyFormStyle(this);
            this.Text = "Sơ đồ phòng trực quan - Khách sạn";
            this.Size = new Size(1360, 800);
            this.MinimumSize = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppTheme.Background;

            // ================= 1. Top Panel: Thanh Thống kê & Nút Làm mới =================
            _pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54,
                BackColor = AppTheme.Surface,
                Padding = new Padding(16, 8, 16, 8)
            };

            _pnlThongKe = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Height = 38,
                AutoSize = true,
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(0)
            };

            _lblStatTong = TaoLabelThongKe("Tổng: 0 phòng", AppTheme.TextPrimary, AppTheme.SurfaceSecondary);
            _lblStatTrong = TaoLabelThongKe("● Trống: 0", AppTheme.StatusSuccessText, AppTheme.StatusSuccessBg);
            _lblStatDangO = TaoLabelThongKe("● Đang ở: 0", AppTheme.StatusDangerText, AppTheme.StatusDangerBg);
            _lblStatDangDon = TaoLabelThongKe("● Đang dọn: 0", AppTheme.StatusWarningText, AppTheme.StatusWarningBg);
            _lblStatBaoTri = TaoLabelThongKe("● Bảo trì: 0", AppTheme.StatusNeutralText, AppTheme.StatusNeutralBg);
            _lblStatTyLe = TaoLabelThongKe("● Lấp đầy: 0%", AppTheme.Primary, AppTheme.PrimaryLight);

            _lblStatTong.Margin = new Padding(0, 2, 12, 2);
            _lblStatTrong.Margin = new Padding(0, 2, 12, 2);
            _lblStatDangO.Margin = new Padding(0, 2, 12, 2);
            _lblStatDangDon.Margin = new Padding(0, 2, 12, 2);
            _lblStatBaoTri.Margin = new Padding(0, 2, 12, 2);
            _lblStatTyLe.Margin = new Padding(0, 2, 0, 2);

            _pnlThongKe.Controls.AddRange(new Control[] {
                _lblStatTong, _lblStatTrong, _lblStatDangO, _lblStatDangDon, _lblStatBaoTri, _lblStatTyLe
            });

            _btnLamMoi = new Button
            {
                Text = "🔄 Làm mới (F5)",
                Width = 140,
                Height = 36,
                Dock = DockStyle.Right,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnLamMoi);

            _pnlTop.Controls.Add(_pnlThongKe);
            _pnlTop.Controls.Add(_btnLamMoi);

            // ================= 2. Left Filter Panel: Bộ lọc & Tìm kiếm (Phần A) =================
            _pnlLeftFilter = new Panel
            {
                Dock = DockStyle.Left,
                Width = 245,
                BackColor = AppTheme.Surface,
                Padding = new Padding(14, 12, 14, 12),
                AutoScroll = true
            };

            _flpLeftContent = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            int itemW = 215;

            _lblFilterHeader = new Label
            {
                Text = "🔍 BỘ LỌC PHÒNG",
                Width = itemW,
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                Margin = new Padding(0, 0, 0, 8),
                AutoSize = false,
                Height = 22
            };

            _lblTimKiemHeader = new Label
            {
                Text = "Tìm kiếm số phòng / loại:",
                Width = itemW,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Margin = new Padding(0, 4, 0, 4),
                AutoSize = true
            };

            _txtTimKiem = new TextBox
            {
                Width = itemW,
                PlaceholderText = "Nhập mã phòng, loại...",
                Margin = new Padding(0, 0, 0, 10)
            };
            AppTheme.ApplyTextBox(_txtTimKiem);

            // 2.1 GroupBox lọc theo trạng thái
            _grpLocTrangThai = new GroupBox
            {
                Text = "Trạng thái phòng",
                Width = itemW,
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10, 8, 10, 8)
            };

            _flpTrangThai = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            _radStatusAll = TaoRadioButton("Tất cả trạng thái", true, AppTheme.TextPrimary);
            _radStatusTrong = TaoRadioButton("● Trống (Sẵn sàng)", false, AppTheme.StatusSuccessText);
            _radStatusDangO = TaoRadioButton("● Đang ở", false, AppTheme.StatusDangerText);
            _radStatusDangDon = TaoRadioButton("● Đang dọn", false, AppTheme.StatusWarningText);
            _radStatusBaoTri = TaoRadioButton("● Bảo trì", false, AppTheme.StatusNeutralText);

            _flpTrangThai.Controls.AddRange(new Control[] {
                _radStatusAll, _radStatusTrong, _radStatusDangO, _radStatusDangDon, _radStatusBaoTri
            });
            _grpLocTrangThai.Controls.Add(_flpTrangThai);

            // 2.2 GroupBox lọc theo loại phòng (Động từ DB)
            _grpLocLoaiPhong = new GroupBox
            {
                Text = "Loại phòng",
                Width = itemW,
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10, 8, 10, 8)
            };

            _flpLoaiPhong = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            _radLoaiPhongAll = TaoRadioButton("Tất cả loại phòng", true, AppTheme.TextPrimary);
            _flpLoaiPhong.Controls.Add(_radLoaiPhongAll);
            _grpLocLoaiPhong.Controls.Add(_flpLoaiPhong);

            // 2.3 GroupBox Chế độ hiển thị
            _grpCheDoXem = new GroupBox
            {
                Text = "Chế độ hiển thị",
                Width = itemW,
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10, 8, 10, 8)
            };

            _flpCheDoXem = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            _radXemTheoTang = TaoRadioButton("Gom nhóm theo Tầng", true, AppTheme.TextPrimary);
            _radXemLuoi = TaoRadioButton("Lưới phẳng tổng thể", false, AppTheme.TextPrimary);
            _flpCheDoXem.Controls.AddRange(new Control[] { _radXemTheoTang, _radXemLuoi });
            _grpCheDoXem.Controls.Add(_flpCheDoXem);

            // 2.4 Nút Đặt lại bộ lọc
            _btnDatLaiBoLoc = new Button
            {
                Text = "✖ Đặt lại bộ lọc",
                Width = itemW,
                Height = 32,
                Margin = new Padding(0, 4, 0, 10),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnDatLaiBoLoc);

            _flpLeftContent.Controls.AddRange(new Control[] {
                _lblFilterHeader,
                _lblTimKiemHeader,
                _txtTimKiem,
                _grpLocTrangThai,
                _grpLocLoaiPhong,
                _grpCheDoXem,
                _btnDatLaiBoLoc
            });

            _pnlLeftFilter.Controls.Add(_flpLeftContent);

            // ================= 3. Right Sidebar Panel: Thông tin phòng & Thao tác nhanh =================
            _pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 330,
                BackColor = AppTheme.Surface,
                Padding = new Padding(15)
            };

            _lblChiTietHeader = new Label
            {
                Text = "THÔNG TIN & THAO TÁC",
                UseMnemonic = false,
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(15, 15),
                AutoSize = true
            };

            // Card chi tiết phòng
            _pnlCardPhong = new Panel
            {
                Location = new Point(15, 45),
                Width = 295,
                Height = 140,
                BackColor = AppTheme.SurfaceAlt
            };

            _lblPhongSoPhong = new Label
            {
                Text = "Chưa chọn phòng",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(12, 10),
                AutoSize = true
            };

            _lblPhongTrangThaiBadge = new Label
            {
                Text = "--",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                BackColor = AppTheme.SurfaceSecondary,
                Location = new Point(15, 50),
                AutoSize = false,
                Size = new Size(110, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _lblPhongLoaiPhong = new Label
            {
                Text = "Loại phòng: --",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(15, 82),
                AutoSize = true
            };

            _lblThongTinPhu = new Label
            {
                Text = "Booking hiện tại: --",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(15, 108),
                AutoSize = true
            };

            _pnlCardPhong.Controls.AddRange(new Control[] {
                _lblPhongSoPhong, _lblPhongTrangThaiBadge, _lblPhongLoaiPhong, _lblThongTinPhu
            });

            // Nút thao tác chính (Context-aware button)
            _btnThaoTacChinh = new Button
            {
                Text = "🛎️ Thực hiện Check-out",
                Location = new Point(15, 200),
                Width = 295,
                Height = 42,
                Visible = false
            };
            AppTheme.ApplyDangerButton(_btnThaoTacChinh);

            // Nút chuyển trạng thái nhanh
            var lblNhanh = new Label
            {
                Text = "Chuyển nhanh trạng thái phòng:",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(15, 255),
                AutoSize = true
            };

            _btnDoiSangTrong = TaoButtonPhu("● Đánh dấu đã dọn / Sẵn sàng (Trống)", 280, AppTheme.StatusSuccessText, AppTheme.StatusSuccessBg, AppTheme.StatusSuccessBorder);
            _btnDoiSangDangDon = TaoButtonPhu("● Chuyển sang Đang dọn", 322, AppTheme.StatusWarningText, AppTheme.StatusWarningBg, AppTheme.StatusWarningBorder);
            _btnDoiSangBaoTri = TaoButtonPhu("● Chuyển sang Bảo trì phòng", 364, AppTheme.StatusNeutralText, AppTheme.StatusNeutralBg, AppTheme.StatusNeutralBorder);

            _btnMoFormPhong = new Button
            {
                Text = "⚙️ Mở danh mục Quản lý phòng",
                Location = new Point(15, 415),
                Width = 295,
                Height = 35,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnMoFormPhong);

            // GroupBox chú thích màu trạng thái (CK_Phong_TrangThai)
            _grpChuThich = new GroupBox
            {
                Text = "Chú thích 4 trạng thái (CK_Phong_TrangThai)",
                Location = new Point(15, 465),
                Width = 295,
                Height = 155,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary
            };

            var lblLegTrong = TaoItemChuThich("● Trống", "Phòng sẵn sàng đón khách mới", 24, AppTheme.StatusSuccessText);
            var lblLegDangO = TaoItemChuThich("● Đang ở", "Đang có khách lưu trú", 54, AppTheme.StatusDangerText);
            var lblLegDangDon = TaoItemChuThich("● Đang dọn", "Vừa check-out, đang vệ sinh", 84, AppTheme.StatusWarningText);
            var lblLegBaoTri = TaoItemChuThich("● Bảo trì", "Sửa chữa hoặc dừng phục vụ", 114, AppTheme.StatusNeutralText);

            _grpChuThich.Controls.AddRange(new Control[] { lblLegTrong, lblLegDangO, lblLegDangDon, lblLegBaoTri });

            _lblHuongDanThaoTac = new Label
            {
                Text = "💡 Mẹo: Nhấp đúp vào phòng để thao tác nhanh (Check-out hoặc Đặt phòng). Chuột phải để mở menu tác vụ.",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                Location = new Point(15, 630),
                Width = 295,
                Height = 45
            };

            _pnlRight.Controls.AddRange(new Control[] {
                _lblChiTietHeader, _pnlCardPhong,
                _btnThaoTacChinh, lblNhanh,
                _btnDoiSangTrong, _btnDoiSangDangDon, _btnDoiSangBaoTri,
                _btnMoFormPhong, _grpChuThich, _lblHuongDanThaoTac
            });

            // ================= 4. Center: GDI+ Room Map Canvas =================
            _pnlCanvasContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(12)
            };

            _mapCanvas = new RoomMapCanvas
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background
            };

            _pnlCanvasContainer.Controls.Add(_mapCanvas);

            // ================= 5. Context Menu Chuột Phải =================
            _ctxMenuPhong = new ContextMenuStrip(this.components);

            _mnuCtxCheckIn = new ToolStripMenuItem("🛎️ Xác nhận Check-in...");
            _mnuCtxCheckOut = new ToolStripMenuItem("🛎️ Thực hiện Check-out...");
            _mnuCtxDatPhong = new ToolStripMenuItem("📝 Tạo đặt phòng cho phòng này...");
            _mnuCtxSep1 = new ToolStripSeparator();

            _mnuCtxDoiTrangThai = new ToolStripMenuItem("🔄 Đổi trạng thái phòng");
            _mnuCtxSangTrong = new ToolStripMenuItem("🟢 Trống (Sẵn sàng)");
            _mnuCtxSangDangDon = new ToolStripMenuItem("🟠 Đang dọn");
            _mnuCtxSangBaoTri = new ToolStripMenuItem("⚪ Bảo trì");
            _mnuCtxDoiTrangThai.DropDownItems.AddRange(new ToolStripItem[] {
                _mnuCtxSangTrong, _mnuCtxSangDangDon, _mnuCtxSangBaoTri
            });

            _mnuCtxSep2 = new ToolStripSeparator();
            _mnuCtxQuanLyPhong = new ToolStripMenuItem("⚙️ Mở quản lý phòng");

            _ctxMenuPhong.Items.AddRange(new ToolStripItem[] {
                _mnuCtxCheckIn,
                _mnuCtxCheckOut,
                _mnuCtxDatPhong,
                _mnuCtxSep1,
                _mnuCtxDoiTrangThai,
                _mnuCtxSep2,
                _mnuCtxQuanLyPhong
            });

            // Ghép toàn bộ vào Form theo thứ tự Docking chuẩn WinForms
            this.Controls.Add(_pnlCanvasContainer);
            this.Controls.Add(_pnlRight);
            this.Controls.Add(_pnlLeftFilter);
            this.Controls.Add(_pnlTop);
        }

        private static Label TaoLabelThongKe(string text, Color foreColor, Color backColor)
        {
            var lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6),
                Font = AppTheme.FontSmallBold,
                ForeColor = foreColor,
                BackColor = backColor
            };
            return lbl;
        }

        private static RadioButton TaoRadioButton(string text, bool isChecked, Color foreColor)
        {
            return new RadioButton
            {
                Text = text,
                Checked = isChecked,
                ForeColor = foreColor,
                Font = AppTheme.FontBody,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Margin = new Padding(2, 3, 2, 3),
                BackColor = Color.Transparent
            };
        }

        private static Button TaoButtonPhu(string text, int top, Color foreColor, Color backColor, Color borderColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(15, top),
                Width = 295,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Visible = false
            };
            btn.FlatAppearance.BorderColor = borderColor;
            return btn;
        }

        private static Label TaoItemChuThich(string title, string desc, int top, Color color)
        {
            var lbl = new Label
            {
                Text = $"{title}: {desc}",
                Location = new Point(12, top),
                AutoSize = true,
                Font = AppTheme.FontSmall,
                ForeColor = color
            };
            return lbl;
        }

        #endregion
    }
}
