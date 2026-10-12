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
    }
}