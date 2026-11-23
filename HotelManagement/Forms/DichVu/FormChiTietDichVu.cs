using System;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.Forms
{
    public class FormChiTietDichVu : Form
    {
        private readonly DichVuDto? _dtoGoc;
        public DichVuDto DtoKetQua { get; private set; } = null!;

        private TextBox _txtTen = null!;
        private NumericUpDown _numGia = null!;
        private Button _btnLuu = null!;
        private Button _btnHuy = null!;

        public FormChiTietDichVu(DichVuDto? dto = null)
        {
            _dtoGoc = dto;
            KhoiTaoGiaoDien();
            TaiDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = _dtoGoc == null ? "Thêm mới dịch vụ" : "Cập nhật dịch vụ";
            Width = 460;
            Height = 280;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AppTheme.ApplyFormStyle(this);

            var lblTieuDe = new Label
            {
                Text = _dtoGoc == null ? "THÊM MỚI DỊCH VỤ" : "CẬP NHẬT DỊCH VỤ",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(24, 20),
                AutoSize = true
            };

            int y = 65;
            int lblX = 24;
            int inputX = 140;
            int inputW = 270;
            int spacing = 45;

            var lblTen = new Label { Text = "Tên dịch vụ (*):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblTen);
            _txtTen = new TextBox { Location = new Point(inputX, y), Width = inputW };
            AppTheme.ApplyTextBox(_txtTen);

            y += spacing;
            var lblGia = new Label { Text = "Đơn giá (VNĐ):", Location = new Point(lblX, y + 4), AutoSize = true };
            AppTheme.ApplyLabel(lblGia);
            _numGia = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Width = inputW,
                Maximum = 1_000_000_000,
                ThousandsSeparator = true
            };
            AppTheme.ApplyNumericUpDown(_numGia);

            y += 65;
            _btnLuu = new Button { Text = "💾 Lưu", Location = new Point(inputX + 30, y), Width = 110, Height = 36 };
            _btnHuy = new Button { Text = "✕ Hủy", Location = new Point(inputX + 150, y), Width = 110, Height = 36 };

            AppTheme.ApplyPrimaryButton(_btnLuu);
            AppTheme.ApplySecondaryButton(_btnHuy);

            _btnLuu.Click += BtnLuu_Click;
            _btnHuy.Click += (_, _) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lblTieuDe,
                lblTen, _txtTen,
                lblGia, _numGia,
                _btnLuu, _btnHuy
            });
        }

        private void TaiDuLieu()
        {
            if (_dtoGoc == null) return;

            _txtTen.Text = _dtoGoc.TenDichVu;
            _numGia.Value = _dtoGoc.MucGia;
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            string ten = _txtTen.Text.Trim();
            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Vui lòng nhập tên dịch vụ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtTen.Focus();
                return;
            }

            DtoKetQua = new DichVuDto
            {
                IdDichVu = _dtoGoc?.IdDichVu ?? 0,
                TenDichVu = ten,
                MucGia = _numGia.Value
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
