using System;
using System.Collections.Generic;
using System.Text;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface IDatPhongService
    {
        Task<List<Phongtrongdto>> TimPhongTrongAsync(
            DateOnly ngayNhan, DateOnly ngayTra, int? idLoaiPhong, int? soKhach);
            
        Task<(bool Success, string Message)> DatPhongAsync(
            int idKhachHang, int idNhanSu, DateOnly ngayNhan, DateOnly ngayTra, int soNguoi, List<int> danhSachIdPhong);

        /// <summary>
        /// Tra cứu chi tiết đơn đặt phòng bằng mã booking phục vụ hiển thị màn hình Check-in (DCKLTTQ-71).
        /// </summary>
        Task<DatPhongChiTietDto?> LayChiTietTheoMaBookingAsync(string maBooking);

        /// <summary>
        /// Thực hiện quy trình Check-in cho mã booking: kiểm tra hợp lệ, cập nhật trạng thái booking thành "Đã check-in",
        /// ghi nhận ngày nhận phòng thực tế, và cập nhật trạng thái các phòng liên quan sang "Đang ở" trong 1 transaction (DCKLTTQ-71).
        /// </summary>
        Task<(bool Success, string Message)> CheckInAsync(string maBooking);

        /// <summary>
        /// Lấy danh mục dịch vụ phục vụ chọn dịch vụ phát sinh (DCKLTTQ-72).
        /// </summary>
        Task<List<DichVuDto>> LayDanhSachDichVuAsync();

        /// <summary>
        /// Thêm dịch vụ phát sinh trong quá trình lưu trú vào bảng ChiTietDichVu.
        /// Đơn giá chốt tại thời điểm dùng theo giá hiện tại của dịch vụ (DCKLTTQ-72).
        /// </summary>
        Task<(bool Success, string Message)> ThemDichVuPhatSinhAsync(string maBooking, int idDichVu, int soLuong);

        /// <summary>
        /// Xóa một dịch vụ phát sinh đã nhập nhầm khi khách vẫn đang trong trạng thái lưu trú (DCKLTTQ-72).
        /// </summary>
        Task<(bool Success, string Message)> XoaDichVuPhatSinhAsync(int idChiTietDichVu);

        /// <summary>
        /// Thực hiện quy trình Check-out cho mã booking: chỉ cho check-out khi trạng thái là "Đã check-in",
        /// cập nhật trạng thái booking thành "Đã check-out", ghi nhận ngày trả thực tế, và cập nhật trạng thái
        /// các phòng liên quan sang "Đang dọn" trong cùng 1 transaction (DCKLTTQ-72).
        /// </summary>
        Task<(bool Success, string Message)> CheckOutAsync(string maBooking);

        /// <summary>
        /// Tra cứu mã booking hiện tại của một phòng đang ở trạng thái "Đang ở" phục vụ liên kết mở nhanh màn hình Check-out từ Sơ đồ phòng GDI+ (DCKLTTQ-73).
        /// </summary>
        Task<string?> LayMaBookingHienTaiCuaPhongAsync(int idPhong);

        /// <summary>
        /// Tìm kiếm danh sách booking theo nhiều tiêu chí tùy chọn: tên khách hàng, SĐT, CCCD, số phòng, trạng thái.
        /// Hỗ trợ nhân viên lễ tân tra cứu nhanh booking khi check-in/check-out/thanh toán mà không cần nhớ mã booking.
        /// </summary>
        Task<List<BookingTimKiemDto>> TimKiemBookingAsync(
            string? tenKhachHang = null,
            string? sdt = null,
            string? cccd = null,
            string? soPhong = null,
            string? trangThai = null);
    }
}