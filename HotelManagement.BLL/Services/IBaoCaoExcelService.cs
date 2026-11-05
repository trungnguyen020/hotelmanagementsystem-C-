using System;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service chuyên trách xuất báo cáo doanh thu ra định dạng Excel (.xlsx) bằng ClosedXML (DCKLTTQ-78).
    /// Áp dụng mô hình chuẩn Task, IProgress, CancellationToken cho tác vụ nặng (DCKLTTQ-93).
    /// </summary>
    public interface IBaoCaoExcelService
    {
        /// <summary>
        /// Tạo mảng byte của tệp Excel từ dữ liệu BaoCaoDoanhThuDto.
        /// </summary>
        byte[] SinhBaoCaoExcel(BaoCaoDoanhThuDto baoCao);

        /// <summary>
        /// Xuất tệp Excel và lưu trực tiếp xuống đường dẫn trên ổ đĩa.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task XuatFileExcelAsync(
            BaoCaoDoanhThuDto baoCao,
            string duongDanTep,
            IProgress<(int PhanTram, string ThongDiep)>? progress = null,
            CancellationToken cancellationToken = default);
    }
}
