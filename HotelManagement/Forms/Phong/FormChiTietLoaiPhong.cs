using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.Forms
{
    public class FormChiTietLoaiPhong : Form
    {
        private readonly LoaiPhongDto? _dtoGoc;
        public LoaiPhongDto DtoKetQua { get; private set; } = null!;

        private TextBox _txtTen = null!;
        private NumericUpDown _numGiaNgay = null!;
        private NumericUpDown _numGiaGio = null!;
        private NumericUpDown _numSucChua = null!;
        private TextBox _txtMoTa = null!;
        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        public FormChiTietLoaiPhong(LoaiPhongDto? dto = null)
        {
            _dtoGoc = dto;
            KhoiTaoGiaoDien();
            TaiDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = _dtoGoc == null ? "Thêm mới loại phòng" : "Cập nhật loại phòng";
            Width = 560;
            Height = 440;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AppTheme.ApplyFormStyle(this);

            var lblTieuDe = new Label
            {
                Text = _dtoGoc == null ? "THÊM MỚI LOẠI PHÒNG" : "CẬP NHẬT LOẠI PHÒNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 20),
                AutoSize = true
            };

            int y = 65;
            int lblX = 24;
            int inputX = 210;
            int inputW = 300;
            int spacing = 45;

            var lblTen = new Label { Text = "Tên loại phòng (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblTen);
            _txtTen = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtTen);

            y += spacing;
            var lblGiaNgay = new Label { Text = "Giá theo ngày (VNĐ):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblGiaNgay);
            _numGiaNgay = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = inputW,
                Maximum = 1_000_000_000,
                ThousandsSeparator = true
            };
            AppTheme.ApplyNumericUpDown(_numGiaNgay);

            y += spacing;
            var lblGiaGio = new Label { Text = "Giá theo giờ (VNĐ):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblGiaGio);
            _numGiaGio = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = inputW,
                Maximum = 1_000_000_000,
                ThousandsSeparator = true
            };
            AppTheme.ApplyNumericUpDown(_numGiaGio);

            y += spacing;
            var lblSucChua = new Label { Text = "Sức chứa (người):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblSucChua);
            _numSucChua = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = 120,
                Minimum = 1,
                Maximum = 50,
                Value = 2
            };
            AppTheme.ApplyNumericUpDown(_numSucChua);

            y += spacing;
            var lblMoTa = new Label { Text = "Mô tả:", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblMoTa);
            _txtMoTa = new TextBox
            {
                Location = new Point(inputX, y),
                Width = inputW,
                Height = 65,
                Multiline = true
            };
            AppTheme.ApplyTextBox(_txtMoTa);

            y += 85;
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
                lblGiaNgay, _numGiaNgay,
                lblGiaGio, _numGiaGio,
                lblSucChua, _numSucChua,
                lblMoTa, _txtMoTa,
                _btnLuu, _btnHuy
            });
        }

        private void TaiDuLieu()
        {
            if (_dtoGoc == null) return;

            _txtTen.Text = _dtoGoc.TenLoaiPhong;
            _numGiaNgay.Value = _dtoGoc.Gia;
            _numGiaGio.Value = _dtoGoc.GiaTheoGio;
            _numSucChua.Value = _dtoGoc.SucChua > 0 ? _dtoGoc.SucChua : 1;
            _txtMoTa.Text = _dtoGoc.MoTa ?? string.Empty;
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            string ten = _txtTen.Text.Trim();
            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Vui lòng nhập tên loại phòng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtTen.Focus();
                return;
            }

            DtoKetQua = new LoaiPhongDto
            {
                Id = _dtoGoc?.Id ?? 0,
                TenLoaiPhong = ten,
                Gia = _numGiaNgay.Value,
                GiaTheoGio = _numGiaGio.Value,
                SucChua = (int)_numSucChua.Value,
                MoTa = _txtMoTa.Text.Trim()
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
