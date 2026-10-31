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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            Text = "Quản lý khách hàng";
            Width = 1150;
            Height = 760;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 640);

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
                Height = 504,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            // --- Vùng nhập liệu ---
            int yNhap = 564;
            int lblWidth = 120;

            var lblTen = new Label { Text = "Tên khách hàng:", Location = new Point(12, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtTen = new TextBox { Location = new Point(lblWidth + 12, yNhap - 3), Width = 250, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(400, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtCccd = new TextBox { Location = new Point(460, yNhap - 3), Width = 180, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblSdt = new Label { Text = "Số điện thoại:", Location = new Point(12, yNhap + 35), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtSdt = new TextBox { Location = new Point(lblWidth + 12, yNhap + 32), Width = 180, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblEmail = new Label { Text = "Email:", Location = new Point(400, yNhap + 35), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtEmail = new TextBox { Location = new Point(460, yNhap + 32), Width = 250, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblGioiTinh = new Label { Text = "Giới tính:", Location = new Point(12, yNhap + 70), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _cboGioiTinh = new ComboBox
            {
                Location = new Point(lblWidth + 12, yNhap + 67),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _cboGioiTinh.Items.AddRange(new object[] { "", "Nam", "Nữ" });
            _cboGioiTinh.SelectedIndex = 0;

            _chkCoNgaySinh = new CheckBox
            {
                Text = "Ngày sinh:",
                Location = new Point(270, yNhap + 70),
                AutoSize = true,
                Checked = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(370, yNhap + 67),
                Width = 160,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Enabled = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _chkCoNgaySinh.CheckedChanged += (_, _) => _dtpNgaySinh.Enabled = _chkCoNgaySinh.Checked;

            // --- Nút thao tác ---
            int yNut = yNhap + 115;
            _btnThem = new Button { Text = "Thêm", Location = new Point(12, yNut), Width = 100, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnSua = new Button { Text = "Sửa", Location = new Point(122, yNut), Width = 100, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(232, yNut), Width = 100, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(342, yNut), Width = 100, Height = 30, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                lblTen, _txtTen, lblCccd, _txtCccd,
                lblSdt, _txtSdt, lblEmail, _txtEmail,
                lblGioiTinh, _cboGioiTinh,
                _chkCoNgaySinh, _dtpNgaySinh,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi
            });
        }

        #endregion
    }
}
