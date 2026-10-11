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
    }
}