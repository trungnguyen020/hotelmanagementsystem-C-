using System;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service xử lý gửi email thông báo tự động bằng MailKit (DCKLTTQ-81).
    /// Áp dụng mô hình chuẩn Task, IProgress, CancellationToken cho tác vụ nặng (DCKLTTQ-93).
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email HTML tổng quát bất đồng bộ qua SMTP Gmail (cổng 587 StartTls).
        /// Hỗ trợ báo tiến trình qua IProgress và hủy qua CancellationToken.
        /// </summary>
        Task<EmailResult> GuiEmailAsync(
            string nguoiNhan,
            string tieuDe,
            string noiDungHtml,
            byte[]? tepDinhKem = null,
            string? tenTepDinhKem = null,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Luồng 1 (DCKLTTQ-81): Gửi email xác nhận đặt phòng kèm mã booking đến khách hàng
        /// ngay sau khi lễ tân tạo DatPhong mới thành công.
        /// </summary>
        Task<EmailResult> GuiEmailXacNhanDatPhongAsync(
            string maBooking,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Luồng 2 (DCKLTTQ-81): Gửi email kèm hóa đơn PDF đính kèm (sinh từ IHoaDonPdfService)
        /// đến khách hàng ngay sau khi hoàn tất thanh toán hóa đơn.
        /// </summary>
        Task<EmailResult> GuiEmailHoaDonAsync(
            string maBooking,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default);
    }
}

