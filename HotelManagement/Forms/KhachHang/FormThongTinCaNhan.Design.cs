using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormThongTinCaNhan
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

        private Panel pnlHeader = null!;
        private Label lblHeaderIcon = null!;
        private Label lblHeaderTitle = null!;
        private Label lblHeaderSub = null!;

        private Panel pnlMainBody = null!;
        private TableLayoutPanel tlpColumns = null!;

        // --- Cột Trái: Hồ sơ cá nhân ---
        private Panel pnlLeftCard = null!;
        private Label lblTitleLeft = null!;
        private Label lblSubLeft = null!;

        private Label lblHoTen = null!;
        private TextBox txtHoTen = null!;

        private Label lblCccd = null!;
        private TextBox txtCccd = null!;
        private Label lblGhiChuCccd = null!;

        private Label lblSdt = null!;
        private TextBox txtSdt = null!;

        private Label lblEmail = null!;
        private TextBox txtEmail = null!;

        private Label lblGioiTinh = null!;
        private ComboBox cboGioiTinh = null!;

        private CheckBox chkCoNgaySinh = null!;
        private DateTimePicker dtpNgaySinh = null!;

        private Button btnLuuThongTin = null!;
        private Label lblThongBaoHoSo = null!;

        // --- Cột Phải: Đổi mật khẩu ---
        private Panel pnlRightCard = null!;
        private Label lblTitleRight = null!;
        private Label lblSubRight = null!;

        private Label lblTenDangNhap = null!;
        private TextBox txtTenDangNhap = null!;

        private Label lblMatKhauCu = null!;
        private TextBox txtMatKhauCu = null!;

        private Label lblMatKhauMoi = null!;
        private TextBox txtMatKhauMoi = null!;

        private Label lblXacNhanMatKhau = null!;
        private TextBox txtXacNhanMatKhau = null!;

        private CheckBox chkHienThiMatKhau = null!;

        private Button btnDoiMatKhau = null!;
        private Label lblThongBaoBaoMat = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = AppTheme.Background;

            // ================= 1. Header Panel =================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = AppTheme.Surface,
                Padding = new Padding(20, 10, 20, 10)
            };

            lblHeaderIcon = new Label
            {
                Text = "👤",
                Font = new Font(AppTheme.DefaultFontFamily, 22f),
                Location = new Point(16, 10),
                Size = new Size(45, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblHeaderTitle = new Label
            {
                Text = "THÔNG TIN CÁ NHÂN & BẢO MẬT TÀI KHOẢN",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(68, 12),
                AutoSize = true,
                UseMnemonic = false // Tránh ký tự & bị hiểu là phím tắt
            };

            lblHeaderSub = new Label
            {
                Text = "Xem, điều chỉnh thông tin liên hệ và cập nhật mật khẩu tài khoản của bạn.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(70, 36),
                AutoSize = true
            };

            pnlHeader.Controls.AddRange(new Control[]
            {
                lblHeaderIcon,
                lblHeaderTitle,
                lblHeaderSub
            });

            // ================= 2. Main Body (2 Columns Layout) =================
            pnlMainBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 16, 20, 16),
                AutoScroll = true
            };

            tlpColumns = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tlpColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 53f));
            tlpColumns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47f));
            tlpColumns.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // ================= 2.1 Cột Trái: Thẻ Hồ Sơ Khách Hàng =================
            pnlLeftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(24, 20, 24, 20),
                Margin = new Padding(0, 0, 12, 0),
                AutoScroll = true
            };
            AppTheme.ApplyCardPanel(pnlLeftCard);

            lblTitleLeft = new Label
            {
                Text = "📋  Hồ sơ khách hàng",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(20, 18),
                AutoSize = true
            };

            lblSubLeft = new Label
            {
                Text = "Thông tin dùng cho việc xác nhận đặt phòng và xuất hóa đơn.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(22, 44),
                AutoSize = true
            };

            int yL = 80;
            int cotNhapL = 175; // Khoảng cách đủ rộng để nhãn dài nhất không bao giờ che ô nhập
            int inputLWidth = 275;

            // Họ và tên
            lblHoTen = new Label { Text = "Họ và tên *:", Location = new Point(20, yL), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtHoTen = new TextBox { Location = new Point(cotNhapL, yL - 3), Width = inputLWidth, Font = AppTheme.FontBody };

            // Số CCCD (ReadOnly)
            yL += 44;
            lblCccd = new Label { Text = "Số CCCD:", Location = new Point(20, yL), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtCccd = new TextBox
            {
                Location = new Point(cotNhapL, yL - 3),
                Width = inputLWidth,
                ReadOnly = true,
                BackColor = AppTheme.SurfaceSecondary,
                Font = AppTheme.FontBody
            };

            yL += 32;
            lblGhiChuCccd = new Label
            {
                Text = "ℹ️ Số CCCD là định danh cố định. Nếu cần sửa, vui lòng liên hệ lễ tân.",
                Location = new Point(cotNhapL, yL),
                AutoSize = true,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted
            };

            // Số điện thoại
            yL += 36;
            lblSdt = new Label { Text = "Số điện thoại *:", Location = new Point(20, yL), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtSdt = new TextBox { Location = new Point(cotNhapL, yL - 3), Width = inputLWidth, Font = AppTheme.FontBody };

            // Địa chỉ Email
            yL += 44;
            lblEmail = new Label { Text = "Địa chỉ Email *:", Location = new Point(20, yL), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtEmail = new TextBox { Location = new Point(cotNhapL, yL - 3), Width = inputLWidth, Font = AppTheme.FontBody };

            // Giới tính
            yL += 44;
            lblGioiTinh = new Label { Text = "Giới tính:", Location = new Point(20, yL), AutoSize = true, Font = AppTheme.FontBodyBold };
            cboGioiTinh = new ComboBox
            {
                Location = new Point(cotNhapL, yL - 3),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = AppTheme.FontBody
            };
            cboGioiTinh.Items.AddRange(new object[] { "", "Nam", "Nữ" });

            // Ngày sinh
            yL += 44;
            chkCoNgaySinh = new CheckBox
            {
                Text = "Ngày sinh:",
                Location = new Point(20, yL),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                Checked = false
            };
            dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(cotNhapL, yL - 3),
                Width = 160,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Enabled = false,
                Font = AppTheme.FontBody
            };
            chkCoNgaySinh.CheckedChanged += (s, e) => dtpNgaySinh.Enabled = chkCoNgaySinh.Checked;

            // Nút Lưu thay đổi thông tin
            yL += 54;
            btnLuuThongTin = new Button
            {
                Text = "💾  Lưu thay đổi thông tin",
                Location = new Point(cotNhapL, yL),
                Size = new Size(250, 40),
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            AppTheme.ApplyPrimaryButton(btnLuuThongTin);
            btnLuuThongTin.Click += async (s, e) => await LuuThongTinAsync();

            yL += 48;
            lblThongBaoHoSo = new Label
            {
                Location = new Point(20, yL),
                AutoSize = true,
                Font = AppTheme.FontSmallBold
            };

            pnlLeftCard.Controls.AddRange(new Control[]
            {
                lblTitleLeft, lblSubLeft,
                lblHoTen, txtHoTen,
                lblCccd, txtCccd, lblGhiChuCccd,
                lblSdt, txtSdt,
                lblEmail, txtEmail,
                lblGioiTinh, cboGioiTinh,
                chkCoNgaySinh, dtpNgaySinh,
                btnLuuThongTin, lblThongBaoHoSo
            });

            // ================= 2.2 Cột Phải: Thẻ Bảo Mật & Đổi Mật Khẩu =================
            pnlRightCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(24, 20, 24, 20),
                Margin = new Padding(12, 0, 0, 0),
                AutoScroll = true
            };
            AppTheme.ApplyCardPanel(pnlRightCard);

            lblTitleRight = new Label
            {
                Text = "🔒  Bảo mật & Đổi mật khẩu",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(20, 18),
                AutoSize = true,
                UseMnemonic = false
            };

            lblSubRight = new Label
            {
                Text = "Mật khẩu mới phải có tối thiểu 6 ký tự và khác mật khẩu hiện tại.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(22, 44),
                AutoSize = true
            };

            int yR = 80;
            int cotNhapR = 195; // Đủ rộng cho "Mật khẩu hiện tại *:" và "Xác nhận MK mới *:"
            int inputRWidth = 240;

            // Tên tài khoản
            lblTenDangNhap = new Label { Text = "Tên tài khoản:", Location = new Point(20, yR), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtTenDangNhap = new TextBox
            {
                Location = new Point(cotNhapR, yR - 3),
                Width = inputRWidth,
                ReadOnly = true,
                BackColor = AppTheme.SurfaceSecondary,
                Font = AppTheme.FontBody
            };

            // Mật khẩu hiện tại
            yR += 46;
            lblMatKhauCu = new Label { Text = "Mật khẩu hiện tại *:", Location = new Point(20, yR), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtMatKhauCu = new TextBox
            {
                Location = new Point(cotNhapR, yR - 3),
                Width = inputRWidth,
                UseSystemPasswordChar = true,
                Font = AppTheme.FontBody
            };

            // Mật khẩu mới
            yR += 46;
            lblMatKhauMoi = new Label { Text = "Mật khẩu mới *:", Location = new Point(20, yR), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtMatKhauMoi = new TextBox
            {
                Location = new Point(cotNhapR, yR - 3),
                Width = inputRWidth,
                UseSystemPasswordChar = true,
                Font = AppTheme.FontBody
            };

            // Xác nhận mật khẩu mới
            yR += 46;
            lblXacNhanMatKhau = new Label { Text = "Xác nhận MK mới *:", Location = new Point(20, yR), AutoSize = true, Font = AppTheme.FontBodyBold };
            txtXacNhanMatKhau = new TextBox
            {
                Location = new Point(cotNhapR, yR - 3),
                Width = inputRWidth,
                UseSystemPasswordChar = true,
                Font = AppTheme.FontBody
            };

            // Checkbox hiển thị mật khẩu
            yR += 38;
            chkHienThiMatKhau = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Location = new Point(cotNhapR, yR),
                AutoSize = true,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Cursor = Cursors.Hand
            };
            chkHienThiMatKhau.CheckedChanged += (s, e) =>
            {
                bool hide = !chkHienThiMatKhau.Checked;
                txtMatKhauCu.UseSystemPasswordChar = hide;
                txtMatKhauMoi.UseSystemPasswordChar = hide;
                txtXacNhanMatKhau.UseSystemPasswordChar = hide;
            };

            // Nút Cập nhật mật khẩu
            yR += 52;
            btnDoiMatKhau = new Button
            {
                Text = "🔑  Cập nhật mật khẩu",
                Location = new Point(cotNhapR, yR),
                Size = new Size(230, 40),
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            AppTheme.ApplyPrimaryButton(btnDoiMatKhau);
            btnDoiMatKhau.Click += async (s, e) => await DoiMatKhauAsync();

            yR += 48;
            lblThongBaoBaoMat = new Label
            {
                Location = new Point(20, yR),
                AutoSize = true,
                Font = AppTheme.FontSmallBold
            };

            pnlRightCard.Controls.AddRange(new Control[]
            {
                lblTitleRight, lblSubRight,
                lblTenDangNhap, txtTenDangNhap,
                lblMatKhauCu, txtMatKhauCu,
                lblMatKhauMoi, txtMatKhauMoi,
                lblXacNhanMatKhau, txtXacNhanMatKhau,
                chkHienThiMatKhau,
                btnDoiMatKhau, lblThongBaoBaoMat
            });

            // Ráp vào TableLayoutPanel
            tlpColumns.Controls.Add(pnlLeftCard, 0, 0);
            tlpColumns.Controls.Add(pnlRightCard, 1, 0);

            pnlMainBody.Controls.Add(tlpColumns);

            this.Controls.Add(pnlMainBody);
            this.Controls.Add(pnlHeader);

            // Tự động căn chỉnh động theo DPI để đảm bảo không bao giờ bị đè trên mọi màn hình
            this.Load += (_, _) => CanChinhDynamic();
        }

        private void CanChinhDynamic()
        {
            try
            {
                // Canh cột trái
                int maxL = Math.Max(
                    Math.Max(lblHoTen.Right, lblCccd.Right),
                    Math.Max(lblSdt.Right, lblEmail.Right)
                ) + 12;

                txtHoTen.Left = maxL;
                txtCccd.Left = maxL;
                txtSdt.Left = maxL;
                txtEmail.Left = maxL;
                cboGioiTinh.Left = maxL;
                dtpNgaySinh.Left = maxL;
                lblGhiChuCccd.Left = maxL;
                btnLuuThongTin.Left = maxL;

                // Canh cột phải
                int maxR = Math.Max(
                    Math.Max(lblTenDangNhap.Right, lblMatKhauCu.Right),
                    Math.Max(lblMatKhauMoi.Right, lblXacNhanMatKhau.Right)
                ) + 12;

                txtTenDangNhap.Left = maxR;
                txtMatKhauCu.Left = maxR;
                txtMatKhauMoi.Left = maxR;
                txtXacNhanMatKhau.Left = maxR;
                chkHienThiMatKhau.Left = maxR;
                btnDoiMatKhau.Left = maxR;
            }
            catch
            {
                // Bỏ qua nếu form chưa layout xong
            }
        }

        #endregion
    }
}
