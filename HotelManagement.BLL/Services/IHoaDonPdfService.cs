using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Pdf;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ xuất hóa đơn định dạng PDF (DCKLTTQ-77).
    /// Hỗ trợ chuyển đổi từ HoaDonDto (đã nạp computed column) sang tệp PDF tiêu chuẩn hoặc mảng byte phục vụ in/gửi email.
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
        /// </summary>
        Task XuatFilePdfAsync(HoaDonDto hoaDon, string duongDanTep, HoaDonPdfTheme? theme = null);

        /// <summary>
        /// Tra cứu hóa đơn theo mã booking (tái sử dụng IHoaDonService) và xuất ra file PDF trên ổ đĩa.
        /// </summary>
        Task XuatFilePdfTheoMaBookingAsync(string maBooking, string duongDanTep, HoaDonPdfTheme? theme = null);
    }
}
