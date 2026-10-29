using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
        private ToolStripMenuItem mnuQuanLyDatPhong = null!;
        private ToolStripMenuItem mnuDatPhong = null!;
        private ToolStripMenuItem mnuCheckIn = null!;
        private ToolStripMenuItem mnuCheckOut = null!;
        private ToolStripMenuItem mnuThanhToan = null!;
        private ToolStripMenuItem mnuQuanLyNhanSu = null!;   // chỉ Admin
        private ToolStripMenuItem mnuBaoCao = null!;         // chỉ Admin
        private ToolStripMenuItem mnuImportPhong = null!;    // chỉ Admin
        private ToolStripMenuItem mnuDoiMatKhau = null!;
        private ToolStripMenuItem mnuDangXuat = null!;

        // Controls giao diện bổ sung cho màn hình chính
        private Panel _pnlTopBar = null!;
        private Label _lblHotelTitle = null!;
        private Label _lblUserInfoBadge = null!;
        private Panel _pnlMainContainer = null!;
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
            var mnuQuanLyPhong = new ToolStripMenuItem("Quản lý phòng");
            
            var mnuPhong = new ToolStripMenuItem("Danh sách phòng");
            mnuPhong.Click += (_, _) =>
            {
                using var form = new HotelManagement.Forms.FormPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            };
            mnuQuanLyPhong.DropDownItems.Add(mnuPhong);

            var mnuLoaiPhong = new ToolStripMenuItem("Loại phòng");
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
            var mnuSoDoPhongTop = new ToolStripMenuItem("Sơ đồ phòng");
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
            var mnuKhachHang = new ToolStripMenuItem("Quản lý khách hàng");
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

            // --- 1.7 Đổi mật khẩu ---
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
            _lblUserInfoBadge.Location = new Point(_pnlTopBar.Width - 320, 16);
            _pnlTopBar.Resize += (s, e) =>
            {
                _lblUserInfoBadge.Location = new Point(_pnlTopBar.ClientSize.Width - _lblUserInfoBadge.Width - 24, 16);
            };

            _pnlTopBar.Controls.Add(_lblHotelTitle);
            _pnlTopBar.Controls.Add(_lblUserInfoBadge);

            // ================= 3. Main Dashboard: Các thẻ thao tác nhanh =================
            _pnlMainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                AutoScroll = true,
                Padding = new Padding(36, 28, 36, 28)
            };

            var lblDashboardTitle = new Label
            {
                Text = "BẢNG ĐIỀU KHIỂN & TÁC VỤ NHANH",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(36, 18)
            };

            var lblDashboardSub = new Label
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

            // Tạo các thẻ tác vụ nhanh
            ThemTheTacVuNhanh("SƠ ĐỒ PHÒNG TRỰC QUAN", "Theo dõi trạng thái phòng, đặt phòng và check-out nhanh theo sơ đồ", "Sơ đồ phòng", AppTheme.Primary, () =>
            {
                using var form = new HotelManagement.Forms.FormSoDoPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            ThemTheTacVuNhanh("ĐẶT PHÒNG MỚI", "Tạo booking mới cho khách lẻ, khách đoàn với đầy đủ dịch vụ", "Đặt phòng", AppTheme.AccentGold, () =>
            {
                using var form = new HotelManagement.Forms.FormDatPhong(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            ThemTheTacVuNhanh("CHECK-IN (NHẬN PHÒNG)", "Tra cứu booking theo mã hoặc tên/SĐT và bàn giao phòng cho khách", "Check-in", AppTheme.StatusSuccessText, () =>
            {
                using var form = new HotelManagement.Forms.FormCheckIn(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            ThemTheTacVuNhanh("CHECK-OUT (TRẢ PHÒNG)", "Kiểm tra phòng, tổng hợp phí dịch vụ phát sinh và thực hiện trả phòng", "Check-out", AppTheme.StatusWarningText, () =>
            {
                using var form = new HotelManagement.Forms.FormCheckOut(_taiKhoanDangNhap);
                form.ShowDialog(this);
            });

            ThemTheTacVuNhanh("THANH TOÁN HÓA ĐƠN", "Lập hóa đơn thanh toán tiền mặt hoặc chuyển khoản QR ngân hàng", "Thanh toán", AppTheme.Primary, () =>
            {
                using var form = new HotelManagement.Forms.FormThanhToan();
                form.ShowDialog(this);
            });

            ThemTheTacVuNhanh("HỒ SƠ KHÁCH HÀNG", "Tra cứu và quản lý thông tin khách lưu trú, CCCD, số điện thoại", "Khách hàng", AppTheme.TextSecondary, () =>
            {
                using var form = new HotelManagement.Forms.FormKhachHang();
                form.ShowDialog(this);
            });

            _pnlMainContainer.Controls.Add(lblDashboardTitle);
            _pnlMainContainer.Controls.Add(lblDashboardSub);
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
                Text = $"Mở {btnText} →",
                Location = new Point(18, 112),
                Size = new Size(180, 36),
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
