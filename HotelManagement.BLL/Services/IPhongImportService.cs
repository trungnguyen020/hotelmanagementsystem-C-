using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Giao diện dịch vụ xử lý đọc, xác thực và lưu dữ liệu phòng import từ Excel (DCKLTTQ-79).
    /// </summary>
    public interface IPhongImportService
    {
        /// <summary>
        /// Đọc file Excel từ đường dẫn trên đĩa và thực hiện kiểm tra tính hợp lệ toàn diện từng dòng.
        /// Gom toàn bộ lỗi theo từng dòng, không dừng ở lỗi đầu tiên.
        /// </summary>
        /// <param name="duongDanFile">Đường dẫn tuyệt đối đến file Excel (.xlsx hoặc .xls)</param>
        /// <returns>Danh sách các dòng đã đọc kèm thông tin kết quả kiểm tra</returns>
        Task<List<PhongImportDto>> DocVaValidateAsync(string duongDanFile);

        /// <summary>
        /// Đọc file Excel từ Stream và thực hiện kiểm tra tính hợp lệ toàn diện từng dòng.
        /// </summary>
        Task<List<PhongImportDto>> DocVaValidateAsync(Stream stream);

        /// <summary>
        /// Lưu tất cả các dòng hợp lệ vào cơ sở dữ liệu qua IUnitOfWork trong một transaction tường minh.
        /// Bỏ qua các dòng bị lỗi.
        /// </summary>
        /// <param name="danhSach">Danh sách các dòng đã được kiểm tra</param>
        /// <returns>Số lượng phòng đã được thêm thành công vào cơ sở dữ liệu</returns>
        Task<int> LuuDanhSachHopLeAsync(IEnumerable<PhongImportDto> danhSach);

        /// <summary>
        /// Sinh file Excel mẫu (.xlsx) chuẩn cấu trúc cột và dữ liệu mẫu để người dùng tải về tham khảo.
        /// </summary>
        /// <returns>Mảng byte của file Excel sinh ra bằng ClosedXML</returns>
        byte[] TaoFileMauExcel();
    }
}
