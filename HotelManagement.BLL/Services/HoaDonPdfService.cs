using System;
using System.IO;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ xuất hóa đơn PDF bằng QuestPDF (DCKLTTQ-77).
    /// Tuân thủ quy định License Community của QuestPDF cho đề tài học thuật.
    /// Tái sử dụng IHoaDonService để nạp dữ liệu chuẩn, không tự tính đè computed column tổng tiền.
    /// </summary>
    public class HoaDonPdfService : IHoaDonPdfService
    {
        private readonly IHoaDonService _hoaDonService;

        static HoaDonPdfService()
        {
            // Thiết lập License Community cho QuestPDF (hợp lệ cho cá nhân, đồ án môn học, sinh viên)
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseSystemFonts = true;
            QuestPDF.Settings.ThrowOnMissingFontFamilies = false;
        }

        public HoaDonPdfService(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService ?? throw new ArgumentNullException(nameof(hoaDonService));
        }

        public byte[] SinhPdfTuHoaDon(HoaDonDto hoaDon, HoaDonPdfTheme? theme = null)
        {
            if (hoaDon == null)
                throw new ArgumentNullException(nameof(hoaDon), "Dữ liệu hóa đơn không được để trống.");

            var document = new HoaDonPdfDocument(hoaDon, theme);
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

        public async Task XuatFilePdfAsync(HoaDonDto hoaDon, string duongDanTep, HoaDonPdfTheme? theme = null)
        {
            if (hoaDon == null)
                throw new ArgumentNullException(nameof(hoaDon), "Dữ liệu hóa đơn không được để trống.");

            if (string.IsNullOrWhiteSpace(duongDanTep))
                throw new ArgumentException("Đường dẫn lưu tệp không được để trống.", nameof(duongDanTep));

            string? thuMuc = Path.GetDirectoryName(duongDanTep);
            if (!string.IsNullOrEmpty(thuMuc) && !Directory.Exists(thuMuc))
            {
                Directory.CreateDirectory(thuMuc);
            }

            var document = new HoaDonPdfDocument(hoaDon, theme);
            byte[] bytes = document.GeneratePdf();

            await File.WriteAllBytesAsync(duongDanTep, bytes);
        }

        public async Task XuatFilePdfTheoMaBookingAsync(string maBooking, string duongDanTep, HoaDonPdfTheme? theme = null)
        {
            if (string.IsNullOrWhiteSpace(maBooking))
                throw new ArgumentException("Mã booking không hợp lệ.", nameof(maBooking));

            var hoaDon = await _hoaDonService.LayHoaDonTheoMaBookingAsync(maBooking.Trim());
            if (hoaDon == null)
                throw new InvalidOperationException($"Không tìm thấy thông tin hóa đơn cho mã booking '{maBooking}'.");

            await XuatFilePdfAsync(hoaDon, duongDanTep, theme);
        }
    }
}
