using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Models.Entities;

namespace HotelManagement.Forms
{
    partial class FormKhachHangPortal
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

        private Panel pnlTopBar = null!;
        private Label lblLogo = null!;
        private Label lblTitle = null!;
        private Label lblUserBadge = null!;
        private Button btnMoChatbot = null!;
        private Button btnDangXuat = null!;

        private MenuStrip menuStripKhachHang = null!;
        private ToolStripMenuItem mnuDatPhong = null!;
        private ToolStripMenuItem mnuLichSu = null!;
        private ToolStripMenuItem mnuThongTin = null!;
        private ToolStripMenuItem mnuDangXuat = null!;

        private TabControl tabContent = null!;
        private TabPage tabDatPhong = null!;
        private TabPage tabLichSu = null!;
        private TabPage tabThongTin = null!;

        private FormDatPhongKhachHang formDatPhongKhachHang = null!;
        private FormLichSuKhachHang formLichSuKhachHang = null!;
        private FormThongTinCaNhan formThongTinCaNhan = null!;

        // Khung Trợ lý ảo AI Drawer góc phải như Kiosk
        private Panel pnlChatDrawer = null!;
        private FormChatbotKhachHang formChatbotKhachHang = null!;
        private Button btnFloatingAi = null!;

        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel lblStatusInfo = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.Text = "Hệ thống Khách sạn — Cổng thông tin Khách hàng";
            this.ClientSize = new Size(1100, 700);
            this.MinimumSize = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            AppTheme.ApplyFormStyle(this);

            // ================= 1. Top Bar =================
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(20, 10, 20, 10)
            };

            lblLogo = new Label
            {
                Text = "🏨",
                Font = new Font(AppTheme.DefaultFontFamily, 22f),
                Location = new Point(16, 12),
                Size = new Size(45, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblTitle = new Label
            {
                Text = "CỔNG KHÁCH HÀNG — DỊCH VỤ TRỰC TUYẾN",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(65, 14),
                AutoSize = true
            };

            var tenKhach = _taiKhoanDangNhap.IdKhachhangNavigation?.Ten ?? _taiKhoanDangNhap.TenDangnhap;
            lblUserBadge = new Label
            {
                Text = $"Xin chào, {tenKhach} (Khách hàng)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                BackColor = AppTheme.PrimaryLight,
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            lblUserBadge.Location = new Point(this.ClientSize.Width - 580, 18);

            // Nút mở trợ lý ảo AI trên thanh header
            btnMoChatbot = new Button
            {
                Text = "🤖  Trợ lý ảo AI",
                Size = new Size(145, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            btnMoChatbot.Location = new Point(this.ClientSize.Width - 275, 15);
            AppTheme.ApplyPrimaryButton(btnMoChatbot);
            btnMoChatbot.Click += (s, e) => BatTatChatDrawer();

            btnDangXuat = new Button
            {
                Text = "Đăng xuất",
                Size = new Size(100, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnDangXuat.Location = new Point(this.ClientSize.Width - 120, 15);
            AppTheme.ApplySecondaryButton(btnDangXuat);
            btnDangXuat.Click += BtnDangXuat_Click;

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblLogo,
                lblTitle,
                lblUserBadge,
                btnMoChatbot,
                btnDangXuat
            });

            // ================= 2. MenuStrip điều hướng =================
            menuStripKhachHang = new MenuStrip
            {
                Dock = DockStyle.Top,
                BackColor = AppTheme.Surface,
                Font = AppTheme.FontBodyBold,
                Padding = new Padding(12, 6, 12, 6)
            };

            mnuDatPhong = new ToolStripMenuItem("📅  Đặt phòng trực tuyến");
            mnuDatPhong.Click += (s, e) => ChuyenTab(0);

            mnuLichSu = new ToolStripMenuItem("📜  Lịch sử đặt phòng");
            mnuLichSu.Click += (s, e) => ChuyenTab(1);

            mnuThongTin = new ToolStripMenuItem("👤  Thông tin cá nhân");
            mnuThongTin.Click += (s, e) => ChuyenTab(2);

            mnuDangXuat = new ToolStripMenuItem("🚪  Đăng xuất");
            mnuDangXuat.Alignment = ToolStripItemAlignment.Right;
            mnuDangXuat.ForeColor = AppTheme.StatusDangerText;
            mnuDangXuat.Click += BtnDangXuat_Click;

            menuStripKhachHang.Items.AddRange(new ToolStripItem[]
            {
                mnuDatPhong,
                mnuLichSu,
                mnuThongTin,
                mnuDangXuat
            });

            // ================= 3. Chatbot Drawer góc phải (như Kiosk) =================
            pnlChatDrawer = new Panel
            {
                Width = 460,
                Dock = DockStyle.Right,
                Visible = false,
                BackColor = AppTheme.Surface,
                Padding = new Padding(0)
            };

            formChatbotKhachHang = new FormChatbotKhachHang(_taiKhoanDangNhap)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            formChatbotKhachHang.YeuCauDong += (s, e) => BatTatChatDrawer(false);
            pnlChatDrawer.Controls.Add(formChatbotKhachHang);
            formChatbotKhachHang.Show();

            // ================= 4. TabControl / Content Panels =================
            tabContent = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.FontBody,
                ItemSize = new Size(0, 1),
                SizeMode = TabSizeMode.Fixed,
                Appearance = TabAppearance.Buttons
            };

            // Tab 1: Đặt phòng (DCKLTTQ-108)
            tabDatPhong = new TabPage { Text = "Đặt phòng", BackColor = AppTheme.Background };
            formDatPhongKhachHang = new FormDatPhongKhachHang(_taiKhoanDangNhap)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            tabDatPhong.Controls.Add(formDatPhongKhachHang);
            formDatPhongKhachHang.Show();

            // Tab 2: Lịch sử đặt phòng + Xuất hóa đơn PDF (DCKLTTQ-109)
            tabLichSu = new TabPage { Text = "Lịch sử đặt phòng", BackColor = AppTheme.Background };
            formLichSuKhachHang = new FormLichSuKhachHang(_taiKhoanDangNhap)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            tabLichSu.Controls.Add(formLichSuKhachHang);
            formLichSuKhachHang.Show();

            // Tab 3: Thông tin cá nhân & Đổi mật khẩu (DCKLTTQ-110)
            tabThongTin = new TabPage { Text = "Thông tin cá nhân", BackColor = AppTheme.Background };
            formThongTinCaNhan = new FormThongTinCaNhan(_taiKhoanDangNhap)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            formThongTinCaNhan.ThongTinKhachHangDaThayDoi += (s, tenMoi) =>
            {
                lblUserBadge.Text = $"Xin chào, {tenMoi} (Khách hàng)";
            };
            tabThongTin.Controls.Add(formThongTinCaNhan);
            formThongTinCaNhan.Show();

            tabContent.TabPages.AddRange(new TabPage[]
            {
                tabDatPhong,
                tabLichSu,
                tabThongTin
            });

            // ================= 5. Nút icon Trợ lý AI góc phải nổi bật (FAB) =================
            btnFloatingAi = new Button
            {
                Text = "🤖 Trợ lý AI",
                Size = new Size(130, 42),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            btnFloatingAi.Location = new Point(this.ClientSize.Width - 150, this.ClientSize.Height - 80);
            AppTheme.ApplyPrimaryButton(btnFloatingAi);
            btnFloatingAi.Click += (s, e) => BatTatChatDrawer();

            // ================= 6. StatusStrip =================
            statusStrip = new StatusStrip
            {
                BackColor = AppTheme.SurfaceSecondary,
                Font = AppTheme.FontSmall
            };
            lblStatusInfo = new ToolStripStatusLabel
            {
                Text = $"Cổng Khách Hàng • Tài khoản: {_taiKhoanDangNhap.TenDangnhap} • Trạng thái: Sẵn sàng",
                ForeColor = AppTheme.TextSecondary
            };
            statusStrip.Items.Add(lblStatusInfo);

            // Gắn vào Form (tabContent Fill, pnlChatDrawer Right, menu & topbar Top, statusstrip Bottom)
            this.Controls.Add(btnFloatingAi);
            this.Controls.Add(tabContent);
            this.Controls.Add(pnlChatDrawer);
            this.Controls.Add(menuStripKhachHang);
            this.Controls.Add(pnlTopBar);
            this.Controls.Add(statusStrip);

            btnFloatingAi.BringToFront();

            this.MainMenuStrip = menuStripKhachHang;
        }

        private Panel TaoPanelPlaceholder(string tieuDe, string moTa)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(30)
            };

            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface
            };
            AppTheme.ApplyCardPanel(card);

            var lblTieuDe = new Label
            {
                Text = tieuDe,
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            var lblMoTa = new Label
            {
                Text = moTa,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 40
            };

            var lblGhiChu = new Label
            {
                Text = "⚡ Khung giao diện sẵn sàng cho các nhiệm vụ tiếp theo.",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.AccentGold,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 50
            };

            card.Controls.AddRange(new Control[] { lblMoTa, lblTieuDe, lblGhiChu });
            pnl.Controls.Add(card);
            return pnl;
        }

        public void BatTatChatDrawer(bool? hienThi = null)
        {
            bool newState = hienThi ?? !pnlChatDrawer.Visible;
            pnlChatDrawer.Visible = newState;
            btnFloatingAi.Visible = !newState;

            if (newState)
            {
                pnlChatDrawer.BringToFront();
            }
        }

        public void ChuyenTab(int index)
        {
            if (index >= 0 && index < tabContent.TabPages.Count)
            {
                tabContent.SelectedIndex = index;
                CapNhatMenuHighlight(index);

                // Khi chuyển sang Tab Lịch sử (index 1), tự động tải lại danh sách mới nhất
                if (index == 1 && formLichSuKhachHang != null)
                {
                    _ = formLichSuKhachHang.TaiLichSuAsync();
                }
            }
        }

        private void CapNhatMenuHighlight(int index)
        {
            mnuDatPhong.BackColor = index == 0 ? AppTheme.PrimaryLight : AppTheme.Surface;
            mnuLichSu.BackColor = index == 1 ? AppTheme.PrimaryLight : AppTheme.Surface;
            mnuThongTin.BackColor = index == 2 ? AppTheme.PrimaryLight : AppTheme.Surface;
        }

        #endregion
    }
}
