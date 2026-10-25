using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelManagement.BLL.Pdf
{
    /// <summary>
    /// Định nghĩa giao diện, màu sắc, font chữ và thông tin khách sạn cho hóa đơn PDF (DCKLTTQ-77).
    /// Tách biệt phần trình bày khỏi logic nghiệp vụ, chuẩn bị sẵn sàng cho Epic DCKLTTQ-98 (UI/UX).
    /// </summary>
    public class HoaDonPdfTheme
    {
        // Thông tin thương hiệu khách sạn
        public string TenKhachSan { get; set; } = "KHÁCH SẠN HOTEL MANAGEMENT";
        public string Slogan { get; set; } = "Trải nghiệm nghỉ dưỡng đẳng cấp & tiện nghi";
        public string DiaChi { get; set; } = "123 Đường Lê Lợi, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh";
        public string Hotline { get; set; } = "1900 6868 - (028) 3822 5555";
        public string Email { get; set; } = "contact@hotelmanagement.vn";
        public string Website { get; set; } = "www.hotelmanagement.vn";

        // Màu sắc giao diện (Hex Colors chuẩn thiết kế hiện đại)
        public string PrimaryColor { get; set; } = "#0D3B66";       // Xanh Navy đậm sang trọng
        public string SecondaryColor { get; set; } = "#1E5F8A";     // Xanh biển trang nhã
        public string AccentColor { get; set; } = "#D4AF37";        // Vàng kim (Gold) điểm nhấn
        public string TextColor { get; set; } = "#212529";          // Đen xám đậm dễ đọc
        public string MutedTextColor { get; set; } = "#6C757D";     // Xám trung tính cho ghi chú/phụ đề
        public string BackgroundColor { get; set; } = "#F8F9FA";    // Xám trắng nền phụ
        public string TableHeaderBgColor { get; set; } = "#0D3B66"; // Nền tiêu đề bảng
        public string TableAlternateBgColor { get; set; } = "#F9FBFC"; // Nền dòng xen kẽ
        public string BorderColor { get; set; } = "#DEE2E6";        // Đường viền bảng/khung
        public string SuccessColor { get; set; } = "#28A745";       // Xanh lá trạng thái "Đã thanh toán"
        public string WarningColor { get; set; } = "#DC3545";       // Đỏ cảnh báo / Chưa thanh toán

        // Font chữ và kích thước font (Segoe UI hỗ trợ tiếng Việt Unicode hoàn hảo trên Windows)
        public string FontFamily { get; set; } = "Segoe UI";
        public float TitleFontSize { get; set; } = 18f;
        public float SubTitleFontSize { get; set; } = 12f;
        public float SectionHeaderFontSize { get; set; } = 11f;
        public float BodyFontSize { get; set; } = 9.5f;
        public float SmallFontSize { get; set; } = 8.5f;

        // Cấu hình lề trang (Point)
        public float PageMargin { get; set; } = 28f;

        /// <summary>
        /// Tạo theme mặc định phong cách khách sạn cao cấp
        /// </summary>
        public static HoaDonPdfTheme MacDinh => new();
    }
}
