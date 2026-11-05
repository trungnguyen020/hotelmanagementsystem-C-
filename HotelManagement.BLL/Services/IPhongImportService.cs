using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Giao diện dịch vụ xử lý đọc, xác thực và lưu dữ liệu phòng import từ Excel (DCKLTTQ-79).
    /// Áp dụng mẫu Task, IProgress, CancellationToken cho tác vụ nặng (DCKLTTQ-93).
    /// </summary>
    public interface IPhongImportService
    {
        /// <summary>
        /// Đọc file Excel từ đường dẫn trên đĩa và thực hiện kiểm tra tính hợp lệ toàn diện từng dòng.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task<List<PhongImportDto>> DocVaValidateAsync(
            string duongDanFile,
            IProgress<(int PhanTram, string ThongDiep)>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Đọc file Excel từ Stream và thực hiện kiểm tra tính hợp lệ toàn diện từng dòng.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task<List<PhongImportDto>> DocVaValidateAsync(
            Stream stream,
            IProgress<(int PhanTram, string ThongDiep)>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lưu tất cả các dòng hợp lệ vào cơ sở dữ liệu qua IUnitOfWork trong một transaction tường minh.
        /// Hỗ trợ báo tiến trình qua IProgress và hủy giữa chừng qua CancellationToken.
        /// </summary>
        Task<int> LuuDanhSachHopLeAsync(
            IEnumerable<PhongImportDto> danhSach,
            IProgress<(int PhanTram, string ThongDiep)>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sinh file Excel mẫu (.xlsx) chuẩn cấu trúc cột và dữ liệu mẫu để người dùng tải về tham khảo.
        /// </summary>
        byte[] TaoFileMauExcel();
    }
}
