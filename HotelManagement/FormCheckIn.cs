using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Check-in bằng tra mã booking (DCKLTTQ-71).
    /// Hỗ trợ tra cứu thông tin đặt phòng, kiểm tra trạng thái phòng và thực hiện check-in vào hệ thống.
    /// </summary>
    public class FormCheckIn : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private readonly IServiceScope _scope;
        private readonly IDatPhongService _datPhongService;
        private readonly string? _maBookingKhoiTao;

        // UI Controls - Tìm kiếm
        private TextBox _txtMaBooking = null!;
        private Button _btnTim = null!;
        private Button _btnLamMoi = null!;

        // UI Controls - Thông tin booking & khách hàng
        private Label _lblMaBookingVal = null!;
        private Label _lblTrangThaiVal = null!;
        private Label _lblKhachHangVal = null!;
        private Label _lblSdtVal = null!;
        private Label _lblCccdVal = null!;
        private Label _lblEmailVal = null!;
        private Label _lblNgayDatVal = null!;
        private Label _lblNgayNhanDuKienVal = null!;
        private Label _lblNgayTraDuKienVal = null!;
        private Label _lblNgayNhanThucTeVal = null!;
        private Label _lblSoNguoiVal = null!;

        // UI Controls - Danh sách phòng
        private DataGridView _dgvPhong = null!;

        // UI Controls - Thao tác
        private Label _lblThongBao = null!;
        private Button _btnCheckIn = null!;
        private Button _btnDong = null!;

        // Trạng thái hiện tại
        private DatPhongChiTietDto? _currentBooking;

        public FormCheckIn(TaiKhoan taiKhoanDangNhap, string? maBookingKhoiTao = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _maBookingKhoiTao = maBookingKhoiTao;

            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();

            KhoiTaoGiaoDien();

            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) =>
            {
                if (!string.IsNullOrWhiteSpace(_maBookingKhoiTao))
                {
                    _txtMaBooking.Text = _maBookingKhoiTao.Trim();
                    await TraCuuBookingAsync();
                }
            };
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Check-in khách sạn — Tra cứu mã booking";
            Width = 960;
            Height = 680;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(900, 640);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // ================= 1. GroupBox: Tra cứu Booking =================
            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu mã booking",
                Location = new Point(16, 12),
                Size = new Size(912, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblMaBooking = new Label
            {
                Text = "Mã Booking:",
                Location = new Point(16, 28),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold)
            };

            _txtMaBooking = new TextBox
            {
                Location = new Point(115, 25),
                Width = 260,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular)
            };
            _txtMaBooking.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await TraCuuBookingAsync();
                }
            };

            _btnTim = new Button
            {
                Text = "Tra cứu",
                Location = new Point(390, 23),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(Font, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnTim.FlatAppearance.BorderSize = 0;
            _btnTim.Click += async (_, _) => await TraCuuBookingAsync();

            _btnLamMoi = new Button
            {
                Text = "Làm mới",
                Location = new Point(500, 23),
                Size = new Size(90, 32),
                Cursor = Cursors.Hand
            };
            _btnLamMoi.Click += (_, _) => XoaTrangForm();

            grpTraCuu.Controls.AddRange(new Control[] { lblMaBooking, _txtMaBooking, _btnTim, _btnLamMoi });

            // ================= 2. GroupBox: Thông tin Đặt phòng & Khách hàng =================
            var grpThongTin = new GroupBox
            {
                Text = "Thông tin đặt phòng & Khách hàng",
                Location = new Point(16, 88),
                Size = new Size(912, 190),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int col1L = 20, col1V = 160;
            int col2L = 470, col2V = 610;

            // Dòng 1: Mã booking & Khách hàng
            grpThongTin.Controls.Add(new Label { Text = "Mã booking:", Location = new Point(col1L, 24), AutoSize = true, ForeColor = Color.DimGray });
            _lblMaBookingVal = new Label { Text = "-", Location = new Point(col1V, 24), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            grpThongTin.Controls.Add(_lblMaBookingVal);

            grpThongTin.Controls.Add(new Label { Text = "Khách hàng:", Location = new Point(col2L, 24), AutoSize = true, ForeColor = Color.DimGray });
            _lblKhachHangVal = new Label { Text = "-", Location = new Point(col2V, 24), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            grpThongTin.Controls.Add(_lblKhachHangVal);

            // Dòng 2: Ngày đặt & Số điện thoại
            grpThongTin.Controls.Add(new Label { Text = "Ngày đặt:", Location = new Point(col1L, 50), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayDatVal = new Label { Text = "-", Location = new Point(col1V, 50), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayDatVal);

            grpThongTin.Controls.Add(new Label { Text = "Số điện thoại:", Location = new Point(col2L, 50), AutoSize = true, ForeColor = Color.DimGray });
            _lblSdtVal = new Label { Text = "-", Location = new Point(col2V, 50), AutoSize = true };
            grpThongTin.Controls.Add(_lblSdtVal);

            // Dòng 3: Trạng thái & CCCD
            grpThongTin.Controls.Add(new Label { Text = "Trạng thái booking:", Location = new Point(col1L, 76), AutoSize = true, ForeColor = Color.DimGray });
            _lblTrangThaiVal = new Label { Text = "-", Location = new Point(col1V, 74), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            grpThongTin.Controls.Add(_lblTrangThaiVal);

            grpThongTin.Controls.Add(new Label { Text = "CCCD/CMND:", Location = new Point(col2L, 76), AutoSize = true, ForeColor = Color.DimGray });
            _lblCccdVal = new Label { Text = "-", Location = new Point(col2V, 76), AutoSize = true };
            grpThongTin.Controls.Add(_lblCccdVal);

            // Dòng 4: Ngày nhận dự kiến & Email
            grpThongTin.Controls.Add(new Label { Text = "Ngày nhận (dự kiến):", Location = new Point(col1L, 102), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayNhanDuKienVal = new Label { Text = "-", Location = new Point(col1V, 102), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayNhanDuKienVal);

            grpThongTin.Controls.Add(new Label { Text = "Email:", Location = new Point(col2L, 102), AutoSize = true, ForeColor = Color.DimGray });
            _lblEmailVal = new Label { Text = "-", Location = new Point(col2V, 102), AutoSize = true };
            grpThongTin.Controls.Add(_lblEmailVal);

            // Dòng 5: Ngày trả dự kiến & Số khách
            grpThongTin.Controls.Add(new Label { Text = "Ngày trả (dự kiến):", Location = new Point(col1L, 128), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayTraDuKienVal = new Label { Text = "-", Location = new Point(col1V, 128), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayTraDuKienVal);

            grpThongTin.Controls.Add(new Label { Text = "Số khách đăng ký:", Location = new Point(col2L, 128), AutoSize = true, ForeColor = Color.DimGray });
            _lblSoNguoiVal = new Label { Text = "-", Location = new Point(col2V, 128), AutoSize = true };
            grpThongTin.Controls.Add(_lblSoNguoiVal);

            // Dòng 6: Ngày nhận thực tế
            grpThongTin.Controls.Add(new Label { Text = "Ngày nhận thực tế:", Location = new Point(col1L, 154), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayNhanThucTeVal = new Label { Text = "-", Location = new Point(col1V, 154), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayNhanThucTeVal);

            // ================= 3. GroupBox: Danh sách phòng đặt =================
            var grpDanhSachPhong = new GroupBox
            {
                Text = "Danh sách phòng trong booking",
                Location = new Point(16, 286),
                Size = new Size(912, 270),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _dgvPhong = new DataGridView
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
                RowTemplate = { Height = 32 }
            };

            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdPhong", HeaderText = "ID", Visible = false });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoPhong", HeaderText = "Số phòng", FillWeight = 20 });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenLoaiPhong", HeaderText = "Loại phòng", FillWeight = 30 });
            
            var colGia = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DonGia",
                HeaderText = "Đơn giá (VNĐ/đêm)",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            };
            _dgvPhong.Columns.Add(colGia);

            var colTrangThai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThaiPhongHienTai",
                HeaderText = "Trạng thái phòng hiện tại",
                FillWeight = 25
            };
            _dgvPhong.Columns.Add(colTrangThai);

            _dgvPhong.CellFormatting += DgvPhong_CellFormatting;

            grpDanhSachPhong.Controls.Add(_dgvPhong);

            // ================= 4. Panel dưới: Thao tác & Thông báo =================
            var pnlBottom = new Panel
            {
                Location = new Point(16, 566),
                Size = new Size(912, 60),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _lblThongBao = new Label
            {
                Text = "Vui lòng nhập mã booking và nhấn 'Tra cứu' để kiểm tra thông tin.",
                Location = new Point(4, 18),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray,
                Font = new Font(Font, FontStyle.Italic)
            };

            _btnCheckIn = new Button
            {
                Text = "Xác nhận Check-in",
                Location = new Point(680, 10),
                Size = new Size(150, 40),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnCheckIn.FlatAppearance.BorderSize = 0;
            _btnCheckIn.Click += async (_, _) => await ThucHienCheckInAsync();

            _btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(840, 10),
                Size = new Size(70, 40),
                Cursor = Cursors.Hand
            };
            _btnDong.Click += (_, _) => Close();

            pnlBottom.Controls.AddRange(new Control[] { _lblThongBao, _btnCheckIn, _btnDong });

            // Thêm tất cả vào Form
            Controls.AddRange(new Control[] { grpTraCuu, grpThongTin, grpDanhSachPhong, pnlBottom });
        }

        private void DgvPhong_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Tô màu cột Trạng thái phòng hiện tại
            if (_dgvPhong.Columns[e.ColumnIndex].DataPropertyName == "TrangThaiPhongHienTai" && e.Value != null)
            {
                string status = e.Value.ToString() ?? "";
                switch (status)
                {
                    case "Trống":
                        e.CellStyle.ForeColor = Color.DarkGreen;
                        e.CellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;
                    case "Đang ở":
                        e.CellStyle.ForeColor = Color.Crimson;
                        e.CellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;
                    case "Đang dọn":
                        e.CellStyle.ForeColor = Color.DarkOrange;
                        e.CellStyle.Font = new Font(Font, FontStyle.Bold);
                        break;
                    case "Bảo trì":
                        e.CellStyle.ForeColor = Color.Gray;
                        e.CellStyle.Font = new Font(Font, FontStyle.Italic);
                        break;
                }
            }
        }

        private async Task TraCuuBookingAsync()
        {
            string ma = _txtMaBooking.Text.Trim();
            if (string.IsNullOrWhiteSpace(ma))
            {
                MessageBox.Show("Vui lòng nhập mã booking cần tra cứu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtMaBooking.Focus();
                return;
            }

            try
            {
                _btnTim.Enabled = false;
                _btnTim.Text = "Đang tìm...";
                _lblThongBao.Text = "Đang tra cứu dữ liệu booking...";
                _lblThongBao.ForeColor = Color.DarkSlateGray;

                var booking = await _datPhongService.LayChiTietTheoMaBookingAsync(ma);
                if (booking == null)
                {
                    XoaTrangThongTin();
                    _lblThongBao.Text = $"Không tìm thấy booking với mã '{ma}'. Vui lòng kiểm tra lại.";
                    _lblThongBao.ForeColor = Color.Crimson;
                    MessageBox.Show($"Không tìm thấy đơn đặt phòng với mã booking '{ma}'.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _currentBooking = booking;
                HienThiThongTin(booking);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tra cứu booking: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnTim.Enabled = true;
                _btnTim.Text = "Tra cứu";
            }
        }

        private void HienThiThongTin(DatPhongChiTietDto dto)
        {
            _lblMaBookingVal.Text = dto.MaBooking;
            _lblTrangThaiVal.Text = dto.TrangThai;
            CapNhatMauTrangThai(dto.TrangThai);

            _lblKhachHangVal.Text = dto.TenKhachHang;
            _lblNgayDatVal.Text = dto.NgayDat.ToString("dd/MM/yyyy HH:mm");
            _lblSdtVal.Text = dto.SdtKhachHang;
            _lblCccdVal.Text = dto.CccdKhachHang;
            _lblEmailVal.Text = dto.EmailKhachHang;

            _lblNgayNhanDuKienVal.Text = dto.NgayNhanDuKien.ToString("dd/MM/yyyy");
            _lblNgayTraDuKienVal.Text = dto.NgayTraDuKien.ToString("dd/MM/yyyy");
            _lblNgayNhanThucTeVal.Text = dto.NgayNhanThucTe.HasValue ? dto.NgayNhanThucTe.Value.ToString("dd/MM/yyyy HH:mm") : "(Chưa nhận phòng)";
            _lblSoNguoiVal.Text = dto.SoNguoi.ToString() + " người";

            _dgvPhong.AutoGenerateColumns = false;
            _dgvPhong.DataSource = dto.DanhSachPhong;

            // Xử lý nút Check-in theo trạng thái
            if (dto.TrangThai == "Đã xác nhận")
            {
                _btnCheckIn.Enabled = true;
                _lblThongBao.Text = "Booking hợp lệ ở trạng thái 'Đã xác nhận'. Nhấn 'Xác nhận Check-in' để hoàn tất nhận phòng.";
                _lblThongBao.ForeColor = Color.DarkGreen;
            }
            else if (dto.TrangThai == "Đã check-in")
            {
                _btnCheckIn.Enabled = false;
                _lblThongBao.Text = $"Booking này đã hoàn tất check-in vào lúc {dto.NgayNhanThucTe:dd/MM/yyyy HH:mm}.";
                _lblThongBao.ForeColor = Color.Navy;
            }
            else if (dto.TrangThai == "Đã check-out")
            {
                _btnCheckIn.Enabled = false;
                _lblThongBao.Text = "Booking này đã hoàn tất check-out.";
                _lblThongBao.ForeColor = Color.Gray;
            }
            else if (dto.TrangThai == "Đã hủy")
            {
                _btnCheckIn.Enabled = false;
                _lblThongBao.Text = "Booking này đã bị hủy, không thể thực hiện check-in.";
                _lblThongBao.ForeColor = Color.Crimson;
            }
            else
            {
                _btnCheckIn.Enabled = false;
                _lblThongBao.Text = $"Trạng thái booking '{dto.TrangThai}' không hỗ trợ check-in.";
                _lblThongBao.ForeColor = Color.Crimson;
            }
        }

        private void CapNhatMauTrangThai(string trangThai)
        {
            switch (trangThai)
            {
                case "Đã xác nhận":
                    _lblTrangThaiVal.ForeColor = Color.DarkGreen;
                    break;
                case "Đã check-in":
                    _lblTrangThaiVal.ForeColor = Color.RoyalBlue;
                    break;
                case "Đã check-out":
                    _lblTrangThaiVal.ForeColor = Color.DimGray;
                    break;
                case "Đã hủy":
                    _lblTrangThaiVal.ForeColor = Color.Crimson;
                    break;
                default:
                    _lblTrangThaiVal.ForeColor = Color.Black;
                    break;
            }
        }

        private async Task ThucHienCheckInAsync()
        {
            if (_currentBooking == null) return;

            string ma = _currentBooking.MaBooking;
            int soPhong = _currentBooking.DanhSachPhong.Count;

            var xacNhan = MessageBox.Show(
                $"Xác nhận Check-in cho mã booking [{ma}]?\n\n" +
                $"- Khách hàng: {_currentBooking.TenKhachHang}\n" +
                $"- Số phòng nhận: {soPhong} phòng\n" +
                $"- Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                "Hệ thống sẽ cập nhật trạng thái booking sang 'Đã check-in' và các phòng liên quan sang 'Đang ở'.",
                "Xác nhận Check-in",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            try
            {
                _btnCheckIn.Enabled = false;
                _lblThongBao.Text = "Đang xử lý check-in...";
                _lblThongBao.ForeColor = Color.DarkSlateGray;

                var (success, message) = await _datPhongService.CheckInAsync(ma);

                if (success)
                {
                    MessageBox.Show(message, "Check-in thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Tải lại thông tin để hiển thị trạng thái mới nhất
                    await TraCuuBookingAsync();
                }
                else
                {
                    MessageBox.Show(message, "Check-in thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _lblThongBao.Text = message;
                    _lblThongBao.ForeColor = Color.Crimson;
                    _btnCheckIn.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình check-in: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCheckIn.Enabled = true;
            }
        }

        private void XoaTrangForm()
        {
            _txtMaBooking.Clear();
            _currentBooking = null;
            XoaTrangThongTin();
            _lblThongBao.Text = "Vui lòng nhập mã booking và nhấn 'Tra cứu' để kiểm tra thông tin.";
            _lblThongBao.ForeColor = Color.DarkSlateGray;
            _txtMaBooking.Focus();
        }

        private void XoaTrangThongTin()
        {
            _lblMaBookingVal.Text = "-";
            _lblNgayDatVal.Text = "-";
            _lblTrangThaiVal.Text = "-";
            _lblTrangThaiVal.ForeColor = Color.Black;
            _lblKhachHangVal.Text = "-";
            _lblSdtVal.Text = "-";
            _lblCccdVal.Text = "-";
            _lblEmailVal.Text = "-";
            _lblNgayNhanDuKienVal.Text = "-";
            _lblNgayTraDuKienVal.Text = "-";
            _lblNgayNhanThucTeVal.Text = "-";
            _lblSoNguoiVal.Text = "-";
            _dgvPhong.DataSource = null;
            _btnCheckIn.Enabled = false;
        }
    }
}
