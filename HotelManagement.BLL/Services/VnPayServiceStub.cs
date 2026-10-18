using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai Stub mô phỏng cổng thanh toán VNPay phục vụ DCKLTTQ-76 trước khi tích hợp API Sandbox thật ở DCKLTTQ-77.
    /// Giúp Form thanh toán hoạt động trơn tru theo nguyên lý Dependency Inversion.
    /// </summary>
    public class VnPayServiceStub : IVnPayService
    {
        public Task<string> TaoUrlThanhToanAsync(string maBooking, decimal soTien, string noiDungThanhToan)
        {
            long amountInVndCents = (long)(soTien * 100);
            string mockUrl = $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_TxnRef={maBooking}&vnp_Amount={amountInVndCents}&vnp_OrderInfo={Uri.EscapeDataString(noiDungThanhToan)}&vnp_ResponseCode=00";
            return Task.FromResult(mockUrl);
        }

        public Task<(bool Success, string Message, string? MaGiaoDich)> XuLyCallbackAsync(IDictionary<string, string> vnpayData)
        {
            string maGiaoDichMock = "VNP_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            return Task.FromResult((true, "Mô phỏng xác thực giao dịch VNPay Sandbox thành công.", (string?)maGiaoDichMock));
        }
    }
}
