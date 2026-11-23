namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO truyền dữ liệu Loại phòng giữa tầng UI và tầng BLL,
    /// tách biệt với entity do EF Core scaffold sinh ra.
    /// </summary>
    public class LoaiPhongDto
    {
        public int Id { get; set; }
        public string MaLoai => $"LP{Id:D2}";
        public string TenLoaiPhong { get; set; } = string.Empty;
        public int SoGiuong => SucChua > 2 ? 2 : 1;
        public decimal Gia { get; set; }
        public decimal GiaTheoGio { get; set; }
        public int SucChua { get; set; }
        public string? MoTa { get; set; }
    }
}
