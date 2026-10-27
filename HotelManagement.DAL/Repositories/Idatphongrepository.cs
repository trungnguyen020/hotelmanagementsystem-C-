using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    /// <summary>
    /// Repository nghiệp vụ quan trọng nhất hệ thống: chịu trách nhiệm kiểm tra tình trạng
    /// trống/bận của phòng theo khoảng ngày để tránh double-booking.
    /// Cột ngaynhan_dukien / ngaytra_dukien trong bảng DatPhong là kiểu DATE, nên tham số
    /// dùng DateOnly (EF Core 8+ map DATE -> DateOnly mặc định khi scaffold).
    /// </summary>
    public interface IDatPhongRepository : IRepository<DatPhong>
    {
        /// <summary>
        /// Kiểm tra 1 phòng cụ thể (id_phong) có trống trong khoảng [ngayNhan, ngayTra) hay không.
        /// excludeIdDatPhong dùng khi sửa 1 booking đã tồn tại (không tự đụng độ với chính nó).
        /// </summary>
        Task<bool> IsPhongTrongAsync(int idPhong, DateOnly ngayNhan, DateOnly ngayTra, int? excludeIdDatPhong = null);

        // IDatPhongRepository.cs — chỉ sửa chữ ký method này
        /// <summary>Lấy danh sách phòng còn trống trong khoảng ngày, lọc theo loại phòng và/hoặc sức chứa tối thiểu.</summary>
        Task<IEnumerable<Phong>> GetPhongTrongAsync(
            DateOnly ngayNhan, DateOnly ngayTra, int? idLoaiPhong = null, int? soKhach = null);

        /// <summary>Tra cứu booking theo mã booking (mabooking) - dùng cho Kiosk và check-in/check-out.</summary>
        Task<DatPhong?> GetByMaBookingAsync(string maBooking);

        /// <summary>Lấy booking kèm đầy đủ chi tiết phòng, dịch vụ, khách hàng (Include).</summary>
        Task<DatPhong?> GetChiTietDayDuAsync(int idDatPhong);

        /// <summary>
        /// Tìm kiếm danh sách booking theo nhiều tiêu chí tùy chọn: tên khách hàng, SĐT, CCCD,
        /// số phòng, trạng thái booking. Hỗ trợ nhân viên tra cứu nhanh mà không cần nhớ mã booking.
        /// Kết quả bao gồm danh sách phòng thuộc booking (join ChiTietDatPhong → Phong).
        /// </summary>
        Task<IEnumerable<DatPhong>> TimKiemBookingAsync(
            string? tenKhachHang = null,
            string? sdt = null,
            string? cccd = null,
            string? soPhong = null,
            string? trangThai = null);
    }
}