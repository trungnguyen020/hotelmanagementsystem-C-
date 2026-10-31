using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service xử lý gửi email thông báo tự động bằng MailKit (DCKLTTQ-81).
    /// Hỗ trợ gửi email xác nhận đặt phòng kèm mã booking và gửi email hóa đơn PDF sau khi thanh toán.
    /// Mọi phương thức gửi email đều bắt ngoại lệ nội bộ và trả về EmailResult để không làm gián đoạn luồng chính.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email HTML tổng quát bất đồng bộ qua SMTP Gmail (cổng 587 StartTls).
        /// </summary>
        Task<EmailResult> GuiEmailAsync(
            string nguoiNhan,
            string tieuDe,
            string noiDungHtml,
            byte[]? tepDinhKem = null,
            string? tenTepDinhKem = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Luồng 1 (DCKLTTQ-81): Gửi email xác nhận đặt phòng kèm mã booking đến khách hàng
        /// ngay sau khi lễ tân tạo DatPhong mới thành công.
        /// </summary>
        Task<EmailResult> GuiEmailXacNhanDatPhongAsync(
            string maBooking,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Luồng 2 (DCKLTTQ-81): Gửi email kèm hóa đơn PDF đính kèm (sinh từ IHoaDonPdfService)
        /// đến khách hàng ngay sau khi hoàn tất thanh toán hóa đơn.
        /// </summary>
        Task<EmailResult> GuiEmailHoaDonAsync(
            string maBooking,
            CancellationToken cancellationToken = default);
    }
}

