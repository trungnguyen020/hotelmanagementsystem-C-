using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface IDichVuService
    {
        Task<List<DichVuDto>> LayDanhSachAsync();
        Task<DichVuDto?> LayTheoIdAsync(int id);
        Task<List<DichVuDto>> TimKiemAsync(string tuKhoa);
        Task ThemAsync(DichVuDto dto);
        Task CapNhatAsync(DichVuDto dto);
        Task XoaAsync(int id);
    }
}
