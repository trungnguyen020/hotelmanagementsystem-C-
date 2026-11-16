using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormDangKyKhachHang
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

        // Header
        private Label lblTieuDe = null!;
        private Label lblPhuDe = null!;
        private Panel pnlDivider = null!;

        // Group 1: Thông tin khách hàng
        private GroupBox grpKhachHang = null!;
        private Label lblHoTen = null!;
        private TextBox txtHoTen = null!;
        private Label lblCccd = null!;
        private TextBox txtCccd = null!;
        private Label lblSdt = null!;
        private TextBox txtSdt = null!;
        private Label lblEmail = null!;
        private TextBox txtEmail = null!;

        // Group 2: Thông tin tài khoản
        private GroupBox grpTaiKhoan = null!;
        private Label lblTenDangNhap = null!;
        private TextBox txtTenDangNhap = null!;
        private Label lblMatKhau = null!;
        private TextBox txtMatKhau = null!;
        private Label lblXacNhanMatKhau = null!;
        private TextBox txtXacNhanMatKhau = null!;
        private CheckBox chkHienMatKhau = null!;

        // Actions & Feedback
        private Label lblThongBao = null!;
        private Button btnDangKy = null!;
        private Button btnHuy = null!;
        private LinkLabel lnkQuayVeDangNhap = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Form properties
            this.Text = "Đăng ký tài khoản khách hàng";
            this.ClientSize = new Size(580, 720);
            this.MinimumSize = new Size(580, 720);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            AppTheme.ApplyFormStyle(this);

            pnlBackground = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(20)
            };

            pnlCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface
            };
            AppTheme.ApplyCardPanel(pnlCard);

            // ================= Header =================
            lblTieuDe = new Label
            {
                Text = "ĐĂNG KÝ TÀI KHOẢN KHÁCH HÀNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 20),
                Size = new Size(500, 28)
            };

            lblPhuDe = new Label
            {
                Text = "Trở thành thành viên để nhận ưu đãi và quản lý đặt phòng dễ dàng",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 48),
                Size = new Size(500, 20)
            };

            pnlDivider = new Panel
            {
                Size = new Size(80, 3),
                Location = new Point(230, 72),
                BackColor = AppTheme.AccentGold
            };

            // ================= Group 1: Thông tin khách hàng =================
            grpKhachHang = new GroupBox
            {
                Text = "Thông tin cá nhân khách hàng",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 85),
                Size = new Size(492, 235),
                BackColor = AppTheme.Surface
            };

            // Họ tên
            lblHoTen = new Label
            {
                Text = "Họ và tên (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 28),
                AutoSize = true
            };
            txtHoTen = new TextBox
            {
                Location = new Point(16, 48),
                Size = new Size(460, 28),
                PlaceholderText = "Ví dụ: Nguyễn Văn A"
            };
            AppTheme.ApplyTextBox(txtHoTen);

            // CCCD
            lblCccd = new Label
            {
                Text = "Số CCCD/Định danh (12 số) (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 80),
                AutoSize = true
            };
            txtCccd = new TextBox
            {
                Location = new Point(16, 100),
                Size = new Size(460, 28),
                MaxLength = 12,
                PlaceholderText = "Nhập đủ 12 chữ số CCCD..."
            };
            AppTheme.ApplyTextBox(txtCccd);

            // SĐT
            lblSdt = new Label
            {
                Text = "Số điện thoại (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 132),
                AutoSize = true
            };
            txtSdt = new TextBox
            {
                Location = new Point(16, 152),
                Size = new Size(220, 28),
                MaxLength = 15,
                PlaceholderText = "Ví dụ: 0912345678"
            };
            AppTheme.ApplyTextBox(txtSdt);

            // Email
            lblEmail = new Label
            {
                Text = "Email nhận xác nhận (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(250, 132),
                AutoSize = true
            };
            txtEmail = new TextBox
            {
                Location = new Point(250, 152),
                Size = new Size(226, 28),
                PlaceholderText = "email@example.com"
            };
            AppTheme.ApplyTextBox(txtEmail);

            grpKhachHang.Controls.AddRange(new Control[]
            {
                lblHoTen, txtHoTen,
                lblCccd, txtCccd,
                lblSdt, txtSdt,
                lblEmail, txtEmail
            });

            // ================= Group 2: Thông tin tài khoản =================
            grpTaiKhoan = new GroupBox
            {
                Text = "Thông tin tài khoản đăng nhập",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 328),
                Size = new Size(492, 215),
                BackColor = AppTheme.Surface
            };

            // Tên đăng nhập
            lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 26),
                AutoSize = true
            };
            txtTenDangNhap = new TextBox
            {
                Location = new Point(16, 46),
                Size = new Size(460, 28),
                PlaceholderText = "Tên tài khoản (viết liền không dấu)..."
            };
            AppTheme.ApplyTextBox(txtTenDangNhap);

            // Mật khẩu
            lblMatKhau = new Label
            {
                Text = "Mật khẩu (tối thiểu 6 ký tự) (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 78),
                AutoSize = true
            };
            txtMatKhau = new TextBox
            {
                Location = new Point(16, 98),
                Size = new Size(460, 28),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập mật khẩu..."
            };
            AppTheme.ApplyTextBox(txtMatKhau);

            // Xác nhận mật khẩu
            lblXacNhanMatKhau = new Label
            {
                Text = "Xác nhận mật khẩu (*):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 130),
                AutoSize = true
            };
            txtXacNhanMatKhau = new TextBox
            {
                Location = new Point(16, 150),
                Size = new Size(460, 28),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập lại chính xác mật khẩu..."
            };
            AppTheme.ApplyTextBox(txtXacNhanMatKhau);

            // Hiện mật khẩu checkbox
            chkHienMatKhau = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Location = new Point(16, 184),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyCheckBox(chkHienMatKhau);
            chkHienMatKhau.CheckedChanged += (s, e) =>
            {
                txtMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;
                txtXacNhanMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;
            };

            grpTaiKhoan.Controls.AddRange(new Control[]
            {
                lblTenDangNhap, txtTenDangNhap,
                lblMatKhau, txtMatKhau,
                lblXacNhanMatKhau, txtXacNhanMatKhau,
                chkHienMatKhau
            });

            // ================= Thông báo trạng thái =================
            lblThongBao = new Label
            {
                Location = new Point(24, 550),
                Size = new Size(492, 34),
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty
            };

            // ================= Buttons =================
            btnDangKy = new Button
            {
                Text = "ĐĂNG KÝ TÀI KHOẢN",
                Location = new Point(24, 590),
                Size = new Size(340, 42),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnDangKy);
            btnDangKy.Click += BtnDangKy_Click;

            btnHuy = new Button
            {
                Text = "HỦY BỎ",
                Location = new Point(374, 590),
                Size = new Size(142, 42),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(btnHuy);
            btnHuy.Click += (s, e) => this.Close();

            // Link quay lại Đăng nhập
            lnkQuayVeDangNhap = new LinkLabel
            {
                Text = "← Đã có tài khoản? Quay lại Đăng nhập",
                Font = AppTheme.FontBody,
                LinkColor = AppTheme.Primary,
                ActiveLinkColor = AppTheme.PrimaryActive,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(24, 640),
                Size = new Size(492, 24),
                Cursor = Cursors.Hand
            };
            lnkQuayVeDangNhap.LinkClicked += (s, e) => this.Close();

            // Thêm controls vào card
            pnlCard.Controls.AddRange(new Control[]
            {
                lblTieuDe,
                lblPhuDe,
                pnlDivider,
                grpKhachHang,
                grpTaiKhoan,
                lblThongBao,
                btnDangKy,
                btnHuy,
                lnkQuayVeDangNhap
            });

            pnlBackground.Controls.Add(pnlCard);
            this.Controls.Add(pnlBackground);
        }

        #endregion
    }
}

