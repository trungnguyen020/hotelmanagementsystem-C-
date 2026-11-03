using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
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
        private ToolStripMenuItem mnuDoiMatKhau = null!;
        private ToolStripMenuItem mnuDangXuat = null!;

        // Controls giao diện bổ sung cho màn hình chính
        private Panel _pnlTopBar = null!;
        private Label _lblHotelTitle = null!;
        private Label _lblUserInfoBadge = null!;
        private ComboBox _cboLanguage = null!;
        private Panel _pnlMainContainer = null!;
        private Label _lblDashboardTitle = null!;
        private Label _lblDashboardSub = null!;
        private FlowLayoutPanel _pnlQuickActions = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _lblStatusUser = null!;
        private ToolStripStatusLabel _lblStatusTime = null!;

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
                using var form = new HotelManagement.Forms.FormKhachHang();
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

            // --- 1.8 Đổi mật khẩu ---
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
            _cboLanguage.Items.AddRange(new object[] { "🇻🇳  Tiếng Việt", "🇬🇧  English" });
            _cboLanguage.SelectedIndex = LanguageManager.CurrentLanguage == AppLanguage.English ? 1 : 0;
            _cboLanguage.SelectedIndexChanged += (s, e) =>
            {
                if (!_isUpdatingLanguage)
                {
                    var targetLang = _cboLanguage.SelectedIndex == 1 ? AppLanguage.English : AppLanguage.Vietnamese;
                    LanguageManager.SetLanguage(targetLang);
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

            // ================= 3. Main Dashboard: Các thẻ thao tác nhanh =================
            _pnlMainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                AutoScroll = true,
                Padding = new Padding(36, 28, 36, 28)
            };

            _lblDashboardTitle = new Label
            {
                Text = "BẢNG ĐIỀU KHIỂN & TÁC VỤ NHANH",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(36, 18)
            };

            _lblDashboardSub = new Label
            {
                Text = "Chọn nhanh các phân hệ nghiệp vụ khách sạn thường dùng hàng ngày",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(36, 52)
            };

            _pnlQuickActions = new FlowLayoutPanel
            {
                Location = new Point(36, 88),
                Size = new Size(1200, 520),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.Transparent,
                AutoScroll = true,
                WrapContents = true
            };

            // Co giãn theo bề rộng vùng nội dung; không neo phải để tránh vòng lặp
            // kích thước với AutoScroll của panel cha.
            _pnlMainContainer.Resize += (s, e) =>
            {
                int width = _pnlMainContainer.ClientSize.Width - 72;
                if (width > 0 && _pnlQuickActions.Width != width)
                    _pnlQuickActions.Width = width;
            };

            TaiLaiTheTacVuNhanh();

            _pnlMainContainer.Controls.Add(_lblDashboardTitle);
            _pnlMainContainer.Controls.Add(_lblDashboardSub);
            _pnlMainContainer.Controls.Add(_pnlQuickActions);

            // ================= 4. Bottom StatusStrip =================
            _statusStrip = new StatusStrip
            {
                BackColor = AppTheme.Surface,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Padding = new Padding(12, 4, 12, 4)
            };

            _lblStatusUser = new ToolStripStatusLabel
            {
                Text = $"Hệ thống đang hoạt động • Người dùng: {_taiKhoanDangNhap.TenDangnhap} ({_taiKhoanDangNhap.IdVaitroNavigation.TenVaitro})",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblStatusTime = new ToolStripStatusLabel
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                TextAlign = ContentAlignment.MiddleRight
            };

            _statusStrip.Items.AddRange(new ToolStripItem[] { _lblStatusUser, _lblStatusTime });

            // Ráp vào Form
            this.Controls.Add(_pnlMainContainer);
            this.Controls.Add(_pnlTopBar);
            this.Controls.Add(menuChinh);
            this.Controls.Add(_statusStrip);
        }

        private void TaiLaiTheTacVuNhanh()
        {
            if (_pnlQuickActions == null) return;
            _pnlQuickActions.SuspendLayout();
            _pnlQuickActions.Controls.Clear();

            // 1. Sơ đồ phòng
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_SoDoPhong_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_SoDoPhong_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_SoDoPhong_Btn"),
                AppTheme.Primary, () =>
            {
                using var form = new HotelManagement.Forms.FormSoDoPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            // 2. Đặt phòng mới
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_DatPhong_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_DatPhong_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_DatPhong_Btn"),
                AppTheme.AccentGold, () =>
            {
                using var form = new HotelManagement.Forms.FormDatPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            // 3. Check-in
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_CheckIn_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_CheckIn_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_CheckIn_Btn"),
                AppTheme.StatusSuccessText, () =>
            {
                using var form = new HotelManagement.Forms.FormCheckIn(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            // 4. Check-out
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_CheckOut_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_CheckOut_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_CheckOut_Btn"),
                AppTheme.StatusWarningText, () =>
            {
                using var form = new HotelManagement.Forms.FormCheckOut(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            // 5. Thanh toán
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_ThanhToan_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_ThanhToan_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_ThanhToan_Btn"),
                AppTheme.Primary, () =>
            {
                using var form = new HotelManagement.Forms.FormThanhToan();
                form.ShowDialog(this);
            });

            // 6. Khách hàng
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_KhachHang_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_KhachHang_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_KhachHang_Btn"),
                AppTheme.TextSecondary, () =>
            {
                using var form = new HotelManagement.Forms.FormKhachHang();
                form.ShowDialog(this);
            });

            // 7. Lịch sử thông báo
            ThemTheTacVuNhanh(
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_LichSuThongBao_Title"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_LichSuThongBao_Desc"),
                HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_LichSuThongBao_Btn"),
                AppTheme.Primary, () =>
            {
                using var form = new HotelManagement.Forms.FormLichSuThongBao();
                form.ShowDialog(this);
            });

            // 8. Dashboard (Admin)
            if (_taiKhoanDangNhap.IdVaitroNavigation.TenVaitro == "Admin")
            {
                ThemTheTacVuNhanh(
                    HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_Dashboard_Title"),
                    HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_Dashboard_Desc"),
                    HotelManagement.Localization.LanguageManager.GetString("FormChinh_Quick_Dashboard_Btn"),
                    AppTheme.AccentGold, () =>
                {
                    using var form = new HotelManagement.Forms.FormDashboard();
                    form.ShowDialog(this);
                });
            }

            _pnlQuickActions.ResumeLayout();
        }

        private void ThemTheTacVuNhanh(string title, string desc, string btnText, Color accentColor, Action action)
        {
            var pnlCard = new Panel
            {
                Width = 360,
                Height = 170,
                Margin = new Padding(0, 0, 20, 20),
                BackColor = AppTheme.Surface,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyCardPanel(pnlCard);

            // Dải màu nhấn bên trái
            pnlCard.Paint += (s, e) =>
            {
                using var brush = new SolidBrush(accentColor);
                e.Graphics.FillRectangle(brush, 0, 0, 5, pnlCard.Height);
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(18, 16),
                Size = new Size(325, 24)
            };

            var lblDesc = new Label
            {
                Text = desc,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(18, 44),
                Size = new Size(325, 55)
            };

            var btn = new Button
            {
                Text = btnText,
                Location = new Point(18, 112),
                Size = new Size(190, 36),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(btn);
            btn.Click += (_, _) => action();

            pnlCard.Controls.AddRange(new Control[] { lblTitle, lblDesc, btn });
            pnlCard.Click += (_, _) => action();
            lblTitle.Click += (_, _) => action();
            lblDesc.Click += (_, _) => action();

            _pnlQuickActions.Controls.Add(pnlCard);
        }

        #endregion

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
