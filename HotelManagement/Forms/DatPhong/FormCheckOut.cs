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
    /// Màn hình Check-out và thêm dịch vụ phát sinh (DCKLTTQ-72).
    /// Hỗ trợ tra cứu booking theo mã, thêm/xóa dịch vụ phát sinh trong lúc lưu trú,
    /// và thực hiện trả phòng (chuyển trạng thái booking sang "Đã check-out", phòng sang "Đang dọn").
    /// </summary>
    public partial class FormCheckOut : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private readonly IServiceScope _scope;
        private readonly IDatPhongService _datPhongService;
        private readonly IHoaDonService _hoaDonService;
        private readonly string? _maBookingKhoiTao;

        // Trạng thái hiện tại
        private DatPhongChiTietDto? _currentBooking;
        private List<DichVuDto> _danhSachDichVu = new();

        public FormCheckOut(TaiKhoan taiKhoanDangNhap, string? maBookingKhoiTao = null)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _maBookingKhoiTao = maBookingKhoiTao;

            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();
            _hoaDonService = _scope.ServiceProvider.GetRequiredService<IHoaDonService>();

            InitializeComponent();

            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) =>
            {
                await TaiDanhSachDichVuAsync();
                if (!string.IsNullOrWhiteSpace(_maBookingKhoiTao))
                {
                    _txtMaBooking.Text = _maBookingKhoiTao.Trim();
                    await TraCuuBookingAsync();
                }
            };
        }

        private async Task TaiDanhSachDichVuAsync()
        {
            try
            {
                _danhSachDichVu = await _datPhongService.LayDanhSachDichVuAsync();
                _cboDichVu.DisplayMember = "TenDichVu";
                _cboDichVu.ValueMember = "IdDichVu";
                _cboDichVu.DataSource = _danhSachDichVu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục dịch vụ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPhong_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

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
                await HienThiThongTinAsync(booking);
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

        private async Task HienThiThongTinAsync(DatPhongChiTietDto dto)
        {
            _lblMaBookingVal.Text = dto.MaBooking;
            _lblTrangThaiVal.Text = dto.TrangThai;
            CapNhatMauTrangThai(dto.TrangThai);

            _lblKhachHangVal.Text = dto.TenKhachHang;
            _lblSdtVal.Text = dto.SdtKhachHang;
            _lblCccdVal.Text = dto.CccdKhachHang;

            _lblNgayNhanThucTeVal.Text = dto.NgayNhanThucTe.HasValue ? dto.NgayNhanThucTe.Value.ToString("dd/MM/yyyy HH:mm") : "(Chưa nhận phòng)";
            _lblNgayTraDuKienVal.Text = dto.NgayTraDuKien.ToString("dd/MM/yyyy");
            _lblNgayTraThucTeVal.Text = dto.NgayTraThucTe.HasValue ? dto.NgayTraThucTe.Value.ToString("dd/MM/yyyy HH:mm") : "(Chưa trả phòng)";

            // Hiển thị tiền tạm tính
            _lblTongTienPhongVal.Text = $"{dto.TongTienPhong:N0} đ";
            _lblTongTienDichVuVal.Text = $"{dto.TongTienDichVu:N0} đ";
            _lblTongCongVal.Text = $"{dto.TongTienTamTinh:N0} đ";

            // Grid phòng & Dịch vụ
            _dgvPhong.AutoGenerateColumns = false;
            _dgvPhong.DataSource = dto.DanhSachPhong;

            _dgvDichVu.AutoGenerateColumns = false;
            _dgvDichVu.DataSource = dto.DanhSachDichVu;

            // Kiểm tra trạng thái hóa đơn thanh toán
            try
            {
                var hoaDon = await _hoaDonService.LayHoaDonTheoMaBookingAsync(dto.MaBooking);
                if (hoaDon != null && hoaDon.TrangThaiThanhtoan == "Đã thanh toán")
                {
                    _lblTrangThaiThanhToanVal.Text = $"Đã thanh toán ({hoaDon.PhuongThucThanhtoan ?? "Tiền mặt"})";
                    _lblTrangThaiThanhToanVal.ForeColor = Color.DarkGreen;
                }
                else
                {
                    _lblTrangThaiThanhToanVal.Text = "Chưa thanh toán";
                    _lblTrangThaiThanhToanVal.ForeColor = Color.Crimson;
                }
            }
            catch
            {
                _lblTrangThaiThanhToanVal.Text = "Chưa thanh toán";
                _lblTrangThaiThanhToanVal.ForeColor = Color.Crimson;
            }

            // Bật nút Thanh toán cho booking đang ở hoặc đã check-out
            _btnThanhToan.Enabled = (dto.TrangThai == "Đã check-in" || dto.TrangThai == "Đã check-out");

            // Xử lý quyền thao tác Check-out & Thêm DV theo trạng thái
            if (dto.TrangThai == "Đã check-in")
            {
                _btnCheckOut.Enabled = true;
                _grpThemDichVu.Enabled = true;
                _lblThongBao.Text = "Khách đang lưu trú ('Đã check-in'). Có thể thêm dịch vụ phát sinh, thanh toán hoặc thực hiện Check-out.";
                _lblThongBao.ForeColor = Color.DarkGreen;
            }
            else if (dto.TrangThai == "Đã check-out")
            {
                _btnCheckOut.Enabled = false;
                _grpThemDichVu.Enabled = false;
                _lblThongBao.Text = $"Booking này đã hoàn tất check-out vào lúc {dto.NgayTraThucTe:dd/MM/yyyy HH:mm}.";
                _lblThongBao.ForeColor = Color.DimGray;
            }
            else if (dto.TrangThai == "Đã xác nhận")
            {
                _btnCheckOut.Enabled = false;
                _grpThemDichVu.Enabled = false;
                _lblThongBao.Text = "Khách chưa thực hiện Check-in, không thể Check-out.";
                _lblThongBao.ForeColor = Color.DarkOrange;
            }
            else if (dto.TrangThai == "Đã hủy")
            {
                _btnCheckOut.Enabled = false;
                _grpThemDichVu.Enabled = false;
                _lblThongBao.Text = "Booking này đã bị hủy.";
                _lblThongBao.ForeColor = Color.Crimson;
            }
            else
            {
                _btnCheckOut.Enabled = false;
                _grpThemDichVu.Enabled = false;
                _lblThongBao.Text = $"Trạng thái booking '{dto.TrangThai}' không hỗ trợ check-out.";
                _lblThongBao.ForeColor = Color.Crimson;
            }
        }

        private void CapNhatMauTrangThai(string trangThai)
        {
            switch (trangThai)
            {
                case "Đã check-in":
                    _lblTrangThaiVal.ForeColor = Color.RoyalBlue;
                    break;
                case "Đã check-out":
                    _lblTrangThaiVal.ForeColor = Color.DimGray;
                    break;
                case "Đã xác nhận":
                    _lblTrangThaiVal.ForeColor = Color.DarkGreen;
                    break;
                case "Đã hủy":
                    _lblTrangThaiVal.ForeColor = Color.Crimson;
                    break;
                default:
                    _lblTrangThaiVal.ForeColor = Color.Black;
                    break;
            }
        }

        private async Task ThemDichVuAsync()
        {
            if (_currentBooking == null || _currentBooking.TrangThai != "Đã check-in")
            {
                MessageBox.Show("Chỉ có thể thêm dịch vụ khi khách đang lưu trú ('Đã check-in').", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_cboDichVu.SelectedItem is not DichVuDto dv)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần thêm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soLuong = (int)_numSoLuong.Value;
            decimal donGia = dv.MucGia;

            var (success, message) = await _datPhongService.ThemDichVuPhatSinhAsync(_currentBooking.MaBooking, dv.IdDichVu, soLuong);
            if (success)
            {
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _numSoLuong.Value = 1;
                // Tải lại chi tiết để cập nhật danh sách dịch vụ và tổng tiền
                await TraCuuBookingAsync();
            }
            else
            {
                MessageBox.Show(message, "Lỗi thêm dịch vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task XoaDichVuAsync()
        {
            if (_dgvDichVu.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa trong danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dgvDichVu.SelectedRows[0].DataBoundItem is not ChiTietDichVuDto ctDv)
            {
                return;
            }

            var xacNhan = MessageBox.Show(
                $"Xác nhận xóa dịch vụ [{ctDv.TenDichVu}] (SL: {ctDv.SoLuong})?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            var (success, message) = await _datPhongService.XoaDichVuPhatSinhAsync(ctDv.Id);
            if (success)
            {
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await TraCuuBookingAsync();
            }
            else
            {
                MessageBox.Show(message, "Lỗi xóa dịch vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task ThucHienCheckOutAsync()
        {
            if (_currentBooking == null) return;

            string ma = _currentBooking.MaBooking;
            decimal tongTien = _currentBooking.TongTienTamTinh;

            var xacNhan = MessageBox.Show(
                $"Xác nhận hoàn tất Check-out cho mã booking [{ma}]?\n\n" +
                $"- Khách hàng: {_currentBooking.TenKhachHang}\n" +
                $"- Tiền phòng: {_currentBooking.TongTienPhong:N0} đ\n" +
                $"- Tiền dịch vụ phát sinh: {_currentBooking.TongTienDichVu:N0} đ\n" +
                $"----------------------------------------\n" +
                $"TỔNG CHI PHÍ TẠM TÍNH: {tongTien:N0} đ\n\n" +
                "Sau khi Check-out, tất cả phòng liên quan sẽ được chuyển sang trạng thái 'Đang dọn'.",
                "Xác nhận Check-out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            try
            {
                _btnCheckOut.Enabled = false;
                _lblThongBao.Text = "Đang xử lý check-out...";
                _lblThongBao.ForeColor = Color.DarkSlateGray;

                var (success, message) = await _datPhongService.CheckOutAsync(ma);

                if (success)
                {
                    MessageBox.Show(message, "Check-out thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Tải lại để hiển thị trạng thái mới nhất
                    await TraCuuBookingAsync();
                }
                else
                {
                    MessageBox.Show(message, "Check-out thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _lblThongBao.Text = message;
                    _lblThongBao.ForeColor = Color.Crimson;
                    _btnCheckOut.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình check-out: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCheckOut.Enabled = true;
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
            _lblTrangThaiVal.Text = "-";
            _lblTrangThaiVal.ForeColor = Color.Black;
            _lblKhachHangVal.Text = "-";
            _lblSdtVal.Text = "-";
            _lblCccdVal.Text = "-";
            _lblNgayNhanThucTeVal.Text = "-";
            _lblNgayTraDuKienVal.Text = "-";
            _lblNgayTraThucTeVal.Text = "-";
            _lblTongTienPhongVal.Text = "0 đ";
            _lblTongTienDichVuVal.Text = "0 đ";
            _lblTongCongVal.Text = "0 đ";

            _dgvPhong.DataSource = null;
            _dgvDichVu.DataSource = null;
            _lblTrangThaiThanhToanVal.Text = "-";
            _lblTrangThaiThanhToanVal.ForeColor = Color.DarkOrange;
            _grpThemDichVu.Enabled = false;
            _btnCheckOut.Enabled = false;
            _btnThanhToan.Enabled = false;
        }

        private async Task MoManHinhThanhToanAsync()
        {
            if (_currentBooking == null) return;

            using var frm = new FormThanhToan(_currentBooking.MaBooking);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                await TraCuuBookingAsync();
            }
        }
    }
}
