using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ xuất hóa đơn PDF bằng QuestPDF (DCKLTTQ-77).
    /// Áp dụng mô hình chuẩn Task, IProgress, CancellationToken cho tác vụ nặng (DCKLTTQ-93).
    /// Tuân thủ quy định License Community của QuestPDF cho đề tài học thuật.
    /// Tái sử dụng IHoaDonService để nạp dữ liệu chuẩn, không tự tính đè computed column tổng tiền.
    /// </summary>
    public class HoaDonPdfService : IHoaDonPdfService
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IQrCodeService? _qrCodeService;

        static HoaDonPdfService()
        {
            // Thiết lập License Community cho QuestPDF (hợp lệ cho cá nhân, đồ án môn học, sinh viên)
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseSystemFonts = true;
            QuestPDF.Settings.ThrowOnMissingFontFamilies = false;
        }

        public HoaDonPdfService(IHoaDonService hoaDonService, IQrCodeService? qrCodeService = null)
        {
            _hoaDonService = hoaDonService ?? throw new ArgumentNullException(nameof(hoaDonService));
            _qrCodeService = qrCodeService;
        }

        public byte[] SinhPdfTuHoaDon(HoaDonDto hoaDon, HoaDonPdfTheme? theme = null)
        {
            if (hoaDon == null)
                throw new ArgumentNullException(nameof(hoaDon), "Dữ liệu hóa đơn không được để trống.");

            byte[]? qrBytes = null;
            if (_qrCodeService != null && !string.IsNullOrWhiteSpace(hoaDon.MaBooking))
            {
                try
                {
                    qrBytes = _qrCodeService.SinhMaQrByte(hoaDon.MaBooking.Trim(), 180);
                }
                catch
                {
                    // Dự phòng an toàn: nếu sinh QR gặp lỗi vẫn xuất hóa đơn PDF bình thường
                }
            }

            var document = new HoaDonPdfDocument(hoaDon, theme, qrBytes);
            return document.GeneratePdf();
        }

        public async Task<byte[]> SinhPdfTheoMaBookingAsync(string maBooking, HoaDonPdfTheme? theme = null)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                throw new ArgumentException("Mã booking không hợp lệ.", nameof(maBooking));

            var hoaDon = await _hoaDonService.LayHoaDonTheoMaBookingAsync(maBooking.Trim());
            if (hoaDon == null)
                throw new InvalidOperationException($"Không tìm thấy thông tin hóa đơn cho mã booking '{maBooking}'.");

            return SinhPdfTuHoaDon(hoaDon, theme);
        }

        public async Task XuatFilePdfAsync(
            HoaDonDto hoaDon,
            string duongDanTep,
            HoaDonPdfTheme? theme = null,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (hoaDon == null)
                throw new ArgumentNullException(nameof(hoaDon), "Dữ liệu hóa đơn không được để trống.");

            if (string.IsNullOrWhiteSpace(duongDanTep))
                throw new ArgumentException("Đường dẫn lưu tệp không được để trống.", nameof(duongDanTep));

            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Đang chuẩn bị thư mục lưu tệp PDF...");

            string? thuMuc = Path.GetDirectoryName(duongDanTep);
            if (!string.IsNullOrEmpty(thuMuc) && !Directory.Exists(thuMuc))
            {
                Directory.CreateDirectory(thuMuc);
            }

            // Sinh PDF bằng Task.Run trên luồng nền (CPU-bound)
            byte[] bytes = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report("Đang khởi tạo tài liệu hóa đơn và mã QR...");

                byte[]? qrBytes = null;
                if (_qrCodeService != null && !string.IsNullOrWhiteSpace(hoaDon.MaBooking))
                {
                    try
                    {
                        qrBytes = _qrCodeService.SinhMaQrByte(hoaDon.MaBooking.Trim(), 180);
                    }
                    catch
                    {
                        // Dự phòng an toàn: nếu sinh QR gặp lỗi vẫn xuất hóa đơn PDF bình thường
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report("Đang biên dịch và kết xuất hóa đơn ra PDF theo chuẩn QuestPDF...");

                var document = new HoaDonPdfDocument(hoaDon, theme, qrBytes);
                var pdfBytes = document.GeneratePdf();

                cancellationToken.ThrowIfCancellationRequested();
                return pdfBytes;
            }, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Đang ghi tệp PDF xuống đĩa...");

            await File.WriteAllBytesAsync(duongDanTep, bytes, cancellationToken);

            progress?.Report("Xuất hóa đơn PDF thành công!");
        }

        public async Task XuatFilePdfTheoMaBookingAsync(
            string maBooking,
            string duongDanTep,
            HoaDonPdfTheme? theme = null,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                throw new ArgumentException("Mã booking không hợp lệ.", nameof(maBooking));

            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report($"Đang tra cứu dữ liệu hóa đơn cho mã booking '{maBooking}'...");

            var hoaDon = await _hoaDonService.LayHoaDonTheoMaBookingAsync(maBooking.Trim());
            if (hoaDon == null)
                throw new InvalidOperationException($"Không tìm thấy thông tin hóa đơn cho mã booking '{maBooking}'.");

            await XuatFilePdfAsync(hoaDon, duongDanTep, theme, progress, cancellationToken);
        }
    }
}
