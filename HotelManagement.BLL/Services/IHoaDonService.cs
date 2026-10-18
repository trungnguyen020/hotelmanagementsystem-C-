using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ hóa đơn khách sạn (DCKLTTQ-75).
    /// Chịu trách nhiệm tổng hợp tiền phòng, tiền dịch vụ phát sinh, lưu trữ hóa đơn và nhận lại giá trị tổng tiền tự tính từ Database.
    /// </summary>
    public interface IHoaDonService
    {
        /// <summary>
        /// Tra cứu thông tin hóa đơn theo mã booking.
        /// Nếu booking đã có hóa đơn trong DB, trả về thông tin hóa đơn thực tế.
        /// Nếu chưa có, tính toán tạm tính (preview) từ dữ liệu phòng và dịch vụ hiện tại.
        /// </summary>
        Task<HoaDonDto?> LayHoaDonTheoMaBookingAsync(string maBooking);

        /// <summary>
        /// Tính toán tổng tiền phòng từ ChiTietDatPhong, tiền dịch vụ từ ChiTietDichVu,
        /// áp dụng chiết khấu/giảm giá và lưu/cập nhật bản ghi HoaDon vào DB (Idempotent Upsert).
        /// Cột Tonggia được SQL Server tự động tính thông qua Computed Column (PERSISTED),
        /// Service nạp lại giá trị tổng tiền từ DB sau khi lưu.
        /// </summary>
        Task<(bool Success, string Message, HoaDonDto? Data)> TinhVaLuuHoaDonAsync(string maBooking, decimal giamGia = 0);

        /// <summary>
        /// Cập nhật hình thức thanh toán và trạng thái thanh toán (ví dụ: "Tiền mặt", "VNPay", trạng thái "Đã thanh toán").
        /// </summary>
        Task<(bool Success, string Message)> CapNhatThanhToanAsync(string maBooking, string phuongThucThanhToan, string trangThaiThanhToan = "Đã thanh toán");
    }
}
