using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormChatbotKhachHang
    {
        private System.ComponentModel.IContainer components = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private Panel pnlHeader = null!;
        private Label lblHeaderIcon = null!;
        private Label lblHeaderTitle = null!;
        private Label lblHeaderSubtitle = null!;
        private Button btnLamMoiChat = null!;
        private Button btnDongChat = null!;

        private Panel pnlChatContainer = null!;
        private FlowLayoutPanel flpMessages = null!;
        private Label lblThinkingStatus = null!;

        private Panel pnlQuickChips = null!;
        private Panel pnlFooterInput = null!;
        private TextBox txtInput = null!;
        private Button btnSend = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = AppTheme.Background;

            // ================= 1. Header Panel =================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = AppTheme.Surface,
                Padding = new Padding(20, 10, 20, 10)
            };

            lblHeaderIcon = new Label
            {
                Text = "🤖",
                Font = new Font(AppTheme.DefaultFontFamily, 22f),
                Location = new Point(16, 10),
                Size = new Size(45, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblHeaderTitle = new Label
            {
                Text = "TRỢ LÝ ẢO AI — TƯ VẤN KHÁCH SẠN",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(68, 12),
                AutoSize = true
            };

            var tenKhach = _taiKhoanDangNhap.IdKhachhangNavigation?.Ten ?? _taiKhoanDangNhap.TenDangnhap;
            lblHeaderSubtitle = new Label
            {
                Text = $"Chào bạn, {tenKhach}! Em sẵn sàng tư vấn khách sạn 24/7.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(70, 36),
                AutoSize = true
            };

            btnDongChat = new Button
            {
                Text = "✖",
                Size = new Size(34, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            btnDongChat.Location = new Point(this.ClientSize.Width - 46, 15);
            AppTheme.ApplySecondaryButton(btnDongChat);
            btnDongChat.Click += (s, e) => YeuCauDong?.Invoke(this, EventArgs.Empty);

            btnLamMoiChat = new Button
            {
                Text = "🔄",
                Size = new Size(34, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            btnLamMoiChat.Location = new Point(this.ClientSize.Width - 86, 15);
            AppTheme.ApplySecondaryButton(btnLamMoiChat);
            btnLamMoiChat.Click += (s, e) => LamMoiHoiThoai();

            pnlHeader.Controls.AddRange(new Control[]
            {
                lblHeaderIcon,
                lblHeaderTitle,
                lblHeaderSubtitle,
                btnLamMoiChat,
                btnDongChat
            });

            // ================= 2. Footer Input Panel =================
            pnlFooterInput = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = AppTheme.Surface,
                Padding = new Padding(16, 12, 16, 12)
            };

            btnSend = new Button
            {
                Text = "Gửi 🚀",
                Size = new Size(100, 36),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            AppTheme.ApplyPrimaryButton(btnSend);
            btnSend.Click += async (s, e) => await GuiTinNhanAsync();

            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 0, 12, 0)
            };
            txtInput.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await GuiTinNhanAsync();
                }
            };

            var pnlInputWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 12, 0)
            };
            pnlInputWrapper.Controls.Add(txtInput);

            pnlFooterInput.Controls.Add(pnlInputWrapper);
            pnlFooterInput.Controls.Add(btnSend);

            // ================= 3. Quick Chips Panel =================
            pnlQuickChips = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(16, 6, 16, 6)
            };

            TaoQuickChips();

            // ================= 4. Chat Messages Container =================
            pnlChatContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(20, 10, 20, 10)
            };

            lblThinkingStatus = new Label
            {
                Text = "🤖 Trợ lý ảo đang suy nghĩ...",
                Dock = DockStyle.Bottom,
                Height = 26,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.Primary,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Visible = false
            };

            flpMessages = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = AppTheme.Background,
                Padding = new Padding(0, 0, 15, 0)
            };
            flpMessages.SizeChanged += (s, e) => CapNhatKichThuocBongBong();

            pnlChatContainer.Controls.Add(flpMessages);
            pnlChatContainer.Controls.Add(lblThinkingStatus);

            // Add all to form
            this.Controls.Add(pnlChatContainer);
            this.Controls.Add(pnlQuickChips);
            this.Controls.Add(pnlFooterInput);
            this.Controls.Add(pnlHeader);
        }

        #endregion
    }
}

