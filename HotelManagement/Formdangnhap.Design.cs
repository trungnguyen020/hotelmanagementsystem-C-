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
            this.ClientSize = new Size(580, 580);
            this.MinimumSize = new Size(580, 580);
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
                Padding = new Padding(40)
            };

            // Thẻ Card đăng nhập trung tâm
            pnlCard = new Panel
            {
                Size = new Size(500, 500),
                Location = new Point(40, 40),
                BackColor = AppTheme.Surface
            };
            AppTheme.ApplyCardPanel(pnlCard);

            // 1. Nhãn thương hiệu nhỏ phía trên bên trái
            _lblBrand = new Label
            {
                Text = "HOTEL MANAGEMENT • RESORT",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(35, 20),
                Size = new Size(300, 24)
            };

            // Nút chọn ngôn ngữ góc trên bên phải của Card (VI / EN)
            _btnLangVi = new Button
            {
                Text = "VI",
                Size = new Size(56, 28),
                Location = new Point(360, 18),
                Font = AppTheme.FontBodyBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(_btnLangVi);
            _btnLangVi.Click += (s, e) => LanguageManager.SetLanguage(AppLanguage.Vietnamese);

            _btnLangEn = new Button
            {
                Text = "EN",
                Size = new Size(56, 28),
                Location = new Point(422, 18),
                Font = AppTheme.FontBodyBold,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(_btnLangEn);
            _btnLangEn.Click += (s, e) => LanguageManager.SetLanguage(AppLanguage.English);

            // 2. Tiêu đề chính
            _lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = AppTheme.FontDisplay,
                ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 54),
                Size = new Size(460, 40)
            };

            // Dải gạch trang trí màu vàng đồng
            var pnlDivider = new Panel
            {
                Size = new Size(60, 3),
                Location = new Point(220, 98),
                BackColor = AppTheme.AccentGold
            };

            // 3. Nhập Tên đăng nhập
            _lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(45, 118),
                AutoSize = true
            };

            txtTenDangNhap = new TextBox
            {
                Location = new Point(45, 142),
                Size = new Size(410, 34),
                PlaceholderText = "Nhập tài khoản nhân sự..."
            };
            AppTheme.ApplyTextBox(txtTenDangNhap);

            // 4. Nhập Mật khẩu
            _lblMatKhau = new Label
            {
                Text = "Mật khẩu",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(45, 186),
                AutoSize = true
            };

            txtMatKhau = new TextBox
            {
                Location = new Point(45, 210),
                Size = new Size(410, 34),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập mật khẩu..."
            };
            AppTheme.ApplyTextBox(txtMatKhau);

            // 5. Nút Đăng nhập (Primary CTA)
            btnDangNhap = new Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(45, 264),
                Size = new Size(410, 44),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnDangNhap);
            btnDangNhap.Click += BtnDangNhap_Click;

            // 6. Label Thông báo lỗi / trạng thái
            lblThongBao = new Label
            {
                Location = new Point(45, 314),
                Size = new Size(410, 36),
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty
            };

            // 7. Nút chuyển sang Chế độ Kiosk tra cứu dành cho sảnh khách sạn (DCKLTTQ-85)
            _btnKiosk = new Button
            {
                Text = "🖥️  Chế độ Kiosk tra cứu (Dành cho khách hàng)",
                Location = new Point(45, 356),
                Size = new Size(410, 42),
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
                Location = new Point(20, 412),
                Size = new Size(460, 22)
            };

            // Thêm controls vào Card
            pnlCard.Controls.AddRange(new Control[]
            {
                _lblBrand,
                _btnLangVi,
                _btnLangEn,
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
