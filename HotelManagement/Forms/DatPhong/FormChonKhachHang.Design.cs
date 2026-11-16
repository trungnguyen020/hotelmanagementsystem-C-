using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormChonKhachHang
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox _txtTimKiem = null!;
        private Button _btnTimKiem = null!;
        private DataGridView _dgv = null!;
        private Button _btnThemMoi = null!;
        private Button _btnChon = null!;
        private Button _btnHuy = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Chọn khách hàng";
            Size = new Size(900, 560);
            MinimumSize = new Size(720, 440);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;

            var lblTimKiem = new Label { Text = "Tìm kiếm:", Location = new Point(16, 18), AutoSize = true };
            AppTheme.ApplyLabel(lblTimKiem);

            _txtTimKiem = new TextBox { Location = new Point(110, 15), Width = 300 };
            AppTheme.ApplyTextBox(_txtTimKiem);
            _txtTimKiem.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await TimKiemAsync();
                }
            };

            _btnTimKiem = new Button { Text = "Tìm", Location = new Point(420, 13), Width = 90, Height = 32 };
            AppTheme.ApplyPrimaryButton(_btnTimKiem);
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            _dgv = new DataGridView
            {
                Location = new Point(16, 60),
                Size = new Size(852, 397),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(_dgv);
            _dgv.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0) ChonKhachHang();
            };

            _btnThemMoi = new Button
            {
                Text = "Thêm khách hàng mới",
                Location = new Point(16, 469),
                Width = 180,
                Height = 36,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            AppTheme.ApplySecondaryButton(_btnThemMoi);
            _btnThemMoi.Click += async (_, _) => await ThemKhachHangMoiAsync();

            _btnChon = new Button
            {
                Text = "Chọn",
                Location = new Point(636, 469),
                Width = 110,
                Height = 36,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            AppTheme.ApplyPrimaryButton(_btnChon);
            _btnChon.Click += (_, _) => ChonKhachHang();

            _btnHuy = new Button
            {
                Text = "Hủy",
                Location = new Point(758, 469),
                Width = 110,
                Height = 36,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                DialogResult = DialogResult.Cancel
            };
            AppTheme.ApplySecondaryButton(_btnHuy);

            Controls.AddRange(new Control[]
            {
                lblTimKiem, _txtTimKiem, _btnTimKiem,
                _dgv,
                _btnThemMoi, _btnChon, _btnHuy
            });

            CancelButton = _btnHuy;
            AppTheme.AlignInputAfterLabel(lblTimKiem, _txtTimKiem);
        }
    }
}
