using System.Collections.Generic;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO đại diện cho một dòng dữ liệu phòng được đọc từ file Excel và kết quả kiểm tra tính hợp lệ.
    /// Phục vụ DCKLTTQ-79: "Import danh sách phòng từ Excel có validate".
    /// </summary>
    public class PhongImportDto
    {
        /// <summary>
        /// Số thứ tự dòng trong file Excel (1-indexed theo dòng thực tế trên trang tính).
        /// </summary>
        public int DongThuTu { get; set; }

        /// <summary>
        /// Số phòng đọc được từ file Excel.
        /// </summary>
        public string SoPhong { get; set; } = string.Empty;

        /// <summary>
        /// Tên loại phòng đọc được từ file Excel.
        /// </summary>
        public string TenLoaiPhong { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái phòng đọc được từ file Excel (Trống, Đang ở, Đang dọn, Bảo trì).
        /// </summary>
        public string TrangThai { get; set; } = string.Empty;

        /// <summary>
        /// Cờ cho biết dòng này có hợp lệ hoàn toàn để import vào hệ thống hay không.
        /// Chỉ bằng true khi DanhSachLoi rỗng.
        /// </summary>
        public bool HopLe { get; set; }

        /// <summary>
        /// Danh sách tất cả các câu thông báo lỗi thu thập được trên dòng này (không dừng ở lỗi đầu tiên).
        /// </summary>
        public List<string> DanhSachLoi { get; set; } = new();

        /// <summary>
        /// IdLoaiPhong tương ứng trong cơ sở dữ liệu nếu tên loại phòng hợp lệ.
        /// </summary>
        public int? IdLoaiPhong { get; set; }

        /// <summary>
        /// Chuỗi gộp các lỗi để hiển thị trực tiếp lên DataGridView hoặc thông báo.
        /// </summary>
        public string ChiTietLoi => DanhSachLoi.Count > 0 ? string.Join("; ", DanhSachLoi) : string.Empty;

        /// <summary>
        /// Chuỗi hiển thị kết quả kiểm tra (Hợp lệ / Lỗi).
        /// </summary>
        public string KetQuaKiemTra => HopLe ? "Hợp lệ" : "Lỗi";
    }
}
