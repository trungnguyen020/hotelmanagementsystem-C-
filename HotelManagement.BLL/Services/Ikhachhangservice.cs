using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface IKhachHangService
    {
        Task<List<KhachHangDto>> GetAllAsync();
        Task<KhachHangDto?> GetByIdAsync(int id);
        Task<List<KhachHangDto>> TimKiemAsync(string tuKhoa);
        Task<(bool Success, string Message)> AddAsync(KhachHangDto dto);
        Task<(bool Success, string Message)> UpdateAsync(KhachHangDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}