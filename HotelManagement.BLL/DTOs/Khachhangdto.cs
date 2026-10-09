namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO cho Khách hàng (DCKLTTQ-67).
    /// Mapping đúng với entity KhachHang: IdKhachhang, Ten, Sdt, Cccd, Gioitinh, Ngaysinh, Email.
    /// </summary>
    public class KhachHangDto
    {
        public int IdKhachHang { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string Cccd { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Sdt { get; set; } = string.Empty;
        public string? GioiTinh { get; set; }
        public DateOnly? NgaySinh { get; set; }
    }
}