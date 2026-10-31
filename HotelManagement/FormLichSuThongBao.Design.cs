using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormLichSuThongBao
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

        // Header
        private Label _lblHeaderTitle = null!;
        private Label _lblHeaderSub = null!;
        private Label _lblBadgeTong = null!;
        private Label _lblBadgeThanhCong = null!;
        private Label _lblBadgeThatBai = null!;

        // Bộ lọc
        private ComboBox _cboLoaiThongBao = null!;
        private ComboBox _cboTrangThai = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnLoc = null!;
        private Button _btnLamMoi = null!;

        // Danh sách
        private DataGridView _dgvLichSu = null!;

        // Chi tiết bản ghi chọn
        private Label _lblChiTietTieuDe = null!;
        private TextBox _txtChiTietLoi = null!;
        private Button _btnDong = null!;

        #endregion

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "LỊCH SỬ GỬI THÔNG BÁO EMAIL";
            Size = new Size(1220, 750);
            MinimumSize = new Size(1080, 640);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.Background;
            Font = AppTheme.FontBody;
            ForeColor = AppTheme.TextPrimary;
            DoubleBuffered = true;

            // ================= 1. Header Panel =================
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(24, 12, 24, 12)
            };

            // 1.1 Thống kê nhanh bên phải Header (Dock Right để không bao giờ bị tràn/che)
            var pnlStats = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 18, 16, 0),
                WrapContents = false
            };

            _lblBadgeTong = new Label
            {
                Text = "Tổng: 0",
                Font = AppTheme.FontSmallBold,
                BackColor = AppTheme.Surface,
                ForeColor = AppTheme.TextPrimary,
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 0)
            };

            _lblBadgeThanhCong = new Label
            {
                Text = "✓ Thành công: 0",
                Font = AppTheme.FontSmallBold,
                BackColor = AppTheme.StatusSuccessBg,
                ForeColor = AppTheme.StatusSuccessText,
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 0)
            };

            _lblBadgeThatBai = new Label
            {
                Text = "⚠ Thất bại: 0",
                Font = AppTheme.FontSmallBold,
                BackColor = AppTheme.StatusDangerBg,
                ForeColor = AppTheme.StatusDangerText,
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 0)
            };

            pnlStats.Controls.AddRange(new Control[] { _lblBadgeTong, _lblBadgeThanhCong, _lblBadgeThatBai });

            // 1.2 Tiêu đề bên trái Header
            _lblHeaderTitle = new Label
            {
                Text = "✉ LỊCH SỬ GỬI THÔNG BÁO EMAIL",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                AutoSize = true,
                Location = new Point(24, 14)
            };

            _lblHeaderSub = new Label
            {
                Text = "Theo dõi và tra cứu toàn bộ lịch sử gửi email xác nhận đặt phòng và hóa đơn PDF.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(24, 48)
            };

            pnlHeader.Controls.AddRange(new Control[] { pnlStats, _lblHeaderTitle, _lblHeaderSub });

            // ================= 2. Main Container =================
            var pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 14, 24, 14)
            };

            // --- 2.1 Bộ lọc Dùng FlowLayoutPanel để chống đè chữ/control trên mọi độ phân giải DPI ---
            var pnlFilter = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = AppTheme.Surface,
                Padding = new Padding(14, 12, 14, 12),
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };
            AppTheme.ApplyCardPanel(pnlFilter);

            var lblLoai = new Label
            {
                Text = "Loại thông báo:",
                AutoSize = true,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Margin = new Padding(4, 9, 6, 4)
            };

            _cboLoaiThongBao = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 175,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 4, 18, 4)
            };
            _cboLoaiThongBao.Items.AddRange(new object[]
            {
                "-- Tất cả loại --",
                "XacNhanBooking",
                "HoaDon"
            });
            _cboLoaiThongBao.SelectedIndex = 0;

            var lblTrangThai = new Label
            {
                Text = "Trạng thái:",
                AutoSize = true,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Margin = new Padding(4, 9, 6, 4)
            };

            _cboTrangThai = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 145,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 4, 18, 4)
            };
            _cboTrangThai.Items.AddRange(new object[]
            {
                "-- Tất cả --",
                "Thành công",
                "Thất bại"
            });
            _cboTrangThai.SelectedIndex = 0;

            var lblTimKiem = new Label
            {
                Text = "Tìm kiếm:",
                AutoSize = true,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Margin = new Padding(4, 9, 6, 4)
            };

            _txtTimKiem = new TextBox
            {
                Width = 230,
                Font = AppTheme.FontBody,
                PlaceholderText = "Mã booking hoặc email...",
                Margin = new Padding(0, 4, 16, 4)
            };

            _btnLoc = new Button
            {
                Text = "🔍 Lọc",
                Size = new Size(95, 34),
                Margin = new Padding(0, 3, 10, 4)
            };
            AppTheme.ApplyPrimaryButton(_btnLoc);

            _btnLamMoi = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(115, 34),
                Margin = new Padding(0, 3, 8, 4)
            };
            AppTheme.ApplySecondaryButton(_btnLamMoi);

            pnlFilter.Controls.AddRange(new Control[]
            {
                lblLoai, _cboLoaiThongBao,
                lblTrangThai, _cboTrangThai,
                lblTimKiem, _txtTimKiem,
                _btnLoc, _btnLamMoi
            });

            // Spacer giữa Filter và Grid
            var pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 14, BackColor = Color.Transparent };

            // --- 2.2 DataGridView Card ---
            var pnlGridCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(10)
            };
            AppTheme.ApplyCardPanel(pnlGridCard);

            _dgvLichSu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            AppTheme.ApplyDataGridView(_dgvLichSu);

            // Cấu hình các cột hiển thị kèm MinimumWidth để không bao giờ bị cắt ngắn tiêu đề cột
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdLichsu", HeaderText = "#", FillWeight = 30, MinimumWidth = 45 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThoiGianGui", HeaderText = "Thời gian gửi", FillWeight = 95, MinimumWidth = 145 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "LoaiThongbao", HeaderText = "Loại thông báo", FillWeight = 85, MinimumWidth = 135 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaBooking", HeaderText = "Mã Booking", FillWeight = 75, MinimumWidth = 115 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "NguoiNhan", HeaderText = "Người nhận", FillWeight = 110, MinimumWidth = 170 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "TieuDe", HeaderText = "Tiêu đề", FillWeight = 130, MinimumWidth = 180 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 70, MinimumWidth = 105 });
            _dgvLichSu.Columns.Add(new DataGridViewTextBoxColumn { Name = "LyDoLoi", HeaderText = "Chi tiết lỗi", FillWeight = 120, MinimumWidth = 150 });

            pnlGridCard.Controls.Add(_dgvLichSu);

            // Thêm theo đúng thứ tự Dock: pnlGridCard (Fill) thêm trước, sau đó pnlSpacer (Top), rồi pnlFilter (Top)
            pnlMain.Controls.AddRange(new Control[] { pnlGridCard, pnlSpacer, pnlFilter });

            // ================= 3. Bottom Detail Panel =================
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(24, 10, 24, 10)
            };

            _lblChiTietTieuDe = new Label
            {
                Text = "Chi tiết thông báo / Lỗi:",
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(24, 10),
                AutoSize = true
            };

            _txtChiTietLoi = new TextBox
            {
                Location = new Point(24, 32),
                Size = new Size(980, 44),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                Font = AppTheme.FontSmall,
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(1020, 32),
                Size = new Size(130, 44),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnDong);

            pnlBottom.Controls.AddRange(new Control[] { _lblChiTietTieuDe, _txtChiTietLoi, _btnDong });

            // ================= Ghép Form =================
            Controls.AddRange(new Control[]
            {
                pnlMain,
                pnlHeader,
                pnlBottom
            });
        }
    }
}
