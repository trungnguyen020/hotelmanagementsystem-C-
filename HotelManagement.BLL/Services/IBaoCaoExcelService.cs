using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service chuyên trách xuất báo cáo doanh thu ra định dạng Excel (.xlsx) bằng ClosedXML (DCKLTTQ-78).
    /// Tách biệt hoàn toàn phần định dạng, layout, style bảng tính khỏi logic nghiệp vụ.
    /// </summary>
    public interface IBaoCaoExcelService
    {
        /// <summary>
        /// Tạo mảng byte của tệp Excel từ dữ liệu BaoCaoDoanhThuDto.
        /// </summary>
        byte[] SinhBaoCaoExcel(BaoCaoDoanhThuDto baoCao);

        /// <summary>
        /// Xuất tệp Excel và lưu trực tiếp xuống đường dẫn trên ổ đĩa.
        /// </summary>
        Task XuatFileExcelAsync(BaoCaoDoanhThuDto baoCao, string duongDanTep);
    }
}
