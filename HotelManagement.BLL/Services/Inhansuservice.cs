using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface INhanSuService
    {
        Task<List<NhanSuDto>> GetAllAsync();
        Task<NhanSuDto?> GetByIdAsync(int id);
        Task<List<NhanSuDto>> TimKiemAsync(string tuKhoa);
        Task<(bool Success, string Message)> AddAsync(NhanSuDto dto);
        Task<(bool Success, string Message)> UpdateAsync(NhanSuDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        
        
        Task<(bool Success, string Message)> DatLaiMatKhauTaiKhoanAsync(int idNhanSu, string matKhauMoi);
    }
}
