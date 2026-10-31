using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
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
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        private Bitmap? _qrBitmap;
        private int? _idDatPhong;

        public FormXacNhanDatPhong(string maBooking)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                throw new ArgumentException("Mã booking không được để trống.", nameof(maBooking));

            _maBooking = maBooking.Trim();
            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();
            _qrCodeService = _scope.ServiceProvider.GetRequiredService<IQrCodeService>();
            _emailService = _scope.ServiceProvider.GetRequiredService<IEmailService>();
            _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            InitializeComponent();

            FormClosed += (_, _) =>
            {
                _qrBitmap?.Dispose();
                _scope.Dispose();
            };

            Load += async (_, _) => await TaiDuLieuVaSinhQrAsync();

            _btnSaoChepMa.Click += (_, _) => SaoChepMaBooking();
            _btnLuuQr.Click += (_, _) => LuuAnhQr();
            _btnGuiEmail.Click += async (_, _) => await GuiLaiEmailXacNhanAsync();
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
                    _idDatPhong = booking.IdDatPhong;
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

                // Tự động kích hoạt gửi email xác nhận kèm mã booking (DCKLTTQ-81)
                _ = GuiEmailTuDongAsync();
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

        private async Task GuiEmailTuDongAsync()
        {
            try
            {
                _lblEmailStatusVal.Text = "Đang gửi email xác nhận...";
                _lblEmailStatusVal.ForeColor = AppTheme.TextSecondary;

                var result = await _emailService.GuiEmailXacNhanDatPhongAsync(_maBooking);
                CapNhatTrangThaiEmail(result);

                // DCKLTTQ-82: Ghi nhận lịch sử gửi thông báo
                await GhiLichSuThongBaoAsync(result, "XacNhanBooking", _idDatPhong, null);
            }
            catch
            {
                // Bảo vệ không ném lỗi
            }
        }

        private void CapNhatTrangThaiEmail(EmailResult result)
        {
            if (result.ThanhCong)
            {
                _lblEmailStatusVal.Text = $"✓ Đã gửi đến {result.NguoiNhan}";
                _lblEmailStatusVal.ForeColor = AppTheme.StatusSuccessText;
            }
            else
            {
                _lblEmailStatusVal.Text = $"⚠ {result.LyDoLoi}";
                _lblEmailStatusVal.ForeColor = AppTheme.StatusWarningText;
            }
        }

        private async Task GuiLaiEmailXacNhanAsync()
        {
            try
            {
                _btnGuiEmail.Enabled = false;
                _btnGuiEmail.Text = "Đang gửi email...";
                _lblEmailStatusVal.Text = "Đang gửi email xác nhận...";
                _lblEmailStatusVal.ForeColor = AppTheme.TextSecondary;

                var result = await _emailService.GuiEmailXacNhanDatPhongAsync(_maBooking);
                CapNhatTrangThaiEmail(result);

                // DCKLTTQ-82: Ghi nhận lịch sử gửi thông báo
                await GhiLichSuThongBaoAsync(result, "XacNhanBooking", _idDatPhong, null);

                if (result.ThanhCong)
                {
                    MessageBox.Show(
                        $"Đã gửi email xác nhận đặt phòng thành công đến: {result.NguoiNhan}",
                        "Gửi email thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Không thể gửi email xác nhận:\n{result.LyDoLoi}\n\n(Lưu ý: Bạn cần cấu hình Email:SenderEmail và Email:AppPassword trong User Secrets hoặc appsettings.json).",
                        "Thông báo gửi email",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnGuiEmail.Enabled = true;
                _btnGuiEmail.Text = "✉ Gửi lại email xác nhận";
            }
        }

        private async Task GhiLichSuThongBaoAsync(EmailResult result, string loaiThongBao, int? idDatPhong, int? idHoaDon)
        {
            try
            {
                var lichSu = new LichSuGuiThongBao
                {
                    IdDatphong = idDatPhong,
                    IdHoadon = idHoaDon,
                    LoaiThongbao = loaiThongBao,
                    NguoiNhan = result.NguoiNhan ?? string.Empty,
                    TieuDe = result.TieuDe,
                    TrangThai = result.ThanhCong ? "Thành công" : "Thất bại",
                    LyDoLoi = result.LyDoLoi,
                    ThoiGianGui = result.ThoiGianGui ?? DateTime.Now
                };

                await _unitOfWork.LichSuGuiThongBaoRepository.AddAsync(lichSu);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // Yêu cầu bắt buộc: Việc ghi log này không được phép làm gián đoạn luồng nghiệp vụ chính nếu bản thân nó lỗi
            }
        }
    }
}
