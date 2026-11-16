using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement.Forms
{
    public partial class FormChatbotKhachHang : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private readonly List<ChatMessageDto> _chatHistory = new();
        private bool _isAiReplying = false;

        /// <summary>
        /// Sự kiện phát ra khi người dùng bấm nút đóng [✖] trên Header của Chatbot Drawer.
        /// </summary>
        public event EventHandler? YeuCauDong;

        public FormChatbotKhachHang(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap ?? throw new ArgumentNullException(nameof(taiKhoanDangNhap));
            InitializeComponent();

            this.Load += (_, _) => LamMoiHoiThoai();
        }

        private void TaoQuickChips()
        {
            pnlQuickChips.Controls.Clear();

            string[] suggestions = new[]
            {
                "💰 Giá các loại phòng?",
                "🕒 Giờ nhận & trả phòng?",
                "🛎️ Khách sạn có dịch vụ gì?",
                "🔍 Hôm nay còn phòng trống không?",
                "📍 Quy định hủy đặt phòng?"
            };

            int curX = 16;
            foreach (var cauHoi in suggestions)
            {
                var btnChip = new Button
                {
                    Text = cauHoi,
                    AutoSize = true,
                    Height = 28,
                    Location = new Point(curX, 7),
                    Cursor = Cursors.Hand,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppTheme.Surface,
                    ForeColor = AppTheme.Primary,
                    Font = AppTheme.FontSmall
                };
                btnChip.FlatAppearance.BorderColor = AppTheme.PrimaryLight;
                btnChip.Click += async (_, _) =>
                {
                    txtInput.Text = cauHoi;
                    await GuiTinNhanAsync();
                };

                pnlQuickChips.Controls.Add(btnChip);
                curX += btnChip.PreferredSize.Width + 10;
            }
        }

        private void LamMoiHoiThoai()
        {
            _chatHistory.Clear();
            flpMessages.Controls.Clear();

            var tenKhach = _taiKhoanDangNhap.IdKhachhangNavigation?.Ten ?? _taiKhoanDangNhap.TenDangnhap;
            string welcomeMsg = $"Xin chào {tenKhach}! 🛎️ Em là trợ lý ảo của khách sạn Warm Bronze.\nEm có thể giải đáp cho Quý khách về các loại phòng, mức giá, số phòng trống hôm nay và chính sách nhận/trả phòng. Quý khách cần em tư vấn thông tin gì ạ?";
            ThemBongBongTinNhan("model", welcomeMsg);
        }

        private async Task GuiTinNhanAsync()
        {
            string cauHoi = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(cauHoi) || _isAiReplying)
                return;

            txtInput.Clear();

            // 1. Thêm tin nhắn của User
            ThemBongBongTinNhan("user", cauHoi);
            _chatHistory.Add(ChatMessageDto.CreateUser(cauHoi));

            // 2. Trạng thái AI đang trả lời
            _isAiReplying = true;
            lblThinkingStatus.Visible = true;
            btnSend.Enabled = false;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var chatbotService = scope.ServiceProvider.GetRequiredService<IChatbotService>();

                var phanHoi = await chatbotService.GuiTinNhanAsync(cauHoi, _chatHistory);
                ThemBongBongTinNhan(phanHoi.Role, phanHoi.Content, phanHoi.IsError);
                _chatHistory.Add(phanHoi);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[FormChatbotKhachHang] Lỗi khi gọi ChatbotService: {Message}", ex.Message);
                string fallback = "Xin lỗi, hệ thống trợ lý ảo hiện không thể kết nối. Quý khách vui lòng thử lại sau hoặc liên hệ lễ tân để được hỗ trợ trực tiếp.";
                ThemBongBongTinNhan("model", fallback, isError: true);
            }
            finally
            {
                _isAiReplying = false;
                lblThinkingStatus.Visible = false;
                btnSend.Enabled = true;
                txtInput.Focus();
            }
        }

        private void ThemBongBongTinNhan(string role, string noiDung, bool isError = false)
        {
            bool isUser = role.Equals("user", StringComparison.OrdinalIgnoreCase);

            int maxWidth = Math.Max(flpMessages.ClientSize.Width - 40, 400);
            var row = new Panel
            {
                Width = maxWidth,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 10)
            };

            var bubble = new Panel
            {
                Padding = new Padding(14, 10, 14, 10),
                BackColor = isUser
                    ? AppTheme.PrimaryLight
                    : (isError ? AppTheme.StatusWarningBg : AppTheme.Surface)
            };
            AppTheme.ApplyCardPanel(bubble);

            var lblText = new Label
            {
                Text = noiDung,
                Font = AppTheme.FontBody,
                ForeColor = isError ? AppTheme.StatusWarningText : AppTheme.TextPrimary,
                AutoSize = true,
                MaximumSize = new Size(Math.Min(maxWidth - 100, 560), 0),
                Location = new Point(14, 10)
            };

            var lblTime = new Label
            {
                Text = (isUser ? "Bạn • " : "Trợ lý AI • ") + DateTime.Now.ToString("HH:mm"),
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize = true
            };

            bubble.Controls.Add(lblText);
            bubble.Controls.Add(lblTime);

            int bubbleWidth = Math.Min(Math.Max(lblText.PreferredWidth + 30, 140), Math.Min(maxWidth - 80, 580));
            lblTime.Location = new Point(14, lblText.PreferredHeight + 14);
            bubble.Size = new Size(bubbleWidth, lblText.PreferredHeight + lblTime.PreferredHeight + 22);

            if (isUser)
            {
                bubble.Location = new Point(row.Width - bubble.Width - 10, 0);
                bubble.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }
            else
            {
                bubble.Location = new Point(10, 0);
                bubble.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            row.Height = bubble.Height + 4;
            row.Controls.Add(bubble);

            flpMessages.Controls.Add(row);
            flpMessages.ScrollControlIntoView(row);
        }

        private void CapNhatKichThuocBongBong()
        {
            int maxWidth = Math.Max(flpMessages.ClientSize.Width - 40, 400);
            foreach (Control c in flpMessages.Controls)
            {
                if (c is Panel row)
                {
                    row.Width = maxWidth;
                }
            }
        }
    }
}

