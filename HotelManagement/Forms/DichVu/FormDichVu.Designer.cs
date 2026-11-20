using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormDichVu
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

        private DataGridView _dgv = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;

        private TextBox _txtTen = null!;
        private NumericUpDown _numGia = null!;

        private Button _btnThem = null!;
        private Button _btnSua = null!;
        private Button _btnXoa = null!;
        private Button _btnLamMoi = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý dịch vụ";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(880, 580);

            var lblTimKiem = new Label { Text = "Tìm kiếm:", Location = new Point(12, 15), AutoSize = true };
            AppTheme.ApplyLabel(lblTimKiem);
            _txtTimKiem = new TextBox { Location = new Point(80, 12), Width = 300 };
            _btnTimKiem = new Button { Text = "Tìm", Location = new Point(390, 10), Width = 90, Height = 30 };
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            _dgv = new DataGridView
            {
                Location = new Point(12, 48),
                Width = 960,
                Height = 460,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            int yNhap = 525;
            var lblTen = new Label { Text = "Tên dịch vụ:", Location = new Point(12, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            AppTheme.ApplyLabel(lblTen);
            _txtTen = new TextBox { Location = new Point(110, yNhap - 3), Width = 280, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            var lblGia = new Label { Text = "Giá (VNĐ):", Location = new Point(420, yNhap), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            AppTheme.ApplyLabel(lblGia);
            _numGia = new NumericUpDown
            {
                Location = new Point(500, yNhap - 3),
                Width = 180,
                Maximum = 1_000_000_000,
                ThousandsSeparator = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            int yNut = yNhap + 45;
            _btnThem = new Button { Text = "Thêm", Location = new Point(12, yNut), Width = 90, Height = 34, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnSua = new Button { Text = "Sửa", Location = new Point(112, yNut), Width = 90, Height = 34, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnXoa = new Button { Text = "Xóa", Location = new Point(212, yNut), Width = 90, Height = 34, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };
            _btnLamMoi = new Button { Text = "Làm mới", Location = new Point(312, yNut), Width = 90, Height = 34, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            _btnThem.Click += async (_, _) => await ThemAsync();
            _btnSua.Click += async (_, _) => await SuaAsync();
            _btnXoa.Click += async (_, _) => await XoaAsync();
            _btnLamMoi.Click += (_, _) => LamMoiForm();

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                lblTen, _txtTen,
                lblGia, _numGia,
                _btnThem, _btnSua, _btnXoa, _btnLamMoi
            });

            AppTheme.ApplySecondaryButton(_btnTimKiem);
            AppTheme.ApplyTextBox(_txtTimKiem);
            AppTheme.ApplyTextBox(_txtTen);
            AppTheme.ApplyNumericUpDown(_numGia);
            AppTheme.ApplyDataGridView(_dgv);
            AppTheme.ApplyPrimaryButton(_btnThem);
            AppTheme.ApplySecondaryButton(_btnSua);
            AppTheme.ApplyDangerButton(_btnXoa);
            AppTheme.ApplySecondaryButton(_btnLamMoi);

            // Căn chỉnh vị trí ô nhập liệu & nút sau khi font/label tính xong
            this.Shown += (_, _) =>
            {
                _txtTimKiem.Location = new Point(lblTimKiem.Right + 8, lblTimKiem.Top - 3);
                _btnTimKiem.Location = new Point(_txtTimKiem.Right + 10, lblTimKiem.Top - 5);
                _btnTimKiem.Height = _txtTimKiem.Height + 6;

                _txtTen.Left = lblTen.Right + 8;
                lblGia.Left = _txtTen.Right + 20;
                _numGia.Left = lblGia.Right + 8;
            };
        }

        #endregion
    }
}
