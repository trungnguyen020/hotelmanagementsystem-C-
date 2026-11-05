using System;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Service tổng hợp dữ liệu số liệu & chuỗi thời gian cho Dashboard biểu đồ (DCKLTTQ-84).
    /// Áp dụng mô hình chuẩn Task, IProgress, CancellationToken cho tác vụ nặng (DCKLTTQ-93).
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Tổng hợp toàn bộ dữ liệu KPI, biểu đồ cột doanh thu theo ngày,
        /// biểu đồ tròn trạng thái phòng, và biểu đồ đường tỷ lệ lấp đầy theo khoảng ngày.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task<DashboardDataDto> LayDuLieuDashboardAsync(
            DashboardFilterDto filter,
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default);
    }
}

