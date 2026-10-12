using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormNhanSu
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

        private TextBox _txtHoTen = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtSdt = null!;
        private DateTimePicker _dtpNgaySinh = null!;

        // --- Phần tạo tài khoản lễ tân ---
        private CheckBox _chkTaoTaiKhoan = null!;
        private Label _lblTenDangNhap = null!;
        private TextBox _txtTenDangNhap = null!;
        private Label _lblMatKhau = null!;
        private TextBox _txtMatKhau = null!;
        private Label _lblTaiKhoanInfo = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;

        // --- Nút quản lý tài khoản ---
        private Button _btnDatLaiMk = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            Text = "Quản lý nhân sự";
            Width = 1150;
            Height = 800;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 700);

            // --- Thanh tìm kiếm ---
            var lblTimKiem = new Label { Text = "Tìm kiếm:", Location = new Point(12, 15), AutoSize = true };
            _txtTimKiem = new TextBox { Location = new Point(80, 12), Width = 300 };
            _btnTimKiem = new Button { Text = "Tìm", Location = new Point(415, 10), Width = 100, Height = 30 };
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            // --- DataGridView ---
            _dgv = new DataGridView
            {
                Location = new Point(12, 45),
                Width = 1110,
                Height = 489,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            // --- Vùng nhập liệu nhân sự ---
            int yNhap = 549;
            int lblWidth = 120;

            var lblHoTen = new Label { Text = "Họ tên:", Location = new Point(12, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtHoTen = new TextBox { Location = new Point(lblWidth + 12, yNhap - 3), Width = 250, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(400, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtCccd = new TextBox { Location = new Point(460, yNhap - 3), Width = 180, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblSdt = new Label { Text = "Số điện thoại:", Location = new Point(12, yNhap + 35), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtSdt = new TextBox { Location = new Point(lblWidth + 12, yNhap + 32), Width = 180, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblNgaySinh = new Label { Text = "Ngày sinh:", Location = new Point(400, yNhap + 35), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(460, yNhap + 32),
                Width = 160,
                Format = DateTimePickerFormat.Short,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            // --- Phần tạo tài khoản lễ tân ---
            int yTaiKhoan = yNhap + 75;

            // Label hiển thị thông tin tài khoản hiện có (khi chọn nhân sự đã có TK)
            _lblTaiKhoanInfo = new Label
            {
                Text = "",
                Location = new Point(12, yTaiKhoan),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Blue,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            _chkTaoTaiKhoan = new CheckBox
            {
                Text = "Tạo tài khoản Lễ tân khi thêm nhân sự",
                Location = new Point(12, yTaiKhoan + 25),
                AutoSize = true,
                Checked = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _chkTaoTaiKhoan.CheckedChanged += (_, _) => CapNhatHienThiTaiKhoan();

            _lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(12, yTaiKhoan + 55),
                AutoSize = true,
                Visible = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _txtTenDangNhap = new TextBox
            {
                Location = new Point(lblWidth + 12, yTaiKhoan + 52),
                Width = 200,
                Visible = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            _lblMatKhau = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(400, yTaiKhoan + 55),
                AutoSize = true,
                Visible = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _txtMatKhau = new TextBox
            {
                Location = new Point(460, yTaiKhoan + 52),
                Width = 200,
                Visible = false,
                UseSystemPasswordChar = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            // --- Nút thao tác ---
            int yNut = yTaiKhoan + 95;
            _btnThem = new Button { Text = "Thêm", Location = new Point(12, yNut), Width = 90, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnSua = new Button { Text = "Sửa", Location = new Point(112, yNut), Width = 90, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(212, yNut), Width = 90, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(312, yNut), Width = 90, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            _btnDatLaiMk = new Button { Text = "Đặt lại MK", Location = new Point(412, yNut), Width = 100, Height = 30, Visible = false, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();
            _btnDatLaiMk.Click += async (_, _) => await DatLaiMatKhauAsync();

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                lblHoTen, _txtHoTen, lblCccd, _txtCccd,
                lblSdt, _txtSdt, lblNgaySinh, _dtpNgaySinh,
                _lblTaiKhoanInfo,
                _chkTaoTaiKhoan,
                _lblTenDangNhap, _txtTenDangNhap,
                _lblMatKhau, _txtMatKhau,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi, _btnDatLaiMk
            });
        }

        #endregion
    }
}
