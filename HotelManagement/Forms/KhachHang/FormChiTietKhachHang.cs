using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.Forms
{
    public class FormChiTietKhachHang : Form
    {
        private readonly KhachHangDto? _dtoGoc;
        public KhachHangDto DtoKetQua { get; private set; } = null!;

        private TextBox _txtTen = null!;
        private TextBox _txtSdt = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtEmail = null!;
        private ComboBox _cboGioiTinh = null!;
        private DateTimePicker _dtpNgaySinh = null!;
        private CheckBox _chkCoNgaySinh = null!;

        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        public FormChiTietKhachHang(KhachHangDto? dto = null)
        {
            _dtoGoc = dto;
            KhoiTaoGiaoDien();
            TaiDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = _dtoGoc == null ? "Thêm mới khách hàng" : "Cập nhật khách hàng";
            Width = 560;
            Height = 440;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AppTheme.ApplyFormStyle(this);

            var lblTieuDe = new Label
            {
                Text = _dtoGoc == null ? "THÊM MỚI KHÁCH HÀNG" : "CẬP NHẬT KHÁCH HÀNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 20),
                AutoSize = true
            };

            int y = 65;
            int lblX = 24;
            int inputX = 200;
            int inputW = 310;
            int spacing = 42;

            var lblTen = new Label { Text = "Họ và tên (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblTen);
            _txtTen = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtTen);

            y += spacing;
            var lblSdt = new Label { Text = "Số điện thoại (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblSdt);
            _txtSdt = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtSdt);

            y += spacing;
            var lblCccd = new Label { Text = "CCCD/CMND (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblCccd);
            _txtCccd = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtCccd);

            y += spacing;
            var lblEmail = new Label { Text = "Email:", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblEmail);
            _txtEmail = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtEmail);

            y += spacing;
            var lblGioiTinh = new Label { Text = "Giới tính:", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblGioiTinh);
            _cboGioiTinh = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            _cboGioiTinh.SelectedIndex = 0;
            AppTheme.ApplyComboBox(_cboGioiTinh);

            y += spacing;
            var lblNgaySinh = new Label { Text = "Ngày sinh:", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblNgaySinh);

            _chkCoNgaySinh = new CheckBox { Text = "", Location = new Point(inputX, y + 2), AutoSize = true, Checked = true };
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(inputX + 24, y),
                Width = 160,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            AppTheme.ApplyDateTimePicker(_dtpNgaySinh);

            _chkCoNgaySinh.CheckedChanged += (_, _) => _dtpNgaySinh.Enabled = _chkCoNgaySinh.Checked;

            y += 65;
            _btnLuu = new Button { Text = "💾 Lưu", Location = new Point(inputX + 60, y), Width = 110, Height = 36 };
            _btnHuy = new Button { Text = "✕ Hủy", Location = new Point(inputX + 180, y), Width = 110, Height = 36 };

            AppTheme.ApplyPrimaryButton(_btnLuu);
            AppTheme.ApplySecondaryButton(_btnHuy);

            _btnLuu.Click += BtnLuu_Click;
            _btnHuy.Click += (_, _) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lblTieuDe,
                lblTen, _txtTen,
                lblSdt, _txtSdt,
                lblCccd, _txtCccd,
                lblEmail, _txtEmail,
                lblGioiTinh, _cboGioiTinh,
                lblNgaySinh, _chkCoNgaySinh, _dtpNgaySinh,
                _btnLuu, _btnHuy
            });
        }

        private void TaiDuLieu()
        {
            if (_dtoGoc == null) return;

            _txtTen.Text = _dtoGoc.Ten;
            _txtSdt.Text = _dtoGoc.Sdt;
            _txtCccd.Text = _dtoGoc.Cccd;
            _txtEmail.Text = _dtoGoc.Email;

            if (!string.IsNullOrEmpty(_dtoGoc.GioiTinh))
            {
                int idx = _cboGioiTinh.Items.IndexOf(_dtoGoc.GioiTinh);
                if (idx >= 0) _cboGioiTinh.SelectedIndex = idx;
            }

            if (_dtoGoc.NgaySinh.HasValue)
            {
                _chkCoNgaySinh.Checked = true;
                _dtpNgaySinh.Enabled = true;
                _dtpNgaySinh.Value = _dtoGoc.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                _chkCoNgaySinh.Checked = false;
                _dtpNgaySinh.Enabled = false;
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            string ten = _txtTen.Text.Trim();
            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtTen.Focus();
                return;
            }

            string sdt = _txtSdt.Text.Trim();
            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtSdt.Focus();
                return;
            }

            string cccd = _txtCccd.Text.Trim();
            if (string.IsNullOrWhiteSpace(cccd))
            {
                MessageBox.Show("Vui lòng nhập CCCD/CMND.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCccd.Focus();
                return;
            }

            DateOnly? ngaySinh = _chkCoNgaySinh.Checked
                ? DateOnly.FromDateTime(_dtpNgaySinh.Value)
                : null;

            DtoKetQua = new KhachHangDto
            {
                IdKhachHang = _dtoGoc?.IdKhachHang ?? 0,
                Ten = ten,
                Sdt = sdt,
                Cccd = cccd,
                Email = _txtEmail.Text.Trim(),
                GioiTinh = _cboGioiTinh.SelectedItem?.ToString(),
                NgaySinh = ngaySinh,
                TenTaiKhoan = _dtoGoc?.TenTaiKhoan,
                TrangThaiTaiKhoan = _dtoGoc?.TrangThaiTaiKhoan,
                SoLanSaiMatKhau = _dtoGoc?.SoLanSaiMatKhau ?? 0
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
