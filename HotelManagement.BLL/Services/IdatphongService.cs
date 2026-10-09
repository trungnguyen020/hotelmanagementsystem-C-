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
    }
}