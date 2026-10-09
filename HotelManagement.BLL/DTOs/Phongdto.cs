namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO truyền dữ liệu Phòng. TenLoaiPhong chỉ phục vụ hiển thị trên lưới,
    /// không ghi xuống DB (ghi xuống qua IdLoaiPhong).
    /// </summary>
    public class PhongDto
    {
        public int Id { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int IdLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
    }
}