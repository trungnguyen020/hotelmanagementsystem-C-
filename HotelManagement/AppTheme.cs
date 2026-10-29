using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelManagement
{
    /// <summary>
    /// Hệ thống quản lý màu sắc, font chữ và giao diện tập trung (Design System).
    /// Chủ đề: Warm Bronze & Elegant Neutral (Phong cách khách sạn thanh lịch, trung tính, ấm).
    /// </summary>
    public static class AppTheme
    {
        #region 1. Bảng màu chủ đạo (Color Palette - Warm Bronze)

        // Nền & bề mặt (Backgrounds & Surfaces)
        public static readonly Color Background = Color.FromArgb(247, 244, 239);        // Nền Form chính - trắng ngà ấm (#F7F4EF)
        public static readonly Color Surface = Color.FromArgb(255, 255, 255);           // Nền Card / Panel / DataGridView (#FFFFFF)
        public static readonly Color SurfaceSecondary = Color.FromArgb(239, 234, 226);  // Nền MenuStrip / Header / Toolbar (#EFEAE2)
        public static readonly Color SurfaceHover = Color.FromArgb(230, 223, 212);      // Nền khi hover item (#E6DFD4)
        public static readonly Color SurfaceAlt = Color.FromArgb(251, 249, 246);        // Hàng xen kẽ DataGridView (#FBF9F6)

        // Đường viền (Borders & Dividers)
        public static readonly Color Border = Color.FromArgb(218, 210, 198);            // Viền chính (#DAD2C6)
        public static readonly Color BorderLight = Color.FromArgb(233, 227, 217);       // Viền mờ ngăn cách (#E9E3D9)
        public static readonly Color BorderFocus = Color.FromArgb(166, 124, 82);        // Viền khi focus ô nhập liệu (#A67C52)

        // Chữ (Typography Colors)
        public static readonly Color TextPrimary = Color.FromArgb(45, 41, 37);          // Chữ chính - Warm Charcoal (#2D2925)
        public static readonly Color TextSecondary = Color.FromArgb(106, 98, 89);       // Chữ phụ, nhãn (#6A6259)
        public static readonly Color TextMuted = Color.FromArgb(154, 145, 135);         // Placeholder, chú thích (#9A9187)
        public static readonly Color TextOnPrimary = Color.FromArgb(255, 255, 255);     // Chữ trên nền nút màu nhấn (#FFFFFF)

        // Màu nhấn (Primary Accent - Warm Bronze)
        public static readonly Color Primary = Color.FromArgb(140, 98, 57);             // Bronze chính (#8C6239)
        public static readonly Color PrimaryHover = Color.FromArgb(116, 79, 45);         // Bronze đậm khi hover (#744F2D)
        public static readonly Color PrimaryActive = Color.FromArgb(92, 62, 34);        // Bronze khi bấm (#5C3E22)
        public static readonly Color PrimaryLight = Color.FromArgb(244, 235, 221);      // Nền highlight Bronze nhạt (#F4EBDD)
        public static readonly Color AccentGold = Color.FromArgb(176, 141, 87);         // Vàng đồng trang trí (#B08D57)
        public static readonly Color Ink = Color.FromArgb(43, 38, 34);                  // Nền đậm tuỳ chọn (#2B2622)

        #endregion

        #region 2. Bảng màu trạng thái (Status Colors - Đồng bộ Sơ đồ phòng & Nghiệp vụ)

        // Trống / Sẵn sàng / Thành công (Available / Success)
        public static readonly Color StatusSuccessText = Color.FromArgb(62, 107, 79);      // #3E6B4F
        public static readonly Color StatusSuccessBg = Color.FromArgb(231, 240, 232);      // #E7F0E8
        public static readonly Color StatusSuccessBorder = Color.FromArgb(143, 181, 154);  // #8FB59A
        public static readonly Color StatusSuccessSurface = Color.FromArgb(244, 248, 244); // #F4F8F4

        // Đang ở / Bận / Lỗi (Occupied / Danger / Error)
        public static readonly Color StatusDangerText = Color.FromArgb(155, 68, 54);       // #9B4436
        public static readonly Color StatusDangerBg = Color.FromArgb(247, 231, 226);       // #F7E7E2
        public static readonly Color StatusDangerBorder = Color.FromArgb(217, 160, 147);   // #D9A093
        public static readonly Color StatusDangerSurface = Color.FromArgb(251, 244, 242);  // #FBF4F2

        // Đang dọn / Cảnh báo / Chờ (Cleaning / Warning / Pending)
        public static readonly Color StatusWarningText = Color.FromArgb(138, 90, 27);      // #8A5A1B
        public static readonly Color StatusWarningBg = Color.FromArgb(250, 240, 218);      // #FAF0DA
        public static readonly Color StatusWarningBorder = Color.FromArgb(222, 196, 135);  // #DEC487
        public static readonly Color StatusWarningSurface = Color.FromArgb(252, 248, 239); // #FCF8EF

        // Bảo trì / Tạm ngưng / Trung tính (Maintenance / Neutral)
        public static readonly Color StatusNeutralText = Color.FromArgb(91, 99, 107);      // #5B636B
        public static readonly Color StatusNeutralBg = Color.FromArgb(236, 237, 238);      // #ECEDEE
        public static readonly Color StatusNeutralBorder = Color.FromArgb(179, 184, 189);  // #B3B8BD
        public static readonly Color StatusNeutralSurface = Color.FromArgb(245, 245, 244); // #F5F5F4

        #endregion

        #region 3. Hệ thống Font chữ (Typography)

        public const string DefaultFontFamily = "Segoe UI";

        public static readonly Font FontDisplay = new(DefaultFontFamily, 18f, FontStyle.Bold);
        public static readonly Font FontHeader = new(DefaultFontFamily, 14f, FontStyle.Bold);
        public static readonly Font FontSubHeader = new(DefaultFontFamily, 11f, FontStyle.Bold);
        public static readonly Font FontBody = new(DefaultFontFamily, 10f, FontStyle.Regular);
        public static readonly Font FontBodyBold = new(DefaultFontFamily, 10f, FontStyle.Bold);
        public static readonly Font FontSmall = new(DefaultFontFamily, 8.5f, FontStyle.Regular);
        public static readonly Font FontSmallBold = new(DefaultFontFamily, 8.5f, FontStyle.Bold);
        public static readonly Font FontCaption = new(DefaultFontFamily, 8f, FontStyle.Italic);

        #endregion

        #region 4. Helper Methods áp dụng Style giao diện

        /// <summary>
        /// Chuẩn hóa giao diện cho Form nền.
        /// </summary>
        public static void ApplyFormStyle(Form form)
        {
            form.BackColor = Background;
            form.Font = FontBody;
            form.ForeColor = TextPrimary;
        }

        /// <summary>
        /// Nút bấm chính (Call to Action) - Nền Teal, chữ trắng.
        /// </summary>
        public static void ApplyPrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Primary;
            btn.ForeColor = TextOnPrimary;
            btn.Font = FontBodyBold;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter -= OnPrimaryButtonMouseEnter;
            btn.MouseLeave -= OnPrimaryButtonMouseLeave;
            btn.MouseEnter += OnPrimaryButtonMouseEnter;
            btn.MouseLeave += OnPrimaryButtonMouseLeave;
        }

        private static void OnPrimaryButtonMouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
                btn.BackColor = PrimaryHover;
        }

        private static void OnPrimaryButtonMouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
                btn.BackColor = Primary;
        }

        /// <summary>
        /// Nút bấm phụ (Secondary Button) - Nền xám ngà nhẹ, viền bo, chữ xám xanh.
        /// </summary>
        public static void ApplySecondaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Border;
            btn.BackColor = Surface;
            btn.ForeColor = TextPrimary;
            btn.Font = FontBody;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter -= OnSecondaryButtonMouseEnter;
            btn.MouseLeave -= OnSecondaryButtonMouseLeave;
            btn.MouseEnter += OnSecondaryButtonMouseEnter;
            btn.MouseLeave += OnSecondaryButtonMouseLeave;
        }

        private static void OnSecondaryButtonMouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
            {
                btn.BackColor = SurfaceSecondary;
                btn.FlatAppearance.BorderColor = Primary;
            }
        }

        private static void OnSecondaryButtonMouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
            {
                btn.BackColor = Surface;
                btn.FlatAppearance.BorderColor = Border;
            }
        }

        /// <summary>
        /// Áp dụng giao diện thẻ Card (Panel trắng, viền thanh mảnh).
        /// </summary>
        public static void ApplyCardPanel(Panel pnl, int borderRadius = 8)
        {
            pnl.BackColor = Surface;
            pnl.Paint -= OnCardPanelPaint;
            pnl.Paint += OnCardPanelPaint;
        }

        private static void OnCardPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is Panel pnl)
            {
                using var pen = new Pen(Border, 1);
                var rect = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = CreateRoundedRectanglePath(rect, 8);
                e.Graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Áp dụng style chuẩn cho TextBox nhập liệu.
        /// </summary>
        public static void ApplyTextBox(TextBox txt)
        {
            txt.Font = FontBody;
            txt.ForeColor = TextPrimary;
            txt.BackColor = Surface;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// Áp dụng style hiện đại cho DataGridView.
        /// </summary>
        public static void ApplyDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BorderLight;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            // Header Style
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = SurfaceSecondary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontBodyBold;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
            dgv.ColumnHeadersHeight = 38;

            // Rows Default Style
            dgv.DefaultCellStyle.BackColor = Surface;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = FontBody;
            dgv.DefaultCellStyle.SelectionBackColor = PrimaryLight;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            dgv.RowTemplate.Height = 32;

            // Alternating Row Style (Zebra stripe nhẹ)
            dgv.AlternatingRowsDefaultCellStyle.BackColor = SurfaceAlt;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = PrimaryLight;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;
        }

        /// <summary>
        /// Tạo badge trạng thái (pill nhãn) đồng bộ màu sắc.
        /// </summary>
        public static void ApplyStatusBadge(Label lbl, string trangThai)
        {
            lbl.AutoSize = true;
            lbl.Font = FontSmallBold;
            lbl.Padding = new Padding(10, 4, 10, 4);

            switch (trangThai.Trim())
            {
                case "Trống":
                case "Đã check-out":
                case "Đã thanh toán":
                case "Thành công":
                    lbl.ForeColor = StatusSuccessText;
                    lbl.BackColor = StatusSuccessBg;
                    break;

                case "Đang ở":
                case "Chưa thanh toán":
                case "Đã hủy":
                case "Lỗi":
                    lbl.ForeColor = StatusDangerText;
                    lbl.BackColor = StatusDangerBg;
                    break;

                case "Đang dọn":
                case "Chờ xác nhận":
                case "Cảnh báo":
                    lbl.ForeColor = StatusWarningText;
                    lbl.BackColor = StatusWarningBg;
                    break;

                case "Bảo trì":
                default:
                    lbl.ForeColor = StatusNeutralText;
                    lbl.BackColor = StatusNeutralBg;
                    break;
            }
        }

        /// <summary>
        /// Tạo GraphicsPath hình chữ nhật bo tròn.
        /// </summary>
        public static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Góc trên bên trái
            path.AddArc(arc, 180, 90);

            // Góc trên bên phải
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Góc dưới bên phải
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Góc dưới bên trái
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
