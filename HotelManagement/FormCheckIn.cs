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
    public partial class FormCheckIn : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private readonly IServiceScope _scope;
        private readonly IDatPhongService _datPhongService;
        private readonly string? _maBookingKhoiTao;

        // Trạng thái hiện tại
        private DatPhongChiTietDto? _currentBooking;

        public FormCheckIn(TaiKhoan taiKhoanDangNhap, string? maBookingKhoiTao = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _maBookingKhoiTao = maBookingKhoiTao;

            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();

            InitializeComponent();

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
