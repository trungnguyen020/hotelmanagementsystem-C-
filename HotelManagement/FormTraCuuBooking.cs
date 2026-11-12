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
        private readonly IQrCodeService _qrCodeService;

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
        private Button _btnQuetQr = null!;

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
            _qrCodeService = _scope.ServiceProvider.GetRequiredService<IQrCodeService>();

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
            MinimumSize = new Size(980, 520);
            MaximizeBox = true;
            MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            // ================= 1. GroupBox: Bộ lọc tìm kiếm =================
            var grpBoLoc = new GroupBox
            {
                Text = "Bộ lọc tìm kiếm (nhập 1 hoặc nhiều tiêu chí)",
                Location = new Point(16, 8),
                Size = new Size(1052, 125),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary
            };

            // --- Dòng 1: Tên khách, SĐT, CCCD ---
            var lblTenKhach = new Label { Text = "Tên khách:", Location = new Point(16, 28), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _txtTenKhach = new TextBox { Location = new Point(100, 25), Width = 200 };
            AppTheme.ApplyTextBox(_txtTenKhach);

            var lblSdt = new Label { Text = "SĐT:", Location = new Point(318, 28), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _txtSdt = new TextBox { Location = new Point(360, 25), Width = 150 };
            AppTheme.ApplyTextBox(_txtSdt);

            var lblCccd = new Label { Text = "CCCD:", Location = new Point(528, 28), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _txtCccd = new TextBox { Location = new Point(580, 25), Width = 170 };
            AppTheme.ApplyTextBox(_txtCccd);

            // --- Dòng 2: Số phòng, Trạng thái, Nút tìm ---
            var lblSoPhong = new Label { Text = "Số phòng:", Location = new Point(16, 68), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _txtSoPhong = new TextBox { Location = new Point(100, 65), Width = 120 };
            AppTheme.ApplyTextBox(_txtSoPhong);

            var lblTrangThai = new Label { Text = "Trạng thái:", Location = new Point(238, 68), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _cboTrangThai = new ComboBox
            {
                Location = new Point(318, 65),
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            AppTheme.ApplyComboBox(_cboTrangThai);
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
                Location = new Point(520, 61),
                Size = new Size(125, 34)
            };
            AppTheme.ApplyPrimaryButton(_btnTimKiem);
            _btnTimKiem.Click += async (_, _) => await TimKiemAsync();

            _btnXoaBoLoc = new Button
            {
                Text = "Xóa bộ lọc",
                Location = new Point(655, 61),
                Size = new Size(110, 34)
            };
            AppTheme.ApplySecondaryButton(_btnXoaBoLoc);
            _btnXoaBoLoc.Click += (_, _) => XoaBoLoc();

            _btnQuetQr = new Button
            {
                Text = "📷 Quét mã QR",
                Location = new Point(775, 61),
                Size = new Size(145, 34)
            };
            AppTheme.ApplyPrimaryButton(_btnQuetQr);
            _btnQuetQr.Click += async (_, _) => await QuetMaQrAsync();

            // Canh ô nhập theo mép phải label để không bị đè khi font/DPI thay đổi
            AppTheme.AlignInputAfterLabel(lblTenKhach, _txtTenKhach, 8);
            lblSdt.Left = _txtTenKhach.Right + 24;
            AppTheme.AlignInputAfterLabel(lblSdt, _txtSdt, 8);
            lblCccd.Left = _txtSdt.Right + 24;
            AppTheme.AlignInputAfterLabel(lblCccd, _txtCccd, 8);

            AppTheme.AlignInputAfterLabel(lblSoPhong, _txtSoPhong, 8);
            lblTrangThai.Left = _txtSoPhong.Right + 24;
            AppTheme.AlignInputAfterLabel(lblTrangThai, _cboTrangThai, 8);
            _btnTimKiem.Left = _cboTrangThai.Right + 24;
            _btnXoaBoLoc.Left = _btnTimKiem.Right + 10;
            _btnQuetQr.Left = _btnXoaBoLoc.Right + 10;

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
                _btnTimKiem, _btnXoaBoLoc, _btnQuetQr
            });

            // ================= 2. DataGridView: Kết quả =================
            var grpKetQua = new GroupBox
            {
                Text = "Kết quả tìm kiếm",
                Location = new Point(16, 140),
                Size = new Size(1052, 430),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary
            };

            _dgvKetQua = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(_dgvKetQua);
            _dgvKetQua.AutoGenerateColumns = false;

            // Cấu hình cột
            _dgvKetQua.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaBooking",
                HeaderText = "Mã Booking",
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle { Font = AppTheme.FontBodyBold }
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

            // Tô màu trạng thái (font in đậm tạo 1 lần, dùng lại cho mọi ô)
            var fontTrangThaiBold = new Font(AppTheme.FontBody, FontStyle.Bold);
            _dgvKetQua.CellFormatting += (sender, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (_dgvKetQua.Columns[e.ColumnIndex].DataPropertyName != "TrangThai" || e.Value == null) return;

                switch (e.Value.ToString())
                {
                    case "Đã xác nhận":
                        e.CellStyle.ForeColor = AppTheme.StatusSuccessText;
                        e.CellStyle.Font = fontTrangThaiBold;
                        break;
                    case "Đã check-in":
                        e.CellStyle.ForeColor = AppTheme.PrimaryActive;
                        e.CellStyle.Font = fontTrangThaiBold;
                        break;
                    case "Đã check-out":
                        e.CellStyle.ForeColor = AppTheme.TextMuted;
                        break;
                    case "Đã hủy":
                        e.CellStyle.ForeColor = AppTheme.StatusDangerText;
                        break;
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
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontCaption
            };

            _btnChon = new Button
            {
                Text = "✓ Chọn booking này",
                Size = new Size(180, 40),
                Location = new Point(765, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Enabled = false
            };
            AppTheme.ApplyPrimaryButton(_btnChon);
            _btnChon.Click += (_, _) => ChonBooking();

            _btnHuy = new Button
            {
                Text = "Hủy",
                Size = new Size(90, 40),
                Location = new Point(955, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnHuy);
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
                _lblSoKetQua.ForeColor = AppTheme.TextSecondary;

                string? tenKhach = string.IsNullOrWhiteSpace(_txtTenKhach.Text) ? null : _txtTenKhach.Text.Trim();
                string? sdt = string.IsNullOrWhiteSpace(_txtSdt.Text) ? null : _txtSdt.Text.Trim();
                string? cccd = string.IsNullOrWhiteSpace(_txtCccd.Text) ? null : _txtCccd.Text.Trim();
                string? soPhong = string.IsNullOrWhiteSpace(_txtSoPhong.Text) ? null : _txtSoPhong.Text.Trim();
                string? trangThai = _cboTrangThai.SelectedIndex <= 0 ? null : _cboTrangThai.SelectedItem?.ToString();

                var ketQua = await _datPhongService.TimKiemBookingAsync(tenKhach, sdt, cccd, soPhong, trangThai);

                _dgvKetQua.DataSource = ketQua;
                _lblSoKetQua.Text = $"Tìm thấy {ketQua.Count} booking.";
                _lblSoKetQua.ForeColor = ketQua.Count > 0 ? AppTheme.StatusSuccessText : AppTheme.StatusWarningText;

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
                _lblSoKetQua.ForeColor = AppTheme.StatusDangerText;
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

        private async Task QuetMaQrAsync()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Chọn ảnh mã QR Booking để tra cứu nhanh",
                Filter = "Tệp hình ảnh (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Tất cả tệp (*.*)|*.*"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var bmp = new Bitmap(ofd.FileName);
                string? maBooking = _qrCodeService.DocMaQrTuBitmap(bmp);

                if (string.IsNullOrWhiteSpace(maBooking))
                {
                    MessageBox.Show("Không nhận diện được mã QR hợp lệ từ tệp ảnh này.\nVui lòng đảm bảo ảnh chụp rõ nét mã QR.",
                        "Không đọc được QR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                maBooking = maBooking.Trim();
                _lblSoKetQua.Text = $"Đã quét mã: {maBooking}. Đang kiểm tra hệ thống...";
                _lblSoKetQua.ForeColor = AppTheme.Primary;

                var bookingChiTiet = await _datPhongService.LayChiTietTheoMaBookingAsync(maBooking);
                if (bookingChiTiet == null)
                {
                    MessageBox.Show($"Mã QR chứa mã booking '{maBooking}' nhưng không tìm thấy thông tin trên hệ thống.",
                        "Không tìm thấy booking", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dto = new BookingTimKiemDto
                {
                    IdDatPhong = bookingChiTiet.IdDatPhong,
                    MaBooking = bookingChiTiet.MaBooking,
                    TrangThai = bookingChiTiet.TrangThai,
                    TenKhachHang = bookingChiTiet.TenKhachHang,
                    SdtKhachHang = bookingChiTiet.SdtKhachHang,
                    CccdKhachHang = bookingChiTiet.CccdKhachHang,
                    NgayDat = bookingChiTiet.NgayDat,
                    NgayNhanDuKien = bookingChiTiet.NgayNhanDuKien,
                    NgayTraDuKien = bookingChiTiet.NgayTraDuKien,
                    SoPhong = bookingChiTiet.DanhSachPhong?.Count ?? 0,
                    DanhSachSoPhong = string.Join(", ", (bookingChiTiet.DanhSachPhong ?? new List<ChiTietPhongDatDto>()).Select(p => p.SoPhong)),
                    SoNguoi = bookingChiTiet.SoNguoi
                };

                _dgvKetQua.DataSource = new List<BookingTimKiemDto> { dto };
                if (_dgvKetQua.Rows.Count > 0)
                {
                    _dgvKetQua.Rows[0].Selected = true;
                    _btnChon.Enabled = true;
                }

                _lblSoKetQua.Text = $"✓ Quét mã QR thành công! Booking: '{maBooking}' — Khách: {bookingChiTiet.TenKhachHang}";
                _lblSoKetQua.ForeColor = AppTheme.StatusSuccessText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi giải mã QR: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
