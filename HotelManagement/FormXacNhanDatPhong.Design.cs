using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormXacNhanDatPhong
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

        #region UI Controls

        private Label _lblHeaderTitle = null!;
        private Label _lblHeaderSub = null!;

        // Thông tin booking bên trái
        private Label _lblMaBookingVal = null!;
        private Label _lblKhachHangVal = null!;
        private Label _lblSdtVal = null!;
        private Label _lblThoiGianVal = null!;
        private Label _lblSoKhachVal = null!;
        private Label _lblDanhSachPhongVal = null!;
        private Label _lblTrangThaiVal = null!;
        private Label _lblEmailStatusVal = null!;

        // Khu vực mã QR bên phải
        private PictureBox _picQrCode = null!;
        private Label _lblQrHuongDan = null!;
        private Button _btnSaoChepMa = null!;
        private Button _btnLuuQr = null!;

        // Nút hành động đáy
        private Button _btnGuiEmail = null!;
        private Button _btnDong = null!;

        #endregion

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "XÁC NHẬN ĐẶT PHÒNG THÀNH CÔNG — MÃ QR TRA CỨU";
            Size = new Size(820, 560);
            MinimumSize = new Size(800, 540);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = AppTheme.Background;
            Font = AppTheme.FontBody;
            ForeColor = AppTheme.TextPrimary;

            // ================= 1. Header Panel =================
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(24, 12, 24, 12)
            };

            _lblHeaderTitle = new Label
            {
                Text = "✓ ĐẶT PHÒNG THÀNH CÔNG",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.StatusSuccessText,
                AutoSize = true,
                Location = new Point(24, 12)
            };

            _lblHeaderSub = new Label
            {
                Text = "Mã booking đã được ghi nhận. Khách hàng có thể dùng mã QR để tra cứu nhanh tại Kiosk.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(24, 40)
            };

            pnlHeader.Controls.AddRange(new Control[] { _lblHeaderTitle, _lblHeaderSub });

            // ================= 2. Body Container =================
            var pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 16, 20, 16)
            };

            // --- Card trái: Chi tiết đặt phòng (Width ~ 440) ---
            var pnlThongTin = new Panel
            {
                Location = new Point(20, 16),
                Size = new Size(440, 390),
                BackColor = AppTheme.Surface,
                Padding = new Padding(20)
            };
            AppTheme.ApplyCardPanel(pnlThongTin);

            var lblTieuDeThongTin = new Label
            {
                Text = "CHI TIẾT ĐẶT PHÒNG",
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(18, 16),
                AutoSize = true
            };

            int topY = 52;
            int stepY = 38;

            Label TaoNhan(string tieuDe, int y) => new()
            {
                Text = tieuDe,
                Location = new Point(18, y),
                AutoSize = true,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary
            };

            Label TaoGiaTri(int y, bool bold = false) => new()
            {
                Text = "Đang tải...",
                Location = new Point(150, y),
                AutoSize = true,
                Font = bold ? AppTheme.FontBodyBold : AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary
            };

            var lblNhanMa = TaoNhan("Mã Booking:", topY);
            _lblMaBookingVal = TaoGiaTri(topY, true);
            _lblMaBookingVal.Font = new Font(AppTheme.DefaultFontFamily, 12f, FontStyle.Bold);
            _lblMaBookingVal.ForeColor = AppTheme.Primary;

            topY += stepY;
            var lblNhanKH = TaoNhan("Khách hàng:", topY);
            _lblKhachHangVal = TaoGiaTri(topY, true);

            topY += stepY;
            var lblNhanSdt = TaoNhan("SĐT / CCCD:", topY);
            _lblSdtVal = TaoGiaTri(topY);

            topY += stepY;
            var lblNhanThoiGian = TaoNhan("Thời gian lưu trú:", topY);
            _lblThoiGianVal = TaoGiaTri(topY);

            topY += stepY;
            var lblNhanSoKhach = TaoNhan("Số khách:", topY);
            _lblSoKhachVal = TaoGiaTri(topY);

            topY += stepY;
            var lblNhanPhong = TaoNhan("Phòng đặt:", topY);
            _lblDanhSachPhongVal = new Label
            {
                Text = "Đang tải...",
                Location = new Point(150, topY),
                Size = new Size(270, 48),
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.PrimaryActive
            };

            topY += stepY + 12;
            var lblNhanTrangThai = TaoNhan("Trạng thái:", topY);
            _lblTrangThaiVal = new Label
            {
                Text = "ĐÃ XÁC NHẬN",
                Location = new Point(150, topY - 2),
                AutoSize = false,
                Size = new Size(130, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = AppTheme.FontSmallBold,
                BackColor = AppTheme.StatusSuccessBg,
                ForeColor = AppTheme.StatusSuccessText
            };

            topY += stepY - 4;
            var lblNhanEmail = TaoNhan("Email xác nhận:", topY);
            _lblEmailStatusVal = new Label
            {
                Text = "Đang kiểm tra gửi...",
                Location = new Point(150, topY),
                Size = new Size(270, 38),
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary
            };

            pnlThongTin.Controls.AddRange(new Control[]
            {
                lblTieuDeThongTin,
                lblNhanMa, _lblMaBookingVal,
                lblNhanKH, _lblKhachHangVal,
                lblNhanSdt, _lblSdtVal,
                lblNhanThoiGian, _lblThoiGianVal,
                lblNhanSoKhach, _lblSoKhachVal,
                lblNhanPhong, _lblDanhSachPhongVal,
                lblNhanTrangThai, _lblTrangThaiVal,
                lblNhanEmail, _lblEmailStatusVal
            });

            // --- Card phải: Mã QR Code (Width ~ 300) ---
            var pnlQr = new Panel
            {
                Location = new Point(480, 16),
                Size = new Size(300, 390),
                BackColor = AppTheme.Surface,
                Padding = new Padding(16)
            };
            AppTheme.ApplyCardPanel(pnlQr);

            var lblTieuDeQr = new Label
            {
                Text = "MÃ QR TRA CỨU KIOSK",
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 16),
                AutoSize = true
            };

            _picQrCode = new PictureBox
            {
                Location = new Point(40, 50),
                Size = new Size(220, 220),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            _lblQrHuongDan = new Label
            {
                Text = "Quét mã này tại Kiosk tự phục vụ để mở nhanh thông tin check-in / hóa đơn.",
                Location = new Point(16, 278),
                Size = new Size(268, 38),
                TextAlign = ContentAlignment.TopCenter,
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextSecondary
            };

            _btnLuuQr = new Button
            {
                Text = "💾 Lưu ảnh QR",
                Location = new Point(20, 328),
                Size = new Size(125, 36)
            };
            AppTheme.ApplySecondaryButton(_btnLuuQr);

            _btnSaoChepMa = new Button
            {
                Text = "📋 Copy mã",
                Location = new Point(155, 328),
                Size = new Size(125, 36)
            };
            AppTheme.ApplySecondaryButton(_btnSaoChepMa);

            pnlQr.Controls.AddRange(new Control[]
            {
                lblTieuDeQr,
                _picQrCode,
                _lblQrHuongDan,
                _btnLuuQr,
                _btnSaoChepMa
            });

            pnlBody.Controls.AddRange(new Control[] { pnlThongTin, pnlQr });

            // ================= 3. Bottom Panel =================
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(20, 10, 20, 10)
            };

            _btnGuiEmail = new Button
            {
                Text = "✉ Gửi lại email xác nhận",
                Size = new Size(200, 40),
                Location = new Point(400, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnGuiEmail);

            _btnDong = new Button
            {
                Text = "✓ Đóng & Hoàn tất",
                Size = new Size(160, 40),
                Location = new Point(620, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplyPrimaryButton(_btnDong);

            pnlBottom.Controls.AddRange(new Control[] { _btnGuiEmail, _btnDong });

            // ================= Assemble Form =================
            Controls.AddRange(new Control[]
            {
                pnlBody,
                pnlHeader,
                pnlBottom
            });
        }
    }
}
