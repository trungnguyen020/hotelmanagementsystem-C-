using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormKhachHang
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private DataGridView _dgv = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;

        private TextBox _txtTen = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtEmail = null!;
        private TextBox _txtSdt = null!;
        private ComboBox _cboGioiTinh = null!;
        private DateTimePicker _dtpNgaySinh = null!;
        private CheckBox _chkCoNgaySinh = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;
        private Button _btnDatPhong = null!;

        // Quản lý tài khoản khách hàng dành cho Admin/Lễ tân
        private Label _lblTaiKhoanInfo = null!;
        private Button _btnDatLaiMk = null!;
        private Button _btnKhoaMoKhoa = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý khách hàng";
            Width = 1150;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(950, 600);

            // ================= 1. Thanh tìm kiếm (Top) =================
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                Padding = new Padding(14, 10, 14, 10),
                BackColor = AppTheme.Surface
            };

            var lblTimKiem = new Label { Text = "Tìm kiếm:", AutoSize = true };
            _txtTimKiem = new TextBox { Width = 320, PlaceholderText = "Nhập tên, số điện thoại, CCCD..." };
            _btnTimKiem = new Button { Text = "Tìm", Width = 90, Height = 32, Cursor = Cursors.Hand };
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            pnlTop.Controls.AddRange(new Control[] { lblTimKiem, _txtTimKiem, _btnTimKiem });

            // ================= 2. Vùng điều khiển & Nhập liệu (Bottom) =================
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 185,
                Padding = new Padding(12, 6, 12, 6),
                BackColor = AppTheme.Surface
            };

            // --- 2.1 GroupBox Quản lý tài khoản (Nằm bên phải, luôn cố định và rõ ràng) ---
            var grpTaiKhoan = new GroupBox
            {
                Text = "Tài khoản khách hàng",
                Dock = DockStyle.Right,
                Width = 390,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Padding = new Padding(10)
            };

            _lblTaiKhoanInfo = new Label
            {
                Text = "Chưa chọn khách hàng",
                Location = new Point(15, 25),
                Size = new Size(360, 48),
                AutoSize = false,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary
            };

            _btnDatLaiMk = new Button
            {
                Text = "🔑 Đặt lại MK (123456)",
                Location = new Point(15, 82),
                Width = 175,
                Height = 34,
                Visible = false,
                Cursor = Cursors.Hand
            };
            _btnDatLaiMk.Click += async (_, _) => await DatLaiMatKhauAsync();

            _btnKhoaMoKhoa = new Button
            {
                Text = "🔒 Khóa tài khoản",
                Location = new Point(200, 82),
                Width = 175,
                Height = 34,
                Visible = false,
                Cursor = Cursors.Hand
            };
            _btnKhoaMoKhoa.Click += async (_, _) => await ChuyenTrangThaiTaiKhoanAsync();

            grpTaiKhoan.Controls.AddRange(new Control[] { _lblTaiKhoanInfo, _btnDatLaiMk, _btnKhoaMoKhoa });

            // --- 2.2 Vùng nhập thông tin khách hàng (Nằm bên trái) ---
            var pnlThongTinKhach = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 15, 0)
            };

            int lblWidth = 115;
            var lblTen = new Label { Text = "Tên khách hàng:", Location = new Point(0, 10), AutoSize = true };
            _txtTen = new TextBox { Location = new Point(lblWidth, 7), Width = 230 };

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(360, 10), AutoSize = true };
            _txtCccd = new TextBox { Location = new Point(415, 7), Width = 160 };

            var lblSdt = new Label { Text = "Số điện thoại:", Location = new Point(0, 44), AutoSize = true };
            _txtSdt = new TextBox { Location = new Point(lblWidth, 41), Width = 160 };

            var lblEmail = new Label { Text = "Email:", Location = new Point(320, 44), AutoSize = true };
            _txtEmail = new TextBox { Location = new Point(375, 41), Width = 200 };

            var lblGioiTinh = new Label { Text = "Giới tính:", Location = new Point(0, 78), AutoSize = true };
            _cboGioiTinh = new ComboBox
            {
                Location = new Point(lblWidth, 75),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboGioiTinh.Items.AddRange(new object[] { "", "Nam", "Nữ" });
            _cboGioiTinh.SelectedIndex = 0;

            _chkCoNgaySinh = new CheckBox
            {
                Text = "Ngày sinh:",
                Location = new Point(245, 78),
                AutoSize = true,
                Checked = false
            };
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(340, 75),
                Width = 140,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Enabled = false
            };
            _chkCoNgaySinh.CheckedChanged += (_, _) => _dtpNgaySinh.Enabled = _chkCoNgaySinh.Checked;

            int yNut = 122;
            _btnThem = new Button { Text = "Thêm", Location = new Point(0, yNut), Width = 85, Height = 34, Cursor = Cursors.Hand };
            _btnSua = new Button { Text = "Sửa", Location = new Point(92, yNut), Width = 85, Height = 34, Cursor = Cursors.Hand };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(184, yNut), Width = 85, Height = 34, Cursor = Cursors.Hand };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(276, yNut), Width = 90, Height = 34, Cursor = Cursors.Hand };
            _btnDatPhong = new Button { Text = "🛎️ Đặt phòng", Location = new Point(374, yNut), Width = 125, Height = 34, Cursor = Cursors.Hand };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();
            _btnDatPhong.Click += (_, _) => MoDatPhongChoKhachHang();

            pnlThongTinKhach.Controls.AddRange(new Control[]
            {
                lblTen, _txtTen, lblCccd, _txtCccd,
                lblSdt, _txtSdt, lblEmail, _txtEmail,
                lblGioiTinh, _cboGioiTinh, _chkCoNgaySinh, _dtpNgaySinh,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi, _btnDatPhong
            });

            pnlBottom.Controls.AddRange(new Control[] { pnlThongTinKhach, grpTaiKhoan });

            // ================= 3. DataGridView (Fill ở giữa) =================
            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            // Thứ tự Add: Fill sau cùng để Dock hoạt động chính xác
            Controls.Add(_dgv);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);

            AppTheme.ApplyLabel(lblTimKiem);
            AppTheme.ApplyLabel(lblTen);
            AppTheme.ApplyLabel(lblCccd);
            AppTheme.ApplyLabel(lblSdt);
            AppTheme.ApplyLabel(lblEmail);
            AppTheme.ApplyLabel(lblGioiTinh);

            AppTheme.ApplyTextBox(_txtTimKiem);
            AppTheme.ApplyTextBox(_txtTen);
            AppTheme.ApplyTextBox(_txtCccd);
            AppTheme.ApplyTextBox(_txtSdt);
            AppTheme.ApplyTextBox(_txtEmail);
            AppTheme.ApplyComboBox(_cboGioiTinh);
            AppTheme.ApplyDateTimePicker(_dtpNgaySinh);
            AppTheme.ApplyCheckBox(_chkCoNgaySinh);
            AppTheme.ApplyDataGridView(_dgv);

            AppTheme.ApplyPrimaryButton(_btnTimKiem);
            AppTheme.ApplyPrimaryButton(_btnThem);
            AppTheme.ApplyPrimaryButton(_btnDatPhong);
            AppTheme.ApplySecondaryButton(_btnSua);
            AppTheme.ApplySecondaryButton(_btnXoa);
            AppTheme.ApplySecondaryButton(_btnLamMoi);
            AppTheme.ApplySecondaryButton(_btnDatLaiMk);
            AppTheme.ApplySecondaryButton(_btnKhoaMoKhoa);

            // Label AutoSize rộng ra theo font mới -> canh ô nhập theo mép label, giữ cột thẳng hàng
            int cotNhapTrai = Math.Max(lblTen.Right, Math.Max(lblSdt.Right, lblGioiTinh.Right)) + 8;
            _txtTen.Left = cotNhapTrai;
            _txtSdt.Left = cotNhapTrai;
            _cboGioiTinh.Left = cotNhapTrai;

            // Cột phải phải nằm sau mép phải của ô rộng nhất bên trái
            lblCccd.Left = _txtTen.Right + 24;
            lblEmail.Left = lblCccd.Left;

            int cotNhapPhai = Math.Max(lblCccd.Right, lblEmail.Right) + 8;
            _txtCccd.Left = cotNhapPhai;
            _txtEmail.Left = cotNhapPhai;

            _chkCoNgaySinh.Left = _cboGioiTinh.Right + 24;
            _dtpNgaySinh.Left = _chkCoNgaySinh.Right + 8;

            // Canh lề chuẩn cho thanh tìm kiếm (Top): nhãn, ô nhập và nút đều được canh giữa dọc và cách đều ngang
            lblTimKiem.Location = new Point(14, (pnlTop.Height - lblTimKiem.Height) / 2);
            _txtTimKiem.Location = new Point(lblTimKiem.Right + 8, (pnlTop.Height - _txtTimKiem.Height) / 2);
            _btnTimKiem.Location = new Point(_txtTimKiem.Right + 10, (pnlTop.Height - _btnTimKiem.Height) / 2);
        }

        #endregion
    }
}
