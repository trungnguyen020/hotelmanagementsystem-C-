using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Cấu hình Form đăng nhập
            this.Text = "Hệ thống quản lý khách sạn — Đăng nhập";
            this.ClientSize = new Size(540, 480);
            this.MinimumSize = new Size(540, 480);
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
                Size = new Size(460, 400),
                Location = new Point(40, 40),
                BackColor = AppTheme.Surface
            };
            AppTheme.ApplyCardPanel(pnlCard);

            // 1. Nhãn thương hiệu nhỏ phía trên
            var lblBrand = new Label
            {
                Text = "HOTEL MANAGEMENT • RESORT & SUITES",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.OceanBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 24),
                Size = new Size(420, 20)
            };

            // 2. Tiêu đề chính
            var lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = AppTheme.FontDisplay,
                ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 48),
                Size = new Size(420, 32)
            };

            // Dải gạch trang trí màu Teal
            var pnlDivider = new Panel
            {
                Size = new Size(60, 3),
                Location = new Point(200, 85),
                BackColor = AppTheme.Primary
            };

            // 3. Nhập Tên đăng nhập
            var lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(40, 108),
                AutoSize = true
            };

            txtTenDangNhap = new TextBox
            {
                Location = new Point(40, 132),
                Size = new Size(380, 34),
                PlaceholderText = "Nhập tài khoản nhân sự..."
            };
            AppTheme.ApplyTextBox(txtTenDangNhap);

            // 4. Nhập Mật khẩu
            var lblMatKhau = new Label
            {
                Text = "Mật khẩu",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(40, 180),
                AutoSize = true
            };

            txtMatKhau = new TextBox
            {
                Location = new Point(40, 204),
                Size = new Size(380, 34),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập mật khẩu..."
            };
            AppTheme.ApplyTextBox(txtMatKhau);

            // 5. Nút Đăng nhập (Primary CTA)
            btnDangNhap = new Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(40, 258),
                Size = new Size(380, 42),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnDangNhap);
            btnDangNhap.Click += BtnDangNhap_Click;

            // 6. Label Thông báo lỗi / trạng thái
            lblThongBao = new Label
            {
                Location = new Point(40, 308),
                Size = new Size(380, 42),
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty
            };

            // 7. Ghi chú chân trang
            var lblFooter = new Label
            {
                Text = "Phiên bản bảo mật nội bộ — Ban Quản Lý & Lễ Tân",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 355),
                Size = new Size(380, 20)
            };

            // Thêm controls vào Card
            pnlCard.Controls.AddRange(new Control[]
            {
                lblBrand,
                lblTieuDe,
                pnlDivider,
                lblTenDangNhap,
                txtTenDangNhap,
                lblMatKhau,
                txtMatKhau,
                btnDangNhap,
                lblThongBao,
                lblFooter
            });

            pnlBackground.Controls.Add(pnlCard);
            this.Controls.Add(pnlBackground);

            // Canh giữa card khi resize nếu có
            this.Resize += (s, e) =>
            {
                pnlCard.Location = new Point(
                    Math.Max(10, (pnlBackground.ClientSize.Width - pnlCard.Width) / 2),
                    Math.Max(10, (pnlBackground.ClientSize.Height - pnlCard.Height) / 2)
                );
            };

            this.AcceptButton = btnDangNhap;
        }

        #endregion
    }
}
