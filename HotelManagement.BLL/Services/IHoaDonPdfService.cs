using System;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Pdf;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ xuất hóa đơn định dạng PDF (DCKLTTQ-77).
    /// Áp dụng mô hình chuẩn Task, IProgress, CancellationToken cho tác vụ nặng (DCKLTTQ-93).
    /// </summary>
    public interface IHoaDonPdfService
    {
        /// <summary>
        /// Tạo mảng byte PDF từ dữ liệu hóa đơn DTO đã có sẵn.
        /// </summary>
        byte[] SinhPdfTuHoaDon(HoaDonDto hoaDon, HoaDonPdfTheme? theme = null);

        /// <summary>
        /// Tra cứu hóa đơn theo mã booking (tái sử dụng IHoaDonService) và sinh tệp PDF dưới dạng mảng byte.
        /// </summary>
        Task<byte[]> SinhPdfTheoMaBookingAsync(string maBooking, HoaDonPdfTheme? theme = null);

        /// <summary>
        /// Xuất file PDF trực tiếp ra đường dẫn chỉ định trên ổ đĩa từ dữ liệu hóa đơn DTO.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task XuatFilePdfAsync(
            HoaDonDto hoaDon,
            string duongDanTep,
            HoaDonPdfTheme? theme = null,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Tra cứu hóa đơn theo mã booking (tái sử dụng IHoaDonService) và xuất ra file PDF trên ổ đĩa.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task XuatFilePdfTheoMaBookingAsync(
            string maBooking,
            string duongDanTep,
            HoaDonPdfTheme? theme = null,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default);
    }
}
