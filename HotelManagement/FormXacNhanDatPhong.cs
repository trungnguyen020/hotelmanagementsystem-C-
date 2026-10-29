using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    /// <summary>
    /// Màn hình Xác nhận Đặt phòng thành công kèm mã QR (DCKLTTQ-80).
    /// Hiển thị mã QR chứa mã booking sinh bằng ZXing.Net phục vụ khách quét tra cứu nhanh tại Kiosk.
    /// Tái sử dụng dữ liệu từ IDatPhongRepository.GetByMaBookingAsync thông qua IDatPhongService.
    /// </summary>
    public partial class FormXacNhanDatPhong : Form
    {
        private readonly string _maBooking;
        private readonly IServiceScope _scope;
        private readonly IDatPhongService _datPhongService;
        private readonly IQrCodeService _qrCodeService;

        private Bitmap? _qrBitmap;

        public FormXacNhanDatPhong(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                throw new ArgumentException("Mã booking không được để trống.", nameof(maBooking));

            _maBooking = maBooking.Trim();
            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();
            _qrCodeService = _scope.ServiceProvider.GetRequiredService<IQrCodeService>();

            InitializeComponent();

            FormClosed += (_, _) =>
            {
                _qrBitmap?.Dispose();
                _scope.Dispose();
            };

            Load += async (_, _) => await TaiDuLieuVaSinhQrAsync();

            _btnSaoChepMa.Click += (_, _) => SaoChepMaBooking();
            _btnLuuQr.Click += (_, _) => LuuAnhQr();
            _btnDong.Click += (_, _) => Close();
        }

        private async Task TaiDuLieuVaSinhQrAsync()
        {
            try
            {
                _lblMaBookingVal.Text = _maBooking;

                // 1. Tải thông tin chi tiết booking từ CSDL (tái sử dụng GetByMaBookingAsync)
                var booking = await _datPhongService.LayChiTietTheoMaBookingAsync(_maBooking);
                if (booking != null)
                {
                    _lblKhachHangVal.Text = booking.TenKhachHang;
                    _lblSdtVal.Text = $"{booking.SdtKhachHang}  |  CCCD: {booking.CccdKhachHang}";
                    _lblThoiGianVal.Text = $"{booking.NgayNhanDuKien:dd/MM/yyyy}  ➔  {booking.NgayTraDuKien:dd/MM/yyyy}";
                    _lblSoKhachVal.Text = $"{booking.SoNguoi} khách";

                    if (booking.DanhSachPhong != null && booking.DanhSachPhong.Any())
                    {
                        var tenPhongs = booking.DanhSachPhong.Select(p => $"Phòng {p.SoPhong} ({p.TenLoaiPhong})");
                        _lblDanhSachPhongVal.Text = string.Join(", ", tenPhongs);
                    }
                    else
                    {
                        _lblDanhSachPhongVal.Text = "(Chưa chỉ định phòng)";
                    }

                    _lblTrangThaiVal.Text = booking.TrangThai.ToUpperInvariant();
                }

                // 2. Sinh mã QR động cho mã booking bằng ZXing.Net
                _qrBitmap = await _qrCodeService.SinhMaQrChoBookingBitmapAsync(_maBooking, 260);
                if (_qrBitmap != null)
                {
                    _picQrCode.Image = _qrBitmap;
                }
                else
                {
                    // Dự phòng nếu không tìm thấy booking qua repository
                    _qrBitmap = _qrCodeService.SinhMaQrBitmap(_maBooking, 260);
                    _picQrCode.Image = _qrBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi tạo mã QR hoặc tải thông tin đặt phòng: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaoChepMaBooking()
        {
            try
            {
                Clipboard.SetText(_maBooking);
                MessageBox.Show($"Đã sao chép mã booking '{_maBooking}' vào bộ nhớ tạm!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể sao chép: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LuuAnhQr()
        {
            if (_qrBitmap == null)
            {
                MessageBox.Show("Mã QR chưa sẵn sàng để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg",
                FileName = $"QR_{_maBooking}.png",
                Title = "Lưu mã QR Booking"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var format = sfd.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        ? ImageFormat.Jpeg
                        : ImageFormat.Png;

                    _qrBitmap.Save(sfd.FileName, format);
                    MessageBox.Show($"Đã lưu mã QR thành công tại:\n{sfd.FileName}",
                        "Lưu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu tệp ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
