using System.Drawing;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Giao diện dịch vụ sinh và đọc mã QR phục vụ tra cứu booking nhanh (DCKLTTQ-80).
    /// Hỗ trợ sinh mã QR động theo mã booking (dùng cho Kiosk, phiếu xác nhận, hóa đơn PDF).
    /// </summary>
    [SupportedOSPlatform("windows")]
    public interface IQrCodeService
    {
        /// <summary>
        /// Sinh mã QR dạng mảng byte PNG từ nội dung bất kỳ (chuẩn UTF-8).
        /// </summary>
        byte[] SinhMaQrByte(string noiDung, int kichThuoc = 300);

        /// <summary>
        /// Sinh mã QR dạng Bitmap (GDI+) phục vụ hiển thị trực tiếp trên PictureBox của WinForms.
        /// </summary>
        Bitmap SinhMaQrBitmap(string noiDung, int kichThuoc = 300);

        /// <summary>
        /// Kiểm tra booking tồn tại trong CSDL qua IDatPhongRepository rồi sinh mã QR chứa chính xác mã booking.
        /// Trả về null nếu mã booking không tồn tại trong hệ thống.
        /// </summary>
        Task<byte[]?> SinhMaQrChoBookingAsync(string maBooking, int kichThuoc = 300);

        /// <summary>
        /// Kiểm tra booking tồn tại trong CSDL rồi sinh mã QR dạng Bitmap.
        /// Trả về null nếu mã booking không tồn tại.
        /// </summary>
        Task<Bitmap?> SinhMaQrChoBookingBitmapAsync(string maBooking, int kichThuoc = 300);

        /// <summary>
        /// Đọc và giải mã chuỗi ký tự từ ảnh Bitmap QR code (dùng cho tính năng tra cứu tại Kiosk).
        /// </summary>
        string? DocMaQrTuBitmap(Bitmap bitmap);

        /// <summary>
        /// Đọc và giải mã chuỗi ký tự từ mảng byte ảnh.
        /// </summary>
        string? DocMaQrTuBytes(byte[] imageBytes);
    }
}
