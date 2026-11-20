using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HotelManagement.Common;
using HotelManagement.Localization;

namespace HotelManagement
{
    partial class Formchinh
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

        #region Windows Form Designer generated code

        private MenuStrip menuChinh = null!;
        private ToolStripMenuItem mnuQuanLyPhong = null!;
        private ToolStripMenuItem mnuPhong = null!;
        private ToolStripMenuItem mnuLoaiPhong = null!;
        private ToolStripMenuItem mnuDichVu = null!;
        private ToolStripMenuItem mnuSoDoPhongTop = null!;
        private ToolStripMenuItem mnuQuanLyDatPhong = null!;
        private ToolStripMenuItem mnuDatPhong = null!;
        private ToolStripMenuItem mnuCheckIn = null!;
        private ToolStripMenuItem mnuCheckOut = null!;
        private ToolStripMenuItem mnuThanhToan = null!;
        private ToolStripMenuItem mnuKhachHang = null!;
        private ToolStripMenuItem mnuQuanLyNhanSu = null!;   // chỉ Admin
        private ToolStripMenuItem mnuBaoCao = null!;         // chỉ Admin
        private ToolStripMenuItem mnuDashboard = null!;      // chỉ Admin
        private ToolStripMenuItem mnuImportPhong = null!;    // chỉ Admin
        private ToolStripMenuItem mnuLichSuThongBao = null!;
        private ToolStripMenuItem mnuMoKiosk = null!;        // DCKLTTQ-107 (Admin/LeTan mở Kiosk riêng)
        private ToolStripMenuItem mnuDoiMatKhau = null!;
        private ToolStripMenuItem mnuDangXuat = null!;

        // Controls giao diện bổ sung cho màn hình chính
        private Panel _pnlTopBar = null!;
        private Label _lblHotelTitle = null!;
        private Label _lblUserInfoBadge = null!;
        private ComboBox _cboLanguage = null!;
        private Panel _pnlMainContainer = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _lblStatusUser = null!;
        private ToolStripStatusLabel _lblStatusTime = null!;

        private Panel _pnlSidebar = null!;
        private Panel _pnlSidebarHeader = null!;
        private FlowLayoutPanel _pnlSidebarNav = null!;
        private PictureBox _picSidebarLogo = null!;
        private readonly List<Button> _sidebarNavButtons = new();

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Form cấu hình
            this.Text = "Hệ thống quản lý khách sạn";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1280, 800);
            this.MinimumSize = new Size(1024, 680);
            this.WindowState = FormWindowState.Maximized;
            AppTheme.ApplyFormStyle(this);

            // ================= 1. MenuStrip chính =================
            menuChinh = new MenuStrip
            {
                Dock = DockStyle.Top,
                BackColor = AppTheme.Surface,
                Font = AppTheme.FontBodyBold,
                Padding = new Padding(12, 6, 12, 6),
                Renderer = new ModernMenuRenderer()
            };

            // --- 1.1 Quản lý phòng ---
            mnuQuanLyPhong = new ToolStripMenuItem("Quản lý phòng");
            
            mnuPhong = new ToolStripMenuItem("Danh sách phòng");
            mnuPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuPhong);

            mnuLoaiPhong = new ToolStripMenuItem("Loại phòng");
            mnuLoaiPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormLoaiPhong();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuLoaiPhong);

            mnuDichVu = new ToolStripMenuItem("Quản lý dịch vụ");
            mnuDichVu.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDichVu();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuDichVu);

            mnuImportPhong = new ToolStripMenuItem("Import phòng từ Excel");
            mnuImportPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormImportPhong();
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuImportPhong);

            menuChinh.Items.Add(mnuQuanLyPhong);

            // --- 1.2 Sơ đồ phòng ---
            mnuSoDoPhongTop = new ToolStripMenuItem("Sơ đồ phòng");
            mnuSoDoPhongTop.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormSoDoPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuSoDoPhongTop);

            // --- 1.3 Quản lý đặt phòng ---
            mnuQuanLyDatPhong = new ToolStripMenuItem("Quản lý đặt phòng");

            mnuDatPhong = new ToolStripMenuItem("Đặt phòng mới");
            mnuDatPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDatPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuDatPhong);

            mnuCheckIn = new ToolStripMenuItem("Check-in (Nhận phòng)");
            mnuCheckIn.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormCheckIn(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuCheckIn);

            mnuCheckOut = new ToolStripMenuItem("Check-out (Trả phòng)");
            mnuCheckOut.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormCheckOut(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuCheckOut);

            mnuThanhToan = new ToolStripMenuItem("Thanh toán hóa đơn");
            mnuThanhToan.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormThanhToan();
                form.ShowDialog(this);
            };
            mnuQuanLyDatPhong.DropDownItems.Add(mnuThanhToan);

            menuChinh.Items.Add(mnuQuanLyDatPhong);

            // --- 1.4 Quản lý khách hàng ---
            mnuKhachHang = new ToolStripMenuItem("Quản lý khách hàng");
            mnuKhachHang.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormKhachHang(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuKhachHang);

            // --- 1.5 Quản lý nhân sự (Admin) ---
            mnuQuanLyNhanSu = new ToolStripMenuItem("Quản lý nhân sự");
            mnuQuanLyNhanSu.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormNhanSu();
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuQuanLyNhanSu);

            // --- 1.6 Báo cáo doanh thu (Admin) ---
            mnuBaoCao = new ToolStripMenuItem("Báo cáo doanh thu");
            mnuBaoCao.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormBaoCaoDoanhThu(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuBaoCao);

            // --- 1.7 Dashboard biểu đồ LiveCharts2 (Admin - DCKLTTQ-84) ---
            mnuDashboard = new ToolStripMenuItem("Dashboard thống kê");
            mnuDashboard.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDashboard();
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuDashboard);

            // --- 1.8 Lịch sử thông báo (DCKLTTQ-82) ---
            mnuLichSuThongBao = new ToolStripMenuItem("Lịch sử thông báo");
            mnuLichSuThongBao.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormLichSuThongBao();
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuLichSuThongBao);

            // --- 1.9 Mở chế độ Kiosk (DCKLTTQ-107 - Cửa sổ độc lập không chặn) ---
            mnuMoKiosk = new ToolStripMenuItem("🖥️ Mở chế độ Kiosk");
            mnuMoKiosk.Click += (_, _) =>
            {
                var formKiosk = new HotelManagement.Forms.FormKiosk();
                formKiosk.Show(); // Modeless - độc lập với FormChinh, có thể kéo sang màn hình thứ 2
            };
            menuChinh.Items.Add(mnuMoKiosk);

            // --- 1.10 Đổi mật khẩu ---
            mnuDoiMatKhau = new ToolStripMenuItem("Đổi mật khẩu");
            mnuDoiMatKhau.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormDoiMatKhau(_taiKhoanDangNhap.IdTaikhoan);
                form.ShowDialog(this);
            };
            menuChinh.Items.Add(mnuDoiMatKhau);

            // --- 1.8 Đăng xuất ---
            mnuDangXuat = new ToolStripMenuItem("Đăng xuất");
            mnuDangXuat.Click += MnuDangXuat_Click;
            menuChinh.Items.Add(mnuDangXuat);

            this.MainMenuStrip = menuChinh;

            // ================= 2. Top Bar: Header thương hiệu & thông tin tài khoản =================
            _pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = AppTheme.Surface,
                Padding = new Padding(24, 0, 24, 0)
            };
            _pnlTopBar.Paint += (s, e) =>
            {
                using var pen = new Pen(AppTheme.BorderLight, 1);
                e.Graphics.DrawLine(pen, 0, _pnlTopBar.Height - 1, _pnlTopBar.Width, _pnlTopBar.Height - 1);
            };

            _lblHotelTitle = new Label
            {
                Text = "RESORT & HOTEL MANAGEMENT SYSTEM",
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                AutoSize = true,
                Location = new Point(24, 22)
            };

            _lblUserInfoBadge = new Label
            {
                Text = $"👤 {_taiKhoanDangNhap.IdNhansuNavigation.Hoten} ({_taiKhoanDangNhap.IdVaitroNavigation.TenVaitro})",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                BackColor = AppTheme.PrimaryLight,
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _cboLanguage = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 145,
                Font = AppTheme.FontSmallBold,
                BackColor = AppTheme.Surface,
                ForeColor = AppTheme.TextPrimary,
                Cursor = Cursors.Hand
            };
            _cboLanguage.Items.AddRange(new object[] { "Tiếng Việt (VI)", "English (EN)" });
            _cboLanguage.SelectedIndex = HotelManagement.Localization.LanguageManager.CurrentLanguage == HotelManagement.Localization.AppLanguage.English ? 1 : 0;
            _cboLanguage.SelectedIndexChanged += (s, e) =>
            {
                if (!_isUpdatingLanguage)
                {
                    var targetLang = _cboLanguage.SelectedIndex == 1 ? HotelManagement.Localization.AppLanguage.English : HotelManagement.Localization.AppLanguage.Vietnamese;
                    HotelManagement.Localization.LanguageManager.SetLanguage(targetLang);
                }
            };

            _pnlTopBar.Controls.Add(_lblHotelTitle);
            _pnlTopBar.Controls.Add(_lblUserInfoBadge);
            _pnlTopBar.Controls.Add(_cboLanguage);

            void CanChinhTopBar()
            {
                if (_pnlTopBar == null || _cboLanguage == null || _lblUserInfoBadge == null) return;
                _cboLanguage.Location = new Point(_pnlTopBar.ClientSize.Width - _cboLanguage.Width - 24, 18);
                _lblUserInfoBadge.Location = new Point(_cboLanguage.Left - _lblUserInfoBadge.Width - 16, 16);
            }

            _pnlTopBar.Resize += (s, e) => CanChinhTopBar();
            CanChinhTopBar();

            // ================= 2b. Sidebar điều hướng =================
            _pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = AppTheme.Surface
            };

            _pnlSidebarHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 188,
                BackColor = AppTheme.Surface
            };

            _picSidebarLogo = new PictureBox
            {
                Image = EmbeddedImages.Logo,
                Size = new Size(96, 96),
                Location = new Point((_pnlSidebar.Width - 96) / 2, 16),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = AppTheme.Surface
            };

            var lblSidebarTen = new Label
            {
                Text = "Khách sạn DCKLTTQ",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(8, 118),
                Size = new Size(_pnlSidebar.Width - 16, 24)
            };

            var lblSidebarPhu = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(8, 144),
                Size = new Size(_pnlSidebar.Width - 16, 20)
            };

            _pnlSidebarHeader.Controls.AddRange(new Control[] { _picSidebarLogo, lblSidebarTen, lblSidebarPhu });

            _pnlSidebarNav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = AppTheme.Surface,
                Padding = new Padding(0, 8, 0, 12)
            };

            void ThemNhomSidebar(string tieuDe)
            {
                _pnlSidebarNav.Controls.Add(new Label
                {
                    Text = tieuDe,
                    Font = AppTheme.FontSmallBold,
                    ForeColor = AppTheme.TextMuted,
                    TextAlign = ContentAlignment.BottomLeft,
                    Size = new Size(208, 26),
                    Margin = new Padding(20, 10, 0, 2)
                });
            }

            void ThemNutSidebar(ToolStripMenuItem mucMenu)
            {
                var btn = new Button
                {
                    Text = mucMenu.Text,
                    Width = 208,
                    Height = 40,
                    Margin = new Padding(16, 2, 16, 2),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(12, 0, 0, 0),
                    Font = AppTheme.FontBody,
                    ForeColor = AppTheme.TextPrimary,
                    BackColor = AppTheme.Surface,
                    Cursor = Cursors.Hand,
                    UseVisualStyleBackColor = false
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = AppTheme.SurfaceSecondary;
                btn.FlatAppearance.MouseDownBackColor = AppTheme.SurfaceHover;

                mucMenu.TextChanged += (_, _) => btn.Text = mucMenu.Text;
                // Dùng Available thay vì Visible: mục con trong menu sổ xuống báo Visible=false
                // cho tới khi menu được mở lần đầu, còn Available vẫn đúng trạng thái ẩn/hiện.
                mucMenu.AvailableChanged += (_, _) => btn.Visible = mucMenu.Available;
                btn.Visible = mucMenu.Available;

                btn.Click += (_, _) =>
                {
                    DatNutSidebarDangChon(btn);
                    mucMenu.PerformClick();
                };

                _sidebarNavButtons.Add(btn);
                _pnlSidebarNav.Controls.Add(btn);
            }

            ThemNhomSidebar("ĐIỀU HƯỚNG");
            ThemNutSidebar(mnuDashboard);
            ThemNutSidebar(mnuSoDoPhongTop);

            ThemNhomSidebar("NGHIỆP VỤ");
            ThemNutSidebar(mnuDatPhong);
            ThemNutSidebar(mnuCheckIn);
            ThemNutSidebar(mnuCheckOut);
            ThemNutSidebar(mnuThanhToan);

            ThemNhomSidebar("DANH MỤC");
            ThemNutSidebar(mnuPhong);
            ThemNutSidebar(mnuLoaiPhong);
            ThemNutSidebar(mnuDichVu);
            ThemNutSidebar(mnuKhachHang);
            ThemNutSidebar(mnuQuanLyNhanSu);
            ThemNutSidebar(mnuImportPhong);
            ThemNutSidebar(mnuBaoCao);
            ThemNutSidebar(mnuLichSuThongBao);

            ThemNhomSidebar("HỆ THỐNG");
            ThemNutSidebar(mnuDoiMatKhau);
            ThemNutSidebar(mnuDangXuat);

            var pnlSidebarBorder = new Panel
            {
                Dock = DockStyle.Right,
                Width = 1,
                BackColor = AppTheme.BorderLight
            };

            _pnlSidebar.Controls.Add(_pnlSidebarNav);
            _pnlSidebar.Controls.Add(_pnlSidebarHeader);
            _pnlSidebar.Controls.Add(pnlSidebarBorder);

            // ================= 3. Vùng nội dung: ảnh nền phủ kín =================
            // Panel chỉ chiếm phần còn lại sau MenuStrip/TopBar/Sidebar/StatusStrip
            // nên ảnh nền không thể tràn lên các vùng đó.
            _pnlMainContainer = new BackgroundPanel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background
            };

            // ================= 4. Bottom StatusStrip =================
            _statusStrip = new StatusStrip
            {
                // TableLayout mặc định tính sai bề rộng Spring ở DPI 125%, đẩy nhãn giờ
                // ra ngoài khung; xếp ngang và ghim phải cho kết quả ổn định.
                LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow,
                BackColor = AppTheme.Surface,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Padding = new Padding(12, 4, 12, 4)
            };

            _lblStatusUser = new ToolStripStatusLabel
            {
                Text = $"Hệ thống đang hoạt động • Người dùng: {_taiKhoanDangNhap.TenDangnhap} ({_taiKhoanDangNhap.IdVaitroNavigation.TenVaitro})",
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblStatusTime = new ToolStripStatusLabel
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleRight
            };

            _statusStrip.Items.AddRange(new ToolStripItem[] { _lblStatusUser, _lblStatusTime });

            // Ráp vào Form (thứ tự dock: Fill -> TopBar -> Sidebar -> Menu -> Status)
            this.Controls.Add(_pnlMainContainer);
            this.Controls.Add(_pnlTopBar);
            this.Controls.Add(_pnlSidebar);
            this.Controls.Add(menuChinh);
            this.Controls.Add(_statusStrip);
        }

        private void DatNutSidebarDangChon(Button nutDuocChon)
        {
            foreach (var btn in _sidebarNavButtons)
            {
                bool dangChon = ReferenceEquals(btn, nutDuocChon);
                btn.BackColor = dangChon ? AppTheme.PrimaryLight : AppTheme.Surface;
                btn.ForeColor = dangChon ? AppTheme.Primary : AppTheme.TextPrimary;
                btn.Font = dangChon ? AppTheme.FontBodyBold : AppTheme.FontBody;
                btn.FlatAppearance.MouseOverBackColor = dangChon ? AppTheme.PrimaryLight : AppTheme.SurfaceSecondary;
            }
        }

        #endregion

        /// <summary>
        /// Panel nền phủ kín vùng nội dung: ảnh được phóng theo tỷ lệ gốc và cắt phần dư,
        /// không kéo giãn, không vẽ ra ngoài biên panel.
        /// </summary>
        private sealed class BackgroundPanel : Panel
        {
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                base.OnPaintBackground(e);

                Size kichThuoc = ClientSize;
                if (kichThuoc.Width < 1 || kichThuoc.Height < 1)
                    return;

                Image anh = EmbeddedImages.Background;
                float tyLe = Math.Max((float)kichThuoc.Width / anh.Width, (float)kichThuoc.Height / anh.Height);
                int rong = (int)Math.Ceiling(anh.Width * tyLe);
                int cao = (int)Math.Ceiling(anh.Height * tyLe);
                var dich = new Rectangle((kichThuoc.Width - rong) / 2, (kichThuoc.Height - cao) / 2, rong, cao);

                var cheDoCu = e.Graphics.InterpolationMode;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(anh, dich);
                e.Graphics.InterpolationMode = cheDoCu;
            }
        }

        /// <summary>
        /// Renderer menu phẳng thanh lịch theo theme đại dương.
        /// </summary>
        private class ModernMenuRenderer : ToolStripProfessionalRenderer
        {
            public ModernMenuRenderer() : base(new ModernColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                    using var brush = new SolidBrush(AppTheme.PrimaryLight);
                    e.Graphics.FillRectangle(brush, rect);
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
        }

        private class ModernColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => AppTheme.Surface;
            public override Color MenuStripGradientEnd => AppTheme.Surface;
            public override Color MenuItemSelected => AppTheme.PrimaryLight;
            public override Color MenuItemSelectedGradientBegin => AppTheme.PrimaryLight;
            public override Color MenuItemSelectedGradientEnd => AppTheme.PrimaryLight;
            public override Color MenuItemPressedGradientBegin => AppTheme.PrimaryLight;
            public override Color MenuItemPressedGradientEnd => AppTheme.PrimaryLight;
            public override Color ToolStripDropDownBackground => AppTheme.Surface;
            public override Color ImageMarginGradientBegin => AppTheme.Surface;
            public override Color ImageMarginGradientMiddle => AppTheme.Surface;
            public override Color ImageMarginGradientEnd => AppTheme.Surface;
            public override Color MenuBorder => AppTheme.Border;
            public override Color MenuItemBorder => AppTheme.Border;
        }
    }
}
