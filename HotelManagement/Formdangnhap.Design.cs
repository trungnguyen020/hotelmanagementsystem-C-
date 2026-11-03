using System;
using System.Drawing;
using System.Windows.Forms;
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
        private TextBox txtTenDangNhap = null!;
        private TextBox txtMatKhau = null!;
        private Button btnDangNhap = null!;
        private Label lblThongBao = null!;

        private Label _lblBrand = null!;
        private Label _lblTieuDe = null!;
        private Label _lblTenDangNhap = null!;
        private Label _lblMatKhau = null!;
        private Button _btnKiosk = null!;
        private Label _lblFooter = null!;
        private Button _btnLangVi = null!;
        private Button _btnLangEn = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Cấu hình Form đăng nhập
            this.Text = "Hệ thống quản lý khách sạn — Đăng nhập";
            this.ClientSize = new Size(540, 525);
            this.MinimumSize = new Size(540, 525);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            AppTheme.ApplyFormStyle(this);

            // Container bao quanh
            pnlBackground = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(30)
            };

            // Thẻ Card đăng nhập trung tâm
            pnlCard = new Panel
            {
                Size = new Size(460, 445),
                Location = new Point(40, 40),
                BackColor = AppTheme.Surface
            };
            AppTheme.ApplyCardPanel(pnlCard);

            // Nút chọn ngôn ngữ góc trên bên phải của Card đăng nhập (DCKLTTQ-86)
            _btnLangVi = new Button
            {
                Text = "🇻🇳 VN",
                Size = new Size(54, 26),
                Location = new Point(338, 16),
                Font = AppTheme.FontCaption,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(_btnLangVi);
            _btnLangVi.Click += (s, e) => LanguageManager.SetLanguage(AppLanguage.Vietnamese);

            _btnLangEn = new Button
            {
                Text = "🇬🇧 EN",
                Size = new Size(54, 26),
                Location = new Point(396, 16),
                Font = AppTheme.FontCaption,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnLangEn);
            _btnLangEn.Click += (s, e) => LanguageManager.SetLanguage(AppLanguage.English);

            // 1. Nhãn thương hiệu nhỏ phía trên
            _lblBrand = new Label
            {
                Text = "HOTEL MANAGEMENT • RESORT & SUITES",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(40, 20),
                Size = new Size(290, 18)
            };

            // 2. Tiêu đề chính
            _lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = AppTheme.FontDisplay,
                ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 46),
                Size = new Size(420, 38)
            };

            // Dải gạch trang trí màu vàng đồng
            var pnlDivider = new Panel
            {
                Size = new Size(60, 3),
                Location = new Point(200, 92),
                BackColor = AppTheme.AccentGold
            };

            // 3. Nhập Tên đăng nhập
            _lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(40, 118),
                AutoSize = true
            };

            txtTenDangNhap = new TextBox
            {
                Location = new Point(40, 142),
                Size = new Size(380, 34),
                PlaceholderText = "Nhập tài khoản nhân sự..."
            };
            AppTheme.ApplyTextBox(txtTenDangNhap);

            // 4. Nhập Mật khẩu
            _lblMatKhau = new Label
            {
                Text = "Mật khẩu",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(40, 188),
                AutoSize = true
            };

            txtMatKhau = new TextBox
            {
                Location = new Point(40, 212),
                Size = new Size(380, 34),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập mật khẩu..."
            };
            AppTheme.ApplyTextBox(txtMatKhau);

            // 5. Nút Đăng nhập (Primary CTA)
            btnDangNhap = new Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(40, 268),
                Size = new Size(380, 42),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnDangNhap);
            btnDangNhap.Click += BtnDangNhap_Click;

            // 6. Label Thông báo lỗi / trạng thái
            lblThongBao = new Label
            {
                Location = new Point(40, 318),
                Size = new Size(380, 40),
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty
            };

            // 7. Nút chuyển sang Chế độ Kiosk tra cứu dành cho sảnh khách sạn (DCKLTTQ-85)
            _btnKiosk = new Button
            {
                Text = "🖥️  Chế độ Kiosk tra cứu (Dành cho khách hàng)",
                Location = new Point(40, 362),
                Size = new Size(380, 38),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnKiosk);
            _btnKiosk.Click += (s, e) =>
            {
                this.Hide();
                using (var formKiosk = new HotelManagement.Forms.FormKiosk())
                {
                    formKiosk.ShowDialog();
                }
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Activate();
                txtTenDangNhap.Focus();
            };

            // 8. Ghi chú chân trang
            _lblFooter = new Label
            {
                Text = "Phiên bản bảo mật nội bộ — Ban Quản Lý & Lễ Tân",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 410),
                Size = new Size(380, 20)
            };

            // Thêm controls vào Card
            pnlCard.Controls.AddRange(new Control[]
            {
                _btnLangVi,
                _btnLangEn,
                _lblBrand,
                _lblTieuDe,
                pnlDivider,
                _lblTenDangNhap,
                txtTenDangNhap,
                _lblMatKhau,
                txtMatKhau,
                btnDangNhap,
                lblThongBao,
                _btnKiosk,
                _lblFooter
            });

            pnlBackground.Controls.Add(pnlCard);
            this.Controls.Add(pnlBackground);

            // Phím tắt bàn phím tiện ích
            this.AcceptButton = btnDangNhap;
        }

        #endregion
    }
}
