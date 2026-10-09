using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    public interface IPhongService
    {
        /// <summary>Danh sách trạng thái phòng hợp lệ, dùng chung cho ComboBox và validate.</summary>
        static readonly string[] DanhSachTrangThai = { "Trống", "Đang ở", "Đang dọn", "Bảo trì" };

        Task<List<PhongDto>> LayDanhSachAsync();
        Task<PhongDto?> LayTheoIdAsync(int id);
        Task<List<PhongDto>> TimKiemAsync(string tuKhoa);
        Task ThemAsync(PhongDto dto);
        Task CapNhatAsync(PhongDto dto);
        Task XoaAsync(int id);

        /// <summary>
        /// Chỉ cập nhật trạng thái phòng (dùng ở Epic 3 cho check-in/check-out và sơ đồ phòng),
        /// không cần đi qua validate đầy đủ của CapNhatAsync.
        /// </summary>
        Task CapNhatTrangThaiAsync(int id, string trangThaiMoi);
    }
}