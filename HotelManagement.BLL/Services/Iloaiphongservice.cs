using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface ILoaiPhongService
    {
        Task<List<LoaiPhongDto>> LayDanhSachAsync();
        Task<LoaiPhongDto?> LayTheoIdAsync(int id);
        Task<List<LoaiPhongDto>> TimKiemAsync(string tuKhoa);
        Task ThemAsync(LoaiPhongDto dto);
        Task CapNhatAsync(LoaiPhongDto dto);
        Task XoaAsync(int id);
    }
}