using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.Forms
{
    public class FormChiTietPhong : Form
    {
        private readonly PhongDto? _dtoGoc;
        private readonly List<LoaiPhongDto> _dsLoaiPhong;
        public PhongDto DtoKetQua { get; private set; } = null!;

        private TextBox _txtSoPhong = null!;
        private ComboBox _cboLoaiPhong = null!;
        private ComboBox _cboTrangThai = null!;
        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        public FormChiTietPhong(PhongDto? dto, List<LoaiPhongDto> dsLoaiPhong)
        {
            _dtoGoc = dto;
            _dsLoaiPhong = dsLoaiPhong ?? new List<LoaiPhongDto>();
            KhoiTaoGiaoDien();
            TaiDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = _dtoGoc == null ? "Thêm mới phòng" : "Cập nhật phòng";
            Width = 480;
            Height = 330;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AppTheme.ApplyFormStyle(this);

            var lblTieuDe = new Label
            {
                Text = _dtoGoc == null ? "THÊM MỚI PHÒNG" : "CẬP NHẬT PHÒNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 20),
                AutoSize = true
            };

            int y = 65;
            int lblX = 24;
            int inputX = 150;
            int inputW = 280;
            int spacing = 45;

            var lblSoPhong = new Label { Text = "Số phòng (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblSoPhong);
            _txtSoPhong = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtSoPhong);

            y += spacing;
            var lblLoaiPhong = new Label { Text = "Loại phòng (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblLoaiPhong);
            _cboLoaiPhong = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = inputW,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(LoaiPhongDto.TenLoaiPhong),
                ValueMember = nameof(LoaiPhongDto.Id),
                DataSource = _dsLoaiPhong
            };
            AppTheme.ApplyComboBox(_cboLoaiPhong);

            y += spacing;
            var lblTrangThai = new Label { Text = "Trạng thái:", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblTrangThai);
            _cboTrangThai = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = inputW,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboTrangThai.Items.AddRange(new object[] { "Trống", "Đang ở", "Đang dọn", "Bảo trì" });
            _cboTrangThai.SelectedIndex = 0;
            AppTheme.ApplyComboBox(_cboTrangThai);

            y += 65;
            _btnLuu = new Button { Text = "💾 Lưu", Location = new Point(inputX + 40, y), Width = 110, Height = 36 };
            _btnHuy = new Button { Text = "✕ Hủy", Location = new Point(inputX + 160, y), Width = 110, Height = 36 };

            AppTheme.ApplyPrimaryButton(_btnLuu);
            AppTheme.ApplySecondaryButton(_btnHuy);

            _btnLuu.Click += BtnLuu_Click;
            _btnHuy.Click += (_, _) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lblTieuDe,
                lblSoPhong, _txtSoPhong,
                lblLoaiPhong, _cboLoaiPhong,
                lblTrangThai, _cboTrangThai,
                _btnLuu, _btnHuy
            });
        }

        private void TaiDuLieu()
        {
            if (_dtoGoc == null) return;

            _txtSoPhong.Text = _dtoGoc.SoPhong;
            if (_dtoGoc.IdLoaiPhong > 0)
                _cboLoaiPhong.SelectedValue = _dtoGoc.IdLoaiPhong;

            if (!string.IsNullOrEmpty(_dtoGoc.TrangThai))
            {
                int idx = _cboTrangThai.Items.IndexOf(_dtoGoc.TrangThai);
                if (idx >= 0) _cboTrangThai.SelectedIndex = idx;
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            string soPhong = _txtSoPhong.Text.Trim();
            if (string.IsNullOrWhiteSpace(soPhong))
            {
                MessageBox.Show("Vui lòng nhập số phòng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtSoPhong.Focus();
                return;
            }

            if (_cboLoaiPhong.SelectedValue is not int idLoaiPhong || idLoaiPhong <= 0)
            {
                MessageBox.Show("Vui lòng chọn loại phòng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _cboLoaiPhong.Focus();
                return;
            }

            string trangThai = _cboTrangThai.SelectedItem?.ToString() ?? "Trống";
            string tenLoai = (_cboLoaiPhong.SelectedItem as LoaiPhongDto)?.TenLoaiPhong ?? string.Empty;

            DtoKetQua = new PhongDto
            {
                Id = _dtoGoc?.Id ?? 0,
                SoPhong = soPhong,
                IdLoaiPhong = idLoaiPhong,
                TenLoaiPhong = tenLoai,
                TrangThai = trangThai
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
