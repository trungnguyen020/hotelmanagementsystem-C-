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
    /// Thiết kế hiện đại, tự động tính tổng tiền được miễn phí và hỗ trợ click chọn nhanh.
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
            Height = 490;
            MinimumSize = new Size(650, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AppTheme.ApplyFormStyle(this);

            // 1. Top Panel
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                Padding = new Padding(16, 10, 16, 8),
                BackColor = AppTheme.Surface
            };

            var lblTieuDe = new Label
            {
                Text = "🎁 CHỌN DỊCH VỤ ĐƯỢC MIỄN PHÍ",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(14, 10),
                AutoSize = true
            };

            var lblHuongDan = new Label
            {
                Text = "Nhấp vào dòng hoặc tích chọn ô để miễn phí (100%) cho các dịch vụ bên dưới:",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(16, 36),
                AutoSize = true
            };

            var flpTopActions = new FlowLayoutPanel
            {
                Location = new Point(16, 56),
                Size = new Size(400, 30),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };

            _btnChonTatCa = new Button
            {
                Text = "✓ Chọn tất cả",
                Size = new Size(120, 28),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            AppTheme.ApplySecondaryButton(_btnChonTatCa);

            _btnBoChonTatCa = new Button
            {
                Text = "✕ Bỏ chọn tất cả",
                Size = new Size(130, 28),
                Font = AppTheme.FontSmallBold,
                Cursor = Cursors.Hand,
                Margin = new Padding(0)
            };
            AppTheme.ApplySecondaryButton(_btnBoChonTatCa);

            _btnChonTatCa.Click += (_, _) => DatTrangThaiTatCa(true);
            _btnBoChonTatCa.Click += (_, _) => DatTrangThaiTatCa(false);

            flpTopActions.Controls.AddRange(new Control[] { _btnChonTatCa, _btnBoChonTatCa });
            pnlTop.Controls.AddRange(new Control[] { lblTieuDe, lblHuongDan, flpTopActions });

            // 2. Bottom Panel
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                Padding = new Padding(16, 12, 16, 12),
                BackColor = AppTheme.Surface
            };

            _lblTongMienPhi = new Label
            {
                Text = "Tổng tiền miễn phí: 0 VNĐ (0 dịch vụ)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 20),
                AutoSize = true
            };

            var flpButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 2, 0, 0)
            };

            _btnHuy = new Button
            {
                Text = "✕ Hủy",
                Size = new Size(95, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0)
            };
            AppTheme.ApplySecondaryButton(_btnHuy);
            _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            _btnApDung = new Button
            {
                Text = "🎁 Áp dụng miễn phí",
                Size = new Size(165, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0)
            };
            AppTheme.ApplyPrimaryButton(_btnApDung);
            _btnApDung.Click += BtnApDung_Click;

            flpButtons.Controls.AddRange(new Control[] { _btnHuy, _btnApDung });
            pnlBottom.Controls.AddRange(new Control[] { _lblTongMienPhi, flpButtons });

            // 3. DataGridView
            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                BackgroundColor = AppTheme.Surface,
                ReadOnly = false,
                EditMode = DataGridViewEditMode.EditOnEnter
            };
            AppTheme.ApplyDataGridView(_dgv);
            _dgv.ReadOnly = false; // Phải để false để ô Checkbox có thể tương tác
            _dgv.RowTemplate.Height = 36;
            _dgv.ColumnHeadersHeight = 38;

            var colChon = new DataGridViewCheckBoxColumn
            {
                Name = "Chon",
                HeaderText = "Chọn",
                Width = 65,
                ReadOnly = false,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var colTen = new DataGridViewTextBoxColumn
            {
                Name = "TenDichVu",
                HeaderText = "Tên dịch vụ",
                DataPropertyName = "TenDichVu",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 160,
                ReadOnly = true
            };

            var colLoai = new DataGridViewTextBoxColumn
            {
                Name = "LoaiDichVu",
                HeaderText = "Loại DV",
                DataPropertyName = "LoaiDichVu",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var colSL = new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "SL",
                DataPropertyName = "SoLuong",
                Width = 50,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true
            };

            var colDonGia = new DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Đơn giá (VNĐ)",
                DataPropertyName = "DonGia",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                ReadOnly = true
            };

            var colThanhTien = new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành tiền (VNĐ)",
                DataPropertyName = "ThanhTien",
                Width = 130,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" },
                ReadOnly = true
            };

            _dgv.Columns.AddRange(new DataGridViewColumn[] { colChon, colTen, colLoai, colSL, colDonGia, colThanhTien });

            // Hỗ trợ click trực tiếp vào ô Checkbox
            _dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 0)
                {
                    _dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    TinhToanTongTien();
                }
            };

            // Hỗ trợ click vào bất kỳ ô nào trên dòng cũng tự động bật/tắt chọn checkbox
            _dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex != 0)
                {
                    var cell = _dgv.Rows[e.RowIndex].Cells[0];
                    bool current = Convert.ToBoolean(cell.Value);
                    cell.Value = !current;
                    _dgv.EndEdit();
                    TinhToanTongTien();
                }
            };

            _dgv.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex == 0) TinhToanTongTien();
            };

            // Chuẩn docking WinForms: Thêm Top và Bottom trước, sau đó thêm Fill và BringToFront()
            Controls.Add(pnlTop);
            Controls.Add(pnlBottom);
            Controls.Add(_dgv);
            _dgv.BringToFront();
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
            _dgv.EndEdit();
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
