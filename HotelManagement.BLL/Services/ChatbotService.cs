using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Triển khai dịch vụ Chatbot AI tại Kiosk qua Gemini REST API (DCKLTTQ-88).
    /// Đọc API Key an toàn từ User Secrets / IConfiguration.
    /// Bọc try-catch toàn bộ lời gọi API, bảo đảm fallback thân thiện và không gây gián đoạn Kiosk.
    /// </summary>
    public class ChatbotService : IChatbotService
    {
        private static readonly HttpClient _defaultHttpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        private const string DefaultModel = "gemini-2.5-flash";
        private const string FallbackModel = "gemini-flash-latest";

        private const string ThongBaoLoiFallback =
            "Xin lỗi, hệ thống tư vấn hiện không khả dụng, quý khách vui lòng liên hệ lễ tân để được hỗ trợ trực tiếp.";

        public ChatbotService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            HttpClient? httpClient = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpClient = httpClient ?? _defaultHttpClient;
        }

        #region Helper Đọc Cấu Hình

        private string LayApiKey()
        {
            return _configuration["Gemini:ApiKey"]
                ?? _configuration["GeminiApiKey"]
                ?? _configuration["Google:ApiKey"]
                ?? _configuration["GeminiKey"]
                ?? string.Empty;
        }

        private string LayModelName()
        {
            return _configuration["Gemini:Model"]
                ?? _configuration["GeminiModel"]
                ?? DefaultModel;
        }

        #endregion

        #region IChatbotService Implementation

        public async Task<string> LayThongTinNguCanhPhongVaGiaAsync()
        {
            try
            {
                var sb = new StringBuilder();
                var homNay = DateOnly.FromDateTime(DateTime.Today);
                var ngayMai = homNay.AddDays(1);

                // 1. Lấy toàn bộ danh mục loại phòng
                var dsLoaiPhong = (await _unitOfWork.LoaiPhongRepository.GetAllAsync()).ToList();

                // 2. Lấy danh sách phòng trống thực tế trong ngày hôm nay
                IEnumerable<Phong> dsPhongTrong;
                try
                {
                    dsPhongTrong = await _unitOfWork.DatPhongRepository.GetPhongTrongAsync(homNay, ngayMai);
                }
                catch
                {
                    dsPhongTrong = Enumerable.Empty<Phong>();
                }
                var phongTrongList = dsPhongTrong.ToList();

                // 3. Lấy tất cả phòng để thống kê tổng số
                IEnumerable<Phong> dsTatCaPhong;
                try
                {
                    dsTatCaPhong = await _unitOfWork.PhongRepository.GetAllAsync();
                }
                catch
                {
                    dsTatCaPhong = Enumerable.Empty<Phong>();
                }
                var tatCaPhongList = dsTatCaPhong.ToList();

                sb.AppendLine("=== THÔNG TIN PHÒNG VÀ GIÁ THỰC TẾ TRONG HỆ THỐNG ===");
                sb.AppendLine($"- Thời gian hệ thống: {DateTime.Now:dd/MM/yyyy HH:mm}");
                sb.AppendLine($"- Ngày kiểm tra phòng trống: Hôm nay ({homNay:dd/MM/yyyy}) đến ({ngayMai:dd/MM/yyyy})");
                sb.AppendLine("- Danh sách loại phòng:");

                if (dsLoaiPhong.Count == 0)
                {
                    sb.AppendLine("  (Hiện chưa có dữ liệu loại phòng trong hệ thống)");
                }
                else
                {
                    foreach (var lp in dsLoaiPhong)
                    {
                        int soPhongTrong = phongTrongList.Count(p => p.IdLoaiphong == lp.IdLoaiphong);
                        int tongSoPhong = tatCaPhongList.Count(p => p.IdLoaiphong == lp.IdLoaiphong);

                        sb.AppendLine($"  * Loại phòng: {lp.Ten}");
                        sb.AppendLine($"    - Mức giá niêm yết: {lp.Mucgia:N0} VNĐ / đêm");
                        sb.AppendLine($"    - Sức chứa tối đa: {lp.Succhua} người / phòng");
                        if (!string.IsNullOrWhiteSpace(lp.Mota))
                        {
                            sb.AppendLine($"    - Mô tả tiện ích: {lp.Mota}");
                        }
                        sb.AppendLine($"    - Tình trạng trống hôm nay: Còn {soPhongTrong} phòng trống / Tổng số {tongSoPhong} phòng");
                    }
                }

                sb.AppendLine();
                sb.AppendLine("=== CHÍNH SÁCH KHÁCH SẠN ===");
                sb.AppendLine("1. Giờ nhận phòng (Check-in): Từ 14:00 hàng ngày.");
                sb.AppendLine("2. Giờ trả phòng (Check-out): Trước 12:00 trưa hàng ngày.");
                sb.AppendLine("3. Nhận phòng sớm / Trả phòng trễ: Tùy thuộc tình trạng phòng thực tế và liên hệ Lễ tân sảnh chính để được phục vụ.");
                sb.AppendLine("4. Chính sách hủy phòng: Miễn phí nếu khách báo hủy trước 24 giờ so với ngày nhận phòng. Hủy trong vòng 24 giờ tính phí 1 đêm đầu tiên.");
                sb.AppendLine("5. Đặt phòng & Hủy phòng: Chatbot CHỈ hỗ trợ tư vấn thông tin, KHÔNG thực hiện thao tác đặt phòng hay hủy phòng trực tiếp. Khách vui lòng đến quầy Lễ tân sảnh chính để hoàn tất.");

                return sb.ToString();
            }
            catch (Exception)
            {
                return "Khách sạn DCKLTTQ có các loại phòng đạt chuẩn chất lượng cao. Giờ check-in 14:00, check-out 12:00. Quý khách vui lòng liên hệ Lễ tân để biết thêm chi tiết.";
            }
        }

        public async Task<ChatMessageDto> GuiTinNhanAsync(
            string tinNhanMoi,
            IReadOnlyList<ChatMessageDto> lichSuHoiThoai,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tinNhanMoi))
            {
                return ChatMessageDto.CreateModel("Quý khách vui lòng nhập nội dung câu hỏi cần tư vấn ạ.");
            }

            string apiKey = LayApiKey();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ChatMessageDto.CreateModel(ThongBaoLoiFallback, isError: true);
            }

            try
            {
                // 1. Chuẩn bị ngữ cảnh dữ liệu động từ DB
                string nguCanhDuLieu = await LayThongTinNguCanhPhongVaGiaAsync();

                // 2. Xây dựng System Prompt định hướng nghiệp vụ
                string systemPrompt = $@"Bạn là Trợ lý Ảo tư vấn tại Kiosk tự phục vụ của Khách sạn DCKLTTQ.
BẠN CÓ NHIỆM VỤ DUY NHẤT:
1. Tư vấn chi tiết về các loại phòng, mức giá niêm yết, sức chứa tối đa và số phòng trống thực tế.
2. Giải đáp các chính sách của khách sạn: giờ nhận phòng (check-in 14:00), giờ trả phòng (check-out 12:00), chính sách hủy phòng và nhận/trả phòng sớm/muộn.
3. Hướng dẫn khách liên hệ quầy Lễ tân sảnh chính nếu khách muốn thực hiện đặt phòng, thay đổi hoặc hủy phòng (bạn TUYỆT ĐỐI KHÔNG tự nhận đặt phòng hay hủy phòng).

{nguCanhDuLieu}

QUY TẮC BẮT BUỘC KHI TRẢ LỜI:
- CHỈ sử dụng dữ liệu giá và phòng được cung cấp ở trên. TUYỆT ĐỐI KHÔNG tự suy đoán, bịa đặt loại phòng hoặc mức giá ngoài dữ liệu thực tế.
- Nếu khách hỏi về các chủ đề ngoài nghiệp vụ khách sạn (thời tiết, tin tức, chính trị, viết mã lập trình, giải toán...): Hãy từ chối thật lịch sự và hướng dẫn khách quay lại thông tin khách sạn. Ví dụ: 'Dạ xin lỗi quý khách, em là trợ lý ảo của khách sạn nên chỉ hỗ trợ tư vấn thông tin phòng và chính sách của khách sạn thôi ạ. Quý khách cần em tư vấn thêm gì về phòng nghỉ không ạ?'
- Phong cách giao tiếp: Lịch thiệp, ân cần, ngắn gọn, súc tích (phù hợp đọc trên màn hình cảm ứng Kiosk). Xưng 'Em' hoặc 'Tôi' và gọi khách là 'Quý khách'.
- Ngôn ngữ: Nếu khách hỏi bằng tiếng Việt thì trả lời bằng tiếng Việt; nếu khách hỏi bằng tiếng Anh thì trả lời bằng tiếng Anh.";

                // 3. Chuẩn bị danh sách contents cho multi-turn conversation
                var contents = new List<object>();

                // Chuẩn hóa lịch sử hội thoại: chỉ lấy tin nhắn hợp lệ, đảm bảo luồng xen kẽ role: user -> model
                string? lastRole = null;
                if (lichSuHoiThoai != null)
                {
                    foreach (var msg in lichSuHoiThoai)
                    {
                        if (msg == null || msg.IsError || string.IsNullOrWhiteSpace(msg.Content))
                            continue;

                        string currentRole = (msg.Role ?? "user").ToLowerInvariant();
                        if (currentRole != "user" && currentRole != "model")
                            currentRole = "user";

                        // Tránh 2 tin nhắn cùng role liên tiếp gây lỗi với Gemini API
                        if (currentRole == lastRole)
                            continue;

                        contents.Add(new
                        {
                            role = currentRole,
                            parts = new[] { new { text = msg.Content } }
                        });
                        lastRole = currentRole;
                    }
                }

                // Nếu tin nhắn cuối cùng trong lịch sử là "user", ta không thể chèn thêm "user" ngay lập tức,
                // do đó xóa tin nhắn "user" cũ hoặc chỉ gửi tin nhắn mới
                if (lastRole == "user" && contents.Count > 0)
                {
                    contents.RemoveAt(contents.Count - 1);
                }

                // Thêm câu hỏi mới của khách
                contents.Add(new
                {
                    role = "user",
                    parts = new[] { new { text = tinNhanMoi.Trim() } }
                });

                // 4. Xây dựng Request Body
                var requestBody = new
                {
                    system_instruction = new
                    {
                        parts = new[] { new { text = systemPrompt } }
                    },
                    contents = contents,
                    generationConfig = new
                    {
                        temperature = 0.3,
                        maxOutputTokens = 1000
                    }
                };

                string jsonPayload = JsonSerializer.Serialize(requestBody);
                string modelName = LayModelName();

                // 5. Gọi API với cơ chế tự động thử model chính rồi fallback model phụ nếu cần
                string? responseText = await GoiGeminiApiAsync(modelName, apiKey, jsonPayload, cancellationToken);
                if (string.IsNullOrWhiteSpace(responseText) && modelName != FallbackModel)
                {
                    responseText = await GoiGeminiApiAsync(FallbackModel, apiKey, jsonPayload, cancellationToken);
                }

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    return ChatMessageDto.CreateModel(ThongBaoLoiFallback, isError: true);
                }

                return ChatMessageDto.CreateModel(responseText.Trim(), isError: false);
            }
            catch (Exception)
            {
                // Fallback tuyệt đối an toàn: không bao giờ để exception lan ra ngoài làm crash UI
                return ChatMessageDto.CreateModel(ThongBaoLoiFallback, isError: true);
            }
        }

        private async Task<string?> GoiGeminiApiAsync(
            string model,
            string apiKey,
            string jsonPayload,
            CancellationToken cancellationToken)
        {
            try
            {
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(apiKey)}";

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDoc = JsonDocument.Parse(responseBody);

                if (jsonDoc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var firstPart = parts[0];
                        if (firstPart.TryGetProperty("text", out var textElem))
                        {
                            return textElem.GetString();
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}

