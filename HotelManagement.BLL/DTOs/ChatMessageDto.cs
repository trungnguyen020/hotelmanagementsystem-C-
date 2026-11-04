using System;

namespace HotelManagement.BLL.DTOs
{
    /// <summary>
    /// DTO đại diện cho một tin nhắn hội thoại giữa khách hàng và Chatbot Gemini tại Kiosk (DCKLTTQ-88).
    /// </summary>
    public class ChatMessageDto
    {
        /// <summary>
        /// Vai trò: "user" (khách hỏi) hoặc "model" (chatbot trả lời).
        /// </summary>
        public string Role { get; set; } = "user";

        /// <summary>
        /// Nội dung tin nhắn.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm gửi tin nhắn.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Đánh dấu tin nhắn phản hồi là thông báo fallback/lỗi thân thiện.
        /// </summary>
        public bool IsError { get; set; } = false;

        public static ChatMessageDto CreateUser(string content) =>
            new() { Role = "user", Content = content, Timestamp = DateTime.Now };

        public static ChatMessageDto CreateModel(string content, bool isError = false) =>
            new() { Role = "model", Content = content, Timestamp = DateTime.Now, IsError = isError };
    }
}

