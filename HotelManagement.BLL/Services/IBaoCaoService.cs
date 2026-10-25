using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ thống kê và báo cáo doanh thu theo bộ lọc (DCKLTTQ-78).
    /// </summary>
    public interface IBaoCaoService
    {
        /// <summary>
        /// Tổng hợp báo cáo doanh thu và tỷ lệ lấp đầy phòng theo khoảng ngày và loại phòng.
        /// </summary>
        Task<BaoCaoDoanhThuDto> LayBaoCaoDoanhThuAsync(BaoCaoFilterDto filter, string nguoiLap = "Admin");

        /// <summary>
        /// Lấy danh mục loại phòng để nạp vào ComboBox bộ lọc.
        /// </summary>
        Task<List<LoaiPhongDto>> LayDanhSachLoaiPhongAsync();
    }
}
