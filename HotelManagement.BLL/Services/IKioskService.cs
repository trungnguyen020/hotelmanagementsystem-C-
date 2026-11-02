using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service phục vụ tra cứu thông tin đặt phòng tại Kiosk công cộng (DCKLTTQ-85).
    /// </summary>
    public interface IKioskService
    {
        /// <summary>
        /// Tra cứu thông tin booking dựa trên CẢ HAI yếu tố: mã booking và số điện thoại đặt phòng.
        /// BẮT BUỘC cả 2 thông tin phải khớp chính xác với cùng một khách hàng để bảo mật dữ liệu.
        /// Trả về null nếu sai một trong hai hoặc cả hai.
        /// </summary>
        Task<KioskBookingDto?> TraCuuBookingAsync(string maBooking, string sdt);
    }
}
