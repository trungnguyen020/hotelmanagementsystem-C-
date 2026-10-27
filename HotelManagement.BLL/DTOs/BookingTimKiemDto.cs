namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO hiển thị dòng kết quả tìm kiếm booking trong FormTraCuuBooking.
    /// Chứa thông tin tóm tắt đủ để nhân viên nhận diện nhanh booking cần thao tác,
    /// không cần phải nhớ mã booking.
    /// </summary>
    public class BookingTimKiemDto
    {
        public int IdDatPhong { get; set; }
        public string MaBooking { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;

        // Thông tin khách hàng
        public string TenKhachHang { get; set; } = string.Empty;
        public string SdtKhachHang { get; set; } = string.Empty;
        public string CccdKhachHang { get; set; } = string.Empty;

        // Thời gian
        public DateTime NgayDat { get; set; }
        public DateOnly NgayNhanDuKien { get; set; }
        public DateOnly NgayTraDuKien { get; set; }

        // Phòng (danh sách số phòng gộp thành chuỗi, VD: "101, 102, 305")
        public string DanhSachSoPhong { get; set; } = string.Empty;
        public int SoPhong { get; set; }
        public int SoNguoi { get; set; }
    }
}
