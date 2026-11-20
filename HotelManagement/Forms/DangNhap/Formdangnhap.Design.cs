using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;
using HotelManagement.Localization;

namespace HotelManagement
{
    partial class Formdangnhap
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

        private Panel pnlBackground = null!;
        private Panel pnlCard = null!;
        private Panel _pnlBrandSide = null!;
        private Panel _pnlBrandInfo = null!;
        private PictureBox _picLogo = null!;
        private TextBox txtTenDangNhap = null!;
        private TextBox txtMatKhau = null!;
        private Button btnDangNhap = null!;
        private Label lblThongBao = null!;

        private Label _lblBrand = null!;
        private Label _lblTagline = null!;
        private Label _lblHotline = null!;
        private Label _lblTieuDe = null!;
        private Label _lblTenDangNhap = null!;
        private Label _lblMatKhau = null!;
        private LinkLabel _lnkDangKy = null!;
        private Label _lblFooter = null!;
        private Button _btnLangVi = null!;
        private Button _btnLangEn = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Cấu hình Form đăng nhập: bố cục chia đôi (thương hiệu | đăng nhập)
            this.Text = "Hệ thống quản lý khách sạn — Đăng nhập";
            this.ClientSize = new Size(1040, 640);
            this.MinimumSize = new Size(1040, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            AppTheme.ApplyFormStyle(this);

            // Container bao quanh
            pnlBackground = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background
            };

            // ================= 1. Panel trái: Ảnh nền + thương hiệu =================
            _pnlBrandSide = new Panel
            {
                Dock = DockStyle.Left,
                Width = 520,
                BackColor = AppTheme.Ink,
                BackgroundImage = EmbeddedImages.Background,
                BackgroundImageLayout = ImageLayout.Stretch
            };

            // Logo đặt trên nền trắng để nổi bật trên ảnh
            var pnlLogo = new Panel
            {
                Location = new Point(44, 44),
                Size = new Size(150, 150),
                BackColor = AppTheme.Surface,
                Padding = new Padding(10)
            };
            AppTheme.ApplyCardPanel(pnlLogo);

            _picLogo = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = EmbeddedImages.Logo,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = AppTheme.Surface
            };
            pnlLogo.Controls.Add(_picLogo);

            // Dải thông tin thương hiệu ở đáy ảnh (nền đặc để chữ luôn dễ đọc)
            _pnlBrandInfo = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                BackColor = AppTheme.Ink,
                Padding = new Padding(44, 26, 36, 20)
            };

            _lblBrand = new Label
            {
                Text = "RESORT & HOTEL MANAGEMENT",
                Font = new Font(AppTheme.DefaultFontFamily, 16F, FontStyle.Bold),
                ForeColor = AppTheme.TextOnPrimary,
                Location = new Point(44, 30),
                AutoSize = true,
                UseMnemonic = false
            };

            _lblTagline = new Label
            {
                Text = "Khách sạn nghỉ dưỡng • Trung tâm thành phố",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.SurfaceSecondary,
                Location = new Point(46, 76),
                AutoSize = true,
                UseMnemonic = false
            };

            var pnlDivider = new Panel
            {
                Size = new Size(64, 3),
                Location = new Point(46, 108),
                BackColor = AppTheme.AccentGold
            };

            _lblHotline = new Label
            {
                Text = "☎ Hotline lễ tân: 1900 6868",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.SurfaceSecondary,
                Location = new Point(46, 126),
                AutoSize = true,
                UseMnemonic = false
            };

            _pnlBrandInfo.Controls.AddRange(new Control[]
            {
                _lblHotline,
                pnlDivider,
                _lblTagline,
                _lblBrand
            });

            _pnlBrandSide.Controls.Add(_pnlBrandInfo);
            _pnlBrandSide.Controls.Add(pnlLogo);

            // ================= 2. Panel phải: Card đăng nhập =================
            var pnlLoginSide = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background
            };

            pnlCard = new Panel
            {
                Size = new Size(440, 500),
                Location = new Point(40, 70),
                BackColor = AppTheme.Surface
            };
            AppTheme.ApplyCardPanel(pnlCard);

            // Nút chọn ngôn ngữ góc trên bên phải của Card (VI / EN)
            _btnLangVi = new Button
            {
                Text = "VI",
                Size = new Size(52, 30),
                Location = new Point(322, 16),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(_btnLangVi);
            _btnLangVi.Click += (s, e) => LanguageManager.SetLanguage(AppLanguage.Vietnamese);

            _btnLangEn = new Button
            {
                Text = "EN",
                Size = new Size(52, 30),
                Location = new Point(380, 16),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnLangEn);
            _btnLangEn.Click += (s, e) => LanguageManager.SetLanguage(AppLanguage.English);

            // Tiêu đề chính
            _lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 58),
                Size = new Size(400, 30)
            };

            // Dải gạch trang trí màu vàng đồng
            var pnlCardDivider = new Panel
            {
                Size = new Size(60, 3),
                Location = new Point(190, 100),
                BackColor = AppTheme.AccentGold
            };

            // 3. Nhập Tên đăng nhập
            _lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(30, 118),
                AutoSize = true
            };

            txtTenDangNhap = new TextBox
            {
                Location = new Point(30, 142),
                Size = new Size(380, 32),
                PlaceholderText = "Nhập tài khoản nhân sự...",
                TabIndex = 0,
                TabStop = true
            };
            AppTheme.ApplyTextBox(txtTenDangNhap);

            // 4. Nhập Mật khẩu
            _lblMatKhau = new Label
            {
                Text = "Mật khẩu",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(30, 192),
                AutoSize = true
            };

            txtMatKhau = new TextBox
            {
                Location = new Point(30, 216),
                Size = new Size(380, 32),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập mật khẩu...",
                TabIndex = 1,
                TabStop = true
            };
            AppTheme.ApplyTextBox(txtMatKhau);

            // 5. Nút Đăng nhập (Primary CTA)
            btnDangNhap = new Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(30, 266),
                Size = new Size(380, 44),
                Cursor = Cursors.Hand,
                TabIndex = 2,
                TabStop = true
            };
            AppTheme.ApplyPrimaryButton(btnDangNhap);
            btnDangNhap.Click += BtnDangNhap_Click;

            // 6. Label Thông báo lỗi / trạng thái
            lblThongBao = new Label
            {
                Location = new Point(30, 318),
                Size = new Size(380, 30),
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty
            };

            // 7. Link Đăng ký tài khoản dành cho Khách hàng (DCKLTTQ-106)
            _lnkDangKy = new LinkLabel
            {
                Text = "✨ Khách hàng mới? Đăng ký tài khoản tại đây",
                Location = new Point(30, 365),
                Size = new Size(380, 32),
                Font = AppTheme.FontBodyBold,
                LinkColor = AppTheme.Primary,
                ActiveLinkColor = AppTheme.PrimaryActive,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                TabIndex = 3,
                TabStop = true
            };
            _lnkDangKy.LinkClicked += (s, e) =>
            {
                using var formDangKy = new HotelManagement.Forms.FormDangKyKhachHang();
                formDangKy.ShowDialog();
            };

            // 8. Ghi chú chân trang
            _lblFooter = new Label
            {
                Text = "Hệ thống Quản lý Khách sạn — Dành cho Nhân sự & Khách hàng",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 420),
                Size = new Size(400, 30),
                UseMnemonic = false
            };

            _btnLangVi.TabIndex = 4;
            _btnLangEn.TabIndex = 5;

            // Thêm controls vào Card theo đúng thứ tự hiển thị và TabIndex hợp lý
            pnlCard.Controls.AddRange(new Control[]
            {
                txtTenDangNhap,
                txtMatKhau,
                btnDangNhap,
                _lnkDangKy,
                _btnLangVi,
                _btnLangEn,
                _lblTenDangNhap,
                _lblMatKhau,
                lblThongBao,
                pnlCardDivider,
                _lblTieuDe,
                _lblFooter
            });

            pnlLoginSide.Controls.Add(pnlCard);

            pnlBackground.Controls.Add(pnlLoginSide);
            pnlBackground.Controls.Add(_pnlBrandSide);
            this.Controls.Add(pnlBackground);

            // Phím tắt bàn phím tiện ích
            this.AcceptButton = btnDangNhap;
        }

        #endregion
    }
}
