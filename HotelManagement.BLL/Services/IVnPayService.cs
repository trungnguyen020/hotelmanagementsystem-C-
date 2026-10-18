using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Interface cổng thanh toán điện tử VNPay Sandbox (DCKLTTQ-76, DCKLTTQ-77).
    /// Tách rời trừu tượng để phục vụ gọi stub ở DCKLTTQ-76 và thay thế bằng triển khai sandbox thật ở DCKLTTQ-77.
    /// </summary>
    public interface IVnPayService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPay Sandbox với mã booking và số tiền tương ứng.
        /// </summary>
        Task<string> TaoUrlThanhToanAsync(string maBooking, decimal soTien, string noiDungThanhToan);

        /// <summary>
        /// Xác thực dữ liệu callback/IPN trả về từ VNPay sau khi khách thực hiện thanh toán.
        /// </summary>
        Task<(bool Success, string Message, string? MaGiaoDich)> XuLyCallbackAsync(IDictionary<string, string> vnpayData);
    }
}
