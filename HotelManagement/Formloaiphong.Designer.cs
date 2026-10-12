using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormLoaiPhong
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
        private NumericUpDown _numGia = null!;
        private NumericUpDown _numSucChua = null!;
        private TextBox _txtMoTa = null!;

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

            Text = "Quản lý loại phòng";
            Width = 1150;
            Height = 760;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(920, 620);

            var lblTimKiem = new Label { Text = "Tìm kiếm:", Location = new Point(12, 15), AutoSize = true };
            _txtTimKiem = new TextBox { Location = new Point(80, 12), Width = 300 };
            _btnTimKiem = new Button { Text = "Tìm", Location = new Point(415, 10), Width = 100, Height = 30 };
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

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

            int yNhap = 549;
            var lblTen = new Label { Text = "Tên loại phòng:", Location = new Point(12, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtTen = new TextBox { Location = new Point(130, yNhap - 3), Width = 220, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblGia = new Label { Text = "Giá (VNĐ):", Location = new Point(370, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _numGia = new NumericUpDown
            {
                Location = new Point(460, yNhap - 3),
                Width = 150,
                Maximum = 1_000_000_000,
                ThousandsSeparator = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            var lblSucChua = new Label { Text = "Sức chứa (người):", Location = new Point(12, yNhap + 35), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _numSucChua = new NumericUpDown
            {
                Location = new Point(150, yNhap + 32),
                Width = 80,
                Minimum = 1,
                Maximum = 20,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            var lblMoTa = new Label { Text = "Mô tả:", Location = new Point(12, yNhap + 70), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _txtMoTa = new TextBox
            {
                Location = new Point(130, yNhap + 67),
                Width = 992,
                Height = 50,
                Multiline = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            int yNut = yNhap + 130;
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
                lblTen, _txtTen, lblGia, _numGia,
                lblSucChua, _numSucChua,
                lblMoTa, _txtMoTa,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi
            });
        }

        #endregion
    }
}
