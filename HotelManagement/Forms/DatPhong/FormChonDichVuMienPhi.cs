using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Dialog cho phép người dùng/thu ngân tích chọn cụ thể các dịch vụ được miễn phí trong hóa đơn.
    /// </summary>
    public class FormChonDichVuMienPhi : Form
    {
        private readonly List<ChiTietDichVuDto> _danhSachDichVu;
        private DataGridView _dgv = null!;
        private Label _lblTongMienPhi = null!;
        private Button _btnChonTatCa = null!;
        private Button _btnBoChonTatCa = null!;
        private Button _btnApDung = null!;
        private Button _btnHuy = null!;

        public decimal TongTienMienPhi { get; private set; }
        public List<ChiTietDichVuDto> DichVuDuocMienPhi { get; private set; } = new();
        public string MoTaKhuyenMai { get; private set; } = string.Empty;

        public FormChonDichVuMienPhi(List<ChiTietDichVuDto> danhSachDichVu)
        {
            _danhSachDichVu = danhSachDichVu ?? new List<ChiTietDichVuDto>();
            KhoiTaoGiaoDien();
            NapDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Chọn dịch vụ miễn phí";
            Width = 720;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AppTheme.ApplyFormStyle(this);

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                Padding = new Padding(16, 12, 16, 8),
                BackColor = AppTheme.Surface
            };

            var lblTieuDe = new Label
            {
                Text = "🎁 CHỌN DỊCH VỤ ĐƯỢC MIỄN PHÍ",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 12),
                AutoSize = true
            };

            var lblHuongDan = new Label
            {
                Text = "Tích chọn 1 hoặc nhiều dịch vụ bên dưới để áp dụng giảm giá 100% cho các dịch vụ đó:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(16, 38),
                AutoSize = true
            };

            _btnChonTatCa = new Button
            {
                Text = "✓ Chọn tất cả",
                Size = new Size(110, 28),
                Location = new Point(16, 56),
                Font = AppTheme.FontSmallBold
            };
            AppTheme.ApplySecondaryButton(_btnChonTatCa);

            _btnBoChonTatCa = new Button
            {
                Text = "✕ Bỏ chọn tất cả",
                Size = new Size(120, 28),
                Location = new Point(134, 56),
                Font = AppTheme.FontSmallBold
            };
            AppTheme.ApplySecondaryButton(_btnBoChonTatCa);

            _btnChonTatCa.Click += (_, _) => DatTrangThaiTatCa(true);
            _btnBoChonTatCa.Click += (_, _) => DatTrangThaiTatCa(false);

            pnlTop.Controls.AddRange(new Control[] { lblTieuDe, lblHuongDan, _btnChonTatCa, _btnBoChonTatCa });

            // DataGridView
            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                BackgroundColor = AppTheme.Surface
            };
            AppTheme.ApplyDataGridView(_dgv);
            _dgv.RowTemplate.Height = 34;
            _dgv.ColumnHeadersHeight = 36;

            var colChon = new DataGridViewCheckBoxColumn
            {
                Name = "Chon",
                HeaderText = "Chọn",
                Width = 55,
                ReadOnly = false
            };

            var colTen = new DataGridViewTextBoxColumn
            {
                Name = "TenDichVu",
                HeaderText = "Tên dịch vụ",
                DataPropertyName = "TenDichVu",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 150,
                ReadOnly = true
            };

            var colLoai = new DataGridViewTextBoxColumn
            {
                Name = "LoaiDichVu",
                HeaderText = "Loại dịch vụ",
                DataPropertyName = "LoaiDichVu",
                Width = 110,
                ReadOnly = true
            };

            var colSL = new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "SL",
                DataPropertyName = "SoLuong",
                Width = 45,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true
            };

            var colDonGia = new DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Đơn giá (VNĐ)",
                DataPropertyName = "DonGia",
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                ReadOnly = true
            };

            var colThanhTien = new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành tiền (VNĐ)",
                DataPropertyName = "ThanhTien",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                ReadOnly = true
            };

            _dgv.Columns.AddRange(new DataGridViewColumn[] { colChon, colTen, colLoai, colSL, colDonGia, colThanhTien });

            _dgv.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex == 0) TinhToanTongTien();
            };
            _dgv.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (_dgv.IsCurrentCellDirty && _dgv.CurrentCell.ColumnIndex == 0)
                {
                    _dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            // Bottom Panel
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(16, 12, 16, 12),
                BackColor = AppTheme.Surface
            };

            _lblTongMienPhi = new Label
            {
                Text = "Tổng tiền miễn phí: 0 VNĐ",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 18),
                AutoSize = true
            };

            _btnApDung = new Button
            {
                Text = "🎁 Áp dụng miễn phí",
                Size = new Size(160, 36),
                Location = new Point(Width - 310, 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplyPrimaryButton(_btnApDung);
            _btnApDung.Click += BtnApDung_Click;

            _btnHuy = new Button
            {
                Text = "✕ Hủy",
                Size = new Size(100, 36),
                Location = new Point(Width - 140, 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnHuy);
            _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            pnlBottom.Controls.AddRange(new Control[] { _lblTongMienPhi, _btnApDung, _btnHuy });

            Controls.Add(_dgv);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
        }

        private void NapDuLieu()
        {
            _dgv.Rows.Clear();
            foreach (var item in _danhSachDichVu)
            {
                int rowIdx = _dgv.Rows.Add(false, item.TenDichVu, item.LoaiDichVu, item.SoLuong, item.DonGia, item.ThanhTien);
                _dgv.Rows[rowIdx].Tag = item;
            }
            TinhToanTongTien();
        }

        private void DatTrangThaiTatCa(bool check)
        {
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                row.Cells["Chon"].Value = check;
            }
            TinhToanTongTien();
        }

        private void TinhToanTongTien()
        {
            decimal tong = 0;
            int count = 0;
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Chon"].Value) && row.Tag is ChiTietDichVuDto dv)
                {
                    tong += dv.ThanhTien;
                    count++;
                }
            }
            _lblTongMienPhi.Text = $"Tổng tiền miễn phí: {tong:N0} VNĐ ({count}/{_danhSachDichVu.Count} dịch vụ)";
        }

        private void BtnApDung_Click(object? sender, EventArgs e)
        {
            DichVuDuocMienPhi.Clear();
            TongTienMienPhi = 0;

            foreach (DataGridViewRow row in _dgv.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Chon"].Value) && row.Tag is ChiTietDichVuDto dv)
                {
                    DichVuDuocMienPhi.Add(dv);
                    TongTienMienPhi += dv.ThanhTien;
                }
            }

            if (!DichVuDuocMienPhi.Any())
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất 1 dịch vụ để áp dụng miễn phí.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DichVuDuocMienPhi.Count == _danhSachDichVu.Count)
            {
                MoTaKhuyenMai = $"✓ Đã miễn phí: 100% tất cả dịch vụ (-{TongTienMienPhi:N0} đ)";
            }
            else
            {
                var tenDSDuocChon = DichVuDuocMienPhi.Select(d => d.TenDichVu).ToList();
                string listStr = string.Join(", ", tenDSDuocChon);
                if (listStr.Length > 45) listStr = listStr.Substring(0, 42) + "...";
                MoTaKhuyenMai = $"✓ Miễn phí dịch vụ: {listStr} (-{TongTienMienPhi:N0} đ)";
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
