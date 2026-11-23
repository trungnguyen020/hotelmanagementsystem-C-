using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.Forms
{
    public class FormChiTietNhanSu : Form
    {
        private readonly NhanSuDto? _dtoGoc;
        public NhanSuDto DtoKetQua { get; private set; } = null!;

        private TextBox _txtHoTen = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtSdt = null!;
        private DateTimePicker _dtpNgaySinh = null!;

        private CheckBox _chkTaoTaiKhoan = null!;
        private Label _lblTenDangNhap = null!;
        private TextBox _txtTenDangNhap = null!;
        private Label _lblMatKhau = null!;
        private TextBox _txtMatKhau = null!;

        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        public FormChiTietNhanSu(NhanSuDto? dto = null)
        {
            _dtoGoc = dto;
            KhoiTaoGiaoDien();
            TaiDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = _dtoGoc == null ? "Thêm mới nhân sự" : "Cập nhật nhân sự";
            Width = 500;
            Height = _dtoGoc == null ? 480 : 380;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AppTheme.ApplyFormStyle(this);

            var lblTieuDe = new Label
            {
                Text = _dtoGoc == null ? "THÊM MỚI NHÂN SỰ" : "CẬP NHẬT NHÂN SỰ",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 20),
                AutoSize = true
            };

            int y = 65;
            int lblX = 24;
            int inputX = 160;
            int inputW = 290;
            int spacing = 42;

            var lblHoTen = new Label { Text = "Họ và tên (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblHoTen);
            _txtHoTen = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtHoTen);

            y += spacing;
            var lblCccd = new Label { Text = "CCCD/CMND (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblCccd);
            _txtCccd = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtCccd);

            y += spacing;
            var lblSdt = new Label { Text = "Số điện thoại (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblSdt);
            _txtSdt = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtSdt);

            y += spacing;
            var lblNgaySinh = new Label { Text = "Ngày sinh (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblNgaySinh);
            _dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(inputX, y),
                Width = 160,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            AppTheme.ApplyDateTimePicker(_dtpNgaySinh);

            if (_dtoGoc == null)
            {
                y += spacing;
                _chkTaoTaiKhoan = new CheckBox
                {
                    Text = "Tạo tài khoản Lễ tân cho nhân sự này",
                    Location = new Point(inputX, y),
                    AutoSize = true,
                    Checked = false
                };
                AppTheme.ApplyCheckBox(_chkTaoTaiKhoan);

                y += 35;
                _lblTenDangNhap = new Label { Text = "Tên đăng nhập (*):", Location = new Point(lblX, y + 4), AutoSize = true, Visible = false };
                AppTheme.ApplyLabel(_lblTenDangNhap);
                _txtTenDangNhap = new TextBox { Location = new Point(inputX, y), Width = inputW, Visible = false };
                AppTheme.ApplyTextBox(_txtTenDangNhap);

                y += 38;
                _lblMatKhau = new Label { Text = "Mật khẩu (*):", Location = new Point(lblX, y + 4), AutoSize = true, Visible = false };
                AppTheme.ApplyLabel(_lblMatKhau);
                _txtMatKhau = new TextBox { Location = new Point(inputX, y), Width = inputW, UseSystemPasswordChar = true, Visible = false };
                AppTheme.ApplyTextBox(_txtMatKhau);

                _chkTaoTaiKhoan.CheckedChanged += (_, _) =>
                {
                    bool bật = _chkTaoTaiKhoan.Checked;
                    _lblTenDangNhap.Visible = bật;
                    _txtTenDangNhap.Visible = bật;
                    _lblMatKhau.Visible = bật;
                    _txtMatKhau.Visible = bật;
                };

                Controls.AddRange(new Control[] { _chkTaoTaiKhoan, _lblTenDangNhap, _txtTenDangNhap, _lblMatKhau, _txtMatKhau });
            }

            y += 55;
            _btnLuu = new Button { Text = "💾 Lưu", Location = new Point(inputX + 50, y), Width = 110, Height = 36 };
            _btnHuy = new Button { Text = "✕ Hủy", Location = new Point(inputX + 170, y), Width = 110, Height = 36 };

            AppTheme.ApplyPrimaryButton(_btnLuu);
            AppTheme.ApplySecondaryButton(_btnHuy);

            _btnLuu.Click += BtnLuu_Click;
            _btnHuy.Click += (_, _) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lblTieuDe,
                lblHoTen, _txtHoTen,
                lblCccd, _txtCccd,
                lblSdt, _txtSdt,
                lblNgaySinh, _dtpNgaySinh,
                _btnLuu, _btnHuy
            });
        }

        private void TaiDuLieu()
        {
            if (_dtoGoc == null) return;

            _txtHoTen.Text = _dtoGoc.HoTen;
            _txtCccd.Text = _dtoGoc.Cccd;
            _txtSdt.Text = _dtoGoc.Sdt;
            if (_dtoGoc.NgaySinh != default)
                _dtpNgaySinh.Value = _dtoGoc.NgaySinh.ToDateTime(TimeOnly.MinValue);
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            string hoTen = _txtHoTen.Text.Trim();
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân sự.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtHoTen.Focus();
                return;
            }

            string cccd = _txtCccd.Text.Trim();
            if (string.IsNullOrWhiteSpace(cccd))
            {
                MessageBox.Show("Vui lòng nhập CCCD/CMND.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCccd.Focus();
                return;
            }

            string sdt = _txtSdt.Text.Trim();
            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtSdt.Focus();
                return;
            }

            bool taoTaiKhoan = _chkTaoTaiKhoan != null && _chkTaoTaiKhoan.Checked;
            string? tenDn = null;
            string? matKhau = null;

            if (taoTaiKhoan)
            {
                tenDn = _txtTenDangNhap.Text.Trim();
                matKhau = _txtMatKhau.Text.Trim();
                if (string.IsNullOrWhiteSpace(tenDn))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập cho tài khoản lễ tân.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtTenDangNhap.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(matKhau))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu cho tài khoản lễ tân.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtMatKhau.Focus();
                    return;
                }
            }

            DtoKetQua = new NhanSuDto
            {
                IdNhanSu = _dtoGoc?.IdNhanSu ?? 0,
                HoTen = hoTen,
                Cccd = cccd,
                Sdt = sdt,
                NgaySinh = DateOnly.FromDateTime(_dtpNgaySinh.Value),
                TaoTaiKhoanLeTan = taoTaiKhoan,
                TenDangNhap = tenDn,
                MatKhau = matKhau,
                TenTaiKhoanHienThi = _dtoGoc?.TenTaiKhoanHienThi,
                TrangThaiTaiKhoan = _dtoGoc?.TrangThaiTaiKhoan
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
