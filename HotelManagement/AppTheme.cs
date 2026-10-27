using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelManagement
{
    /// <summary>
    /// Hệ thống quản lý màu sắc, font chữ và giao diện tập trung (Design System).
    /// Chủ đề: Coastal Teal & Deep Ocean (Phong cách nghỉ dưỡng biển cao cấp, thanh lịch, trang nhã).
    /// </summary>
    public static class AppTheme
    {
        #region 1. Bảng màu chủ đạo (Color Palette - Coastal Resort)

        // Nền & bề mặt (Backgrounds & Surfaces)
        public static readonly Color Background = Color.FromArgb(244, 247, 249);        // Nền Form chính (#F4F7F9)
        public static readonly Color Surface = Color.FromArgb(255, 255, 255);           // Nền Card / Panel / DataGridView (#FFFFFF)
        public static readonly Color SurfaceSecondary = Color.FromArgb(235, 242, 245);  // Nền MenuStrip / Header / Toolbar (#EBF2F5)
        public static readonly Color SurfaceHover = Color.FromArgb(226, 236, 240);      // Nền khi hover item (#E2ECF0)

        // Đường viền (Borders & Dividers)
        public static readonly Color Border = Color.FromArgb(209, 223, 230);            // Viền chính (#D1DFE6)
        public static readonly Color BorderLight = Color.FromArgb(229, 239, 244);       // Viền mờ ngăn cách (#E5EFF4)
        public static readonly Color BorderFocus = Color.FromArgb(15, 118, 110);        // Viền khi focus ô nhập liệu

        // Chữ (Typography Colors)
        public static readonly Color TextPrimary = Color.FromArgb(22, 37, 52);          // Chữ chính - Deep Navy Charcoal (#162534)
        public static readonly Color TextSecondary = Color.FromArgb(90, 111, 128);      // Chữ phụ, nhãn (#5A6F80)
        public static readonly Color TextMuted = Color.FromArgb(143, 160, 173);         // Placeholder, chú thích (#8FA0AD)
        public static readonly Color TextOnPrimary = Color.FromArgb(255, 255, 255);     // Chữ trên nền nút màu nhấn (#FFFFFF)

        // Màu nhấn (Primary Accent - Deep Teal)
        public static readonly Color Primary = Color.FromArgb(15, 118, 110);            // Teal chính (#0F766E)
        public static readonly Color PrimaryHover = Color.FromArgb(17, 94, 89);          // Teal đậm khi hover (#115E59)
        public static readonly Color PrimaryActive = Color.FromArgb(19, 78, 74);        // Teal khi bấm (#134E4A)
        public static readonly Color PrimaryLight = Color.FromArgb(224, 246, 244);       // Nền highlight Teal nhạt (#E0F6F4)

        // Màu nhấn phụ (Secondary Accent - Ocean Blue)
        public static readonly Color OceanBlue = Color.FromArgb(2, 132, 199);           // Ocean Blue (#0284C7)
        public static readonly Color OceanBlueHover = Color.FromArgb(3, 105, 161);      // Ocean Blue hover (#0369A1)
        public static readonly Color OceanBlueLight = Color.FromArgb(224, 242, 254);    // Ocean Blue nhạt (#E0F2FE)

        #endregion

        #region 2. Bảng màu trạng thái (Status Colors - Đồng bộ Sơ đồ phòng & Nghiệp vụ)

        // Trống / Sẵn sàng / Thành công (Available / Success)
        public static readonly Color StatusSuccessText = Color.FromArgb(6, 95, 70);     // #065F46
        public static readonly Color StatusSuccessBg = Color.FromArgb(209, 250, 229);   // #D1FAE5
        public static readonly Color StatusSuccessBorder = Color.FromArgb(16, 185, 129);// #10B981

        // Đang ở / Bận / Cảnh báo cao / Lỗi (Occupied / Danger / Error)
        public static readonly Color StatusDangerText = Color.FromArgb(153, 27, 27);    // #991B1B
        public static readonly Color StatusDangerBg = Color.FromArgb(254, 226, 226);    // #FEE2E2
        public static readonly Color StatusDangerBorder = Color.FromArgb(239, 68, 68);  // #EF4444

        // Đang dọn / Cảnh báo / Chờ (Cleaning / Warning / Pending)
        public static readonly Color StatusWarningText = Color.FromArgb(146, 64, 14);   // #92400E
        public static readonly Color StatusWarningBg = Color.FromArgb(254, 243, 199);   // #FEF3C7
        public static readonly Color StatusWarningBorder = Color.FromArgb(245, 158, 11);// #F59E0B

        // Bảo trì / Tạm ngưng / Trung tính (Maintenance / Neutral)
        public static readonly Color StatusNeutralText = Color.FromArgb(71, 85, 105);   // #475569
        public static readonly Color StatusNeutralBg = Color.FromArgb(241, 245, 249);   // #F1F5F9
        public static readonly Color StatusNeutralBorder = Color.FromArgb(148, 163, 184);// #94A3B8

        #endregion

        #region 3. Hệ thống Font chữ (Typography)

        public const string DefaultFontFamily = "Segoe UI";

        public static readonly Font FontDisplay = new(DefaultFontFamily, 15f, FontStyle.Bold);
        public static readonly Font FontHeader = new(DefaultFontFamily, 12f, FontStyle.Bold);
        public static readonly Font FontSubHeader = new(DefaultFontFamily, 10.5f, FontStyle.Bold);
        public static readonly Font FontBody = new(DefaultFontFamily, 9.5f, FontStyle.Regular);
        public static readonly Font FontBodyBold = new(DefaultFontFamily, 9.5f, FontStyle.Bold);
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
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 252, 253);
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
