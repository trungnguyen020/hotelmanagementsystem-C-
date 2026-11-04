using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Giao diện dịch vụ Chatbot AI Gemini cho Kiosk tự phục vụ (DCKLTTQ-88).
    /// Hỗ trợ multi-turn conversation, tích hợp dữ liệu phòng/giá thực tế từ database,
    /// bọc try-catch nội bộ và fallback thân thiện khi xảy ra lỗi.
    /// </summary>
    public interface IChatbotService
    {
        /// <summary>
        /// Gửi tin nhắn mới cùng lịch sử hội thoại của phiên làm việc tới Gemini API.
        /// </summary>
        /// <param name="tinNhanMoi">Câu hỏi hoặc yêu cầu mới từ khách.</param>
        /// <param name="lichSuHoiThoai">Lịch sử các tin nhắn trước đó trong phiên Kiosk hiện tại.</param>
        /// <param name="cancellationToken">Token hủy tác vụ.</param>
        /// <returns>Đối tượng ChatMessageDto chứa phản hồi từ bot hoặc thông báo fallback an toàn.</returns>
        Task<ChatMessageDto> GuiTinNhanAsync(
            string tinNhanMoi,
            IReadOnlyList<ChatMessageDto> lichSuHoiThoai,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy tóm tắt ngữ cảnh phòng và mức giá thực tế từ cơ sở dữ liệu để đưa vào System Prompt.
        /// </summary>
        Task<string> LayThongTinNguCanhPhongVaGiaAsync();
    }
}

