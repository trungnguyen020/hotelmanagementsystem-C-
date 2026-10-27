using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Form tra cứu booking nâng cao dùng chung cho CheckIn, CheckOut, ThanhToan.
    /// Hỗ trợ tìm kiếm theo nhiều tiêu chí: tên khách hàng, SĐT, CCCD, số phòng, trạng thái.
    /// Khi nhân viên chọn 1 booking và nhấn "Chọn", form trả về mã booking thông qua property MaBookingDaChon.
    /// </summary>
    public partial class FormTraCuuBooking : Form
    {
        private readonly IServiceScope _scope;
        private readonly IDatPhongService _datPhongService;

        /// <summary>
        /// Mã booking mà nhân viên đã chọn. Null nếu chưa chọn hoặc hủy.
        /// </summary>
        public string? MaBookingDaChon { get; private set; }

        // UI Controls - Bộ lọc tìm kiếm
        private TextBox _txtTenKhach = null!;
        private TextBox _txtSdt = null!;
        private TextBox _txtCccd = null!;
        private TextBox _txtSoPhong = null!;
        private ComboBox _cboTrangThai = null!;
        private Button _btnTimKiem = null!;
        private Button _btnXoaBoLoc = null!;

        // UI Controls - Kết quả
        private DataGridView _dgvKetQua = null!;
        private Label _lblSoKetQua = null!;

        // UI Controls - Thao tác
        private Button _btnChon = null!;
        private Button _btnHuy = null!;

        public FormTraCuuBooking()
        {
            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();

            InitializeComponent();

            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) =>
            {
                // Tự động tải booking gần nhất khi mở form
                await TimKiemAsync();
            };
        }

        private void InitializeComponent()
        {
            Text = "🔍 Tra cứu booking nâng cao — Tìm theo tên, SĐT, CCCD, số phòng";
            Size = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(900, 500);
            MaximizeBox = true;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            BackColor = Color.FromArgb(248, 249, 250);

            // ================= 1. GroupBox: Bộ lọc tìm kiếm =================
            var grpBoLoc = new GroupBox
            {
                Text = "Bộ lọc tìm kiếm (nhập 1 hoặc nhiều tiêu chí)",
                Location = new Point(16, 8),
                Size = new Size(1052, 115),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            var fontNormal = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // --- Dòng 1: Tên khách, SĐT, CCCD ---
            var lblTenKhach = new Label { Text = "Tên khách:", Location = new Point(16, 28), AutoSize = true, Font = fontNormal };
            _txtTenKhach = new TextBox { Location = new Point(100, 25), Width = 200, Font = fontNormal };

            var lblSdt = new Label { Text = "SĐT:", Location = new Point(318, 28), AutoSize = true, Font = fontNormal };
            _txtSdt = new TextBox { Location = new Point(360, 25), Width = 150, Font = fontNormal };

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(528, 28), AutoSize = true, Font = fontNormal };
            _txtCccd = new TextBox { Location = new Point(580, 25), Width = 170, Font = fontNormal };

            // --- Dòng 2: Số phòng, Trạng thái, Nút tìm ---
            var lblSoPhong = new Label { Text = "Số phòng:", Location = new Point(16, 64), AutoSize = true, Font = fontNormal };
            _txtSoPhong = new TextBox { Location = new Point(100, 61), Width = 120, Font = fontNormal };

            var lblTrangThai = new Label { Text = "Trạng thái:", Location = new Point(238, 64), AutoSize = true, Font = fontNormal };
            _cboTrangThai = new ComboBox
            {
                Location = new Point(318, 61),
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = fontNormal
            };
            _cboTrangThai.Items.AddRange(new object[]
            {
                "(Tất cả)",
                "Đã xác nhận",
                "Đã check-in",
                "Đã check-out",
                "Đã hủy"
            });
            _cboTrangThai.SelectedIndex = 0;

            _btnTimKiem = new Button
            {
                Text = "🔍 Tìm kiếm",
                Location = new Point(520, 57),
                Size = new Size(120, 34),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnTimKiem.FlatAppearance.BorderSize = 0;
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            _btnXoaBoLoc = new Button
            {
                Text = "Xóa bộ lọc",
                Location = new Point(650, 57),
                Size = new Size(100, 34),
                Cursor = Cursors.Hand,
                Font = fontNormal
            };
            _btnXoaBoLoc.Click += (_, _) => XoaBoLoc();

            // Cho phép Enter để tìm kiếm nhanh ở bất kỳ textbox nào
            foreach (var txt in new[] { _txtTenKhach, _txtSdt, _txtCccd, _txtSoPhong })
            {
                txt.KeyDown += async (_, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        await TimKiemAsync();
                    }
                };
            }

            grpBoLoc.Controls.AddRange(new Control[]
            {
                lblTenKhach, _txtTenKhach,
                lblSdt, _txtSdt,
                lblCccd, _txtCccd,
                lblSoPhong, _txtSoPhong,
                lblTrangThai, _cboTrangThai,
                _btnTimKiem, _btnXoaBoLoc
            });

            // ================= 2. DataGridView: Kết quả =================
            var grpKetQua = new GroupBox
            {
                Text = "Kết quả tìm kiếm",
                Location = new Point(16, 130),
                Size = new Size(1052, 440),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            _dgvKetQua = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                RowTemplate = { Height = 30 },
                Font = fontNormal
            };
            _dgvKetQua.AutoGenerateColumns = false;

            // Cấu hình cột
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaBooking",
                HeaderText = "Mã Booking",
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) }
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThai",
                HeaderText = "Trạng thái",
                FillWeight = 12
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenKhachHang",
                HeaderText = "Khách hàng",
                FillWeight = 16
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SdtKhachHang",
                HeaderText = "SĐT",
                FillWeight = 12
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CccdKhachHang",
                HeaderText = "CCCD",
                FillWeight = 13
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DanhSachSoPhong",
                HeaderText = "Phòng",
                FillWeight = 10
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NgayNhanDuKien",
                HeaderText = "Ngày nhận",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NgayTraDuKien",
                HeaderText = "Ngày trả",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoNguoi",
                HeaderText = "Số khách",
                FillWeight = 7,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Tô màu trạng thái
            _dgvKetQua.CellFormatting += (sender, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (_dgvKetQua.Columns[e.ColumnIndex].DataPropertyName == "TrangThai" && e.Value != null)
                {
                    string tt = e.Value.ToString() ?? "";
                    switch (tt)
                    {
                        case "Đã xác nhận":
                            e.CellStyle.ForeColor = Color.DarkGreen;
                            e.CellStyle.Font = new Font(Font, FontStyle.Bold);
                            break;
                        case "Đã check-in":
                            e.CellStyle.ForeColor = Color.RoyalBlue;
                            e.CellStyle.Font = new Font(Font, FontStyle.Bold);
                            break;
                        case "Đã check-out":
                            e.CellStyle.ForeColor = Color.DimGray;
                            break;
                        case "Đã hủy":
                            e.CellStyle.ForeColor = Color.Crimson;
                            break;
                    }
                }
            };

            // Double-click để chọn nhanh
            _dgvKetQua.CellDoubleClick += (_, _) => ChonBooking();

            grpKetQua.Controls.Add(_dgvKetQua);

            // ================= 3. Panel dưới: Thao tác =================
            var pnlBottom = new Panel
            {
                Location = new Point(16, 578),
                Size = new Size(1052, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _lblSoKetQua = new Label
            {
                Text = "",
                Location = new Point(4, 15),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray,
                Font = new Font(Font, FontStyle.Italic)
            };

            _btnChon = new Button
            {
                Text = "✓ Chọn booking này",
                Size = new Size(170, 40),
                Location = new Point(770, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnChon.FlatAppearance.BorderSize = 0;
            _btnChon.Click += (_, _) => ChonBooking();

            _btnHuy = new Button
            {
                Text = "Hủy",
                Size = new Size(80, 40),
                Location = new Point(950, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            _btnHuy.Click += (_, _) =>
            {
                MaBookingDaChon = null;
                DialogResult = DialogResult.Cancel;
                Close();
            };

            // Khi chọn dòng trong grid, enable nút "Chọn"
            _dgvKetQua.SelectionChanged += (_, _) =>
            {
                _btnChon.Enabled = _dgvKetQua.SelectedRows.Count > 0;
            };

            pnlBottom.Controls.AddRange(new Control[] { _lblSoKetQua, _btnChon, _btnHuy });

            Controls.AddRange(new Control[] { grpBoLoc, grpKetQua, pnlBottom });
        }

        private async Task TimKiemAsync()
        {
            try
            {
                _btnTimKiem.Enabled = false;
                _btnTimKiem.Text = "Đang tìm...";
                _lblSoKetQua.Text = "Đang tra cứu...";
                _lblSoKetQua.ForeColor = Color.DarkSlateGray;

                string? tenKhach = string.IsNullOrWhiteSpace(_txtTenKhach.Text) ? null : _txtTenKhach.Text.Trim();
                string? sdt = string.IsNullOrWhiteSpace(_txtSdt.Text) ? null : _txtSdt.Text.Trim();
                string? cccd = string.IsNullOrWhiteSpace(_txtCccd.Text) ? null : _txtCccd.Text.Trim();
                string? soPhong = string.IsNullOrWhiteSpace(_txtSoPhong.Text) ? null : _txtSoPhong.Text.Trim();
                string? trangThai = _cboTrangThai.SelectedIndex <= 0 ? null : _cboTrangThai.SelectedItem?.ToString();

                var ketQua = await _datPhongService.TimKiemBookingAsync(tenKhach, sdt, cccd, soPhong, trangThai);

                _dgvKetQua.DataSource = ketQua;
                _lblSoKetQua.Text = $"Tìm thấy {ketQua.Count} booking.";
                _lblSoKetQua.ForeColor = ketQua.Count > 0 ? Color.DarkGreen : Color.DarkOrange;

                if (ketQua.Count == 0)
                {
                    _lblSoKetQua.Text = "Không tìm thấy booking nào phù hợp. Vui lòng thử tiêu chí khác.";
                }

                _btnChon.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm booking: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblSoKetQua.Text = "Có lỗi xảy ra khi tra cứu.";
                _lblSoKetQua.ForeColor = Color.Crimson;
            }
            finally
            {
                _btnTimKiem.Enabled = true;
                _btnTimKiem.Text = "🔍 Tìm kiếm";
            }
        }

        private void ChonBooking()
        {
            if (_dgvKetQua.SelectedRows.Count == 0) return;

            if (_dgvKetQua.SelectedRows[0].DataBoundItem is BookingTimKiemDto dto)
            {
                MaBookingDaChon = dto.MaBooking;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void XoaBoLoc()
        {
            _txtTenKhach.Clear();
            _txtSdt.Clear();
            _txtCccd.Clear();
            _txtSoPhong.Clear();
            _cboTrangThai.SelectedIndex = 0;
            _txtTenKhach.Focus();
        }
    }
}
