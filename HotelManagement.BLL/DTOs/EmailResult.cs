using System;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// Kết quả thực hiện gửi email thông báo (DCKLTTQ-81).
    /// Chuẩn bị sẵn dữ liệu phục vụ DCKLTTQ-82 (lưu lịch sử gửi thông báo).
    /// Không ném ngoại lệ ra ngoài luồng nghiệp vụ chính khi có lỗi.
    /// </summary>
    public record EmailResult(
        bool ThanhCong,
        string? LyDoLoi = null,
        string? NguoiNhan = null,
        string? TieuDe = null,
        DateTime? ThoiGianGui = null
    );
}

