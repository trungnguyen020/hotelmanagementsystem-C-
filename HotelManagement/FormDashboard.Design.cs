using System;
using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace HotelManagement.Forms
{
    partial class FormDashboard
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

        #region Controls

        // Top Filter Bar
        private Panel _pnlTopBar = null!;
        private Label _lblTitle = null!;
        private Label _lblSubTitle = null!;
        private DateTimePicker _dtpTuNgay = null!;
        private DateTimePicker _dtpDenNgay = null!;
        private Button _btnHomNay = null!;
        private Button _btn7Ngay = null!;
        private Button _btn30Ngay = null!;
        private Button _btnThangNay = null!;
        private Button _btnLoc = null!;
        private Button _btnLamMoi = null!;

        // KPI Summary Cards
        private Label _lblValDoanhThu = null!;
        private Label _lblSubDoanhThu = null!;
        private Label _lblValLapDay = null!;
        private Label _lblSubLapDay = null!;
        private Label _lblValLuotDat = null!;
        private Label _lblSubLuotDat = null!;
        private Label _lblValHienTrang = null!;
        private Label _lblSubHienTrang = null!;

        // 3 LiveCharts Controls
        private CartesianChart _chartDoanhThu = null!;
        private PieChart _chartTrangThaiPhong = null!;
        private CartesianChart _chartLapDay = null!;

        // Bảng tóm tắt thông số phụ
        private Label _lblSummaryTieuDe = null!;
        private Label _lblSummaryNoiDung = null!;

        #endregion

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "DASHBOARD BIỂU ĐỒ DOANH THU & HIỆU SUẤT PHÒNG";
            Size = new Size(1300, 850);
            MinimumSize = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            BackColor = AppTheme.Background;
            Font = AppTheme.FontBody;
            ForeColor = AppTheme.TextPrimary;
            DoubleBuffered = true;

            // ================= 1. Header & Bộ lọc Khoảng ngày =================
            _pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(20, 10, 20, 10)
            };

            _lblTitle = new Label
            {
                Text = "📊 DASHBOARD QUẢN TRỊ & THỐNG KÊ DOANH THU",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                AutoSize = true,
                Location = new Point(20, 10)
            };

            _lblSubTitle = new Label
            {
                Text = "Phân tích trực quan doanh thu, tỷ lệ lấp đầy và hiện trạng phòng (DCKLTTQ-84 - LiveCharts2).",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(20, 42)
            };

            // FlowLayoutPanel chứa các nút lọc ngày
            var flpFilter = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 12, 0, 0),
                WrapContents = false
            };

            var lblTu = new Label
            {
                Text = "Từ ngày:",
                AutoSize = true,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Margin = new Padding(4, 9, 4, 4)
            };

            _dtpTuNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 125,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 4, 8, 4)
            };

            var lblDen = new Label
            {
                Text = "➔",
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Margin = new Padding(2, 8, 6, 4)
            };

            _dtpDenNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 125,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 4, 12, 4)
            };

            _btnHomNay = new Button { Text = "Hôm nay", Size = new Size(80, 32), Margin = new Padding(0, 4, 6, 4) };
            AppTheme.ApplySecondaryButton(_btnHomNay);

            _btn7Ngay = new Button { Text = "7 ngày", Size = new Size(75, 32), Margin = new Padding(0, 4, 6, 4) };
            AppTheme.ApplySecondaryButton(_btn7Ngay);

            _btn30Ngay = new Button { Text = "30 ngày", Size = new Size(80, 32), Margin = new Padding(0, 4, 6, 4) };
            AppTheme.ApplySecondaryButton(_btn30Ngay);

            _btnThangNay = new Button { Text = "Tháng này", Size = new Size(90, 32), Margin = new Padding(0, 4, 10, 4) };
            AppTheme.ApplySecondaryButton(_btnThangNay);

            _btnLoc = new Button { Text = "🔍 Lọc", Size = new Size(85, 32), Margin = new Padding(0, 4, 6, 4) };
            AppTheme.ApplyPrimaryButton(_btnLoc);

            _btnLamMoi = new Button { Text = "🔄 Tải lại", Size = new Size(85, 32), Margin = new Padding(0, 4, 4, 4) };
            AppTheme.ApplySecondaryButton(_btnLamMoi);

            flpFilter.Controls.AddRange(new Control[]
            {
                lblTu, _dtpTuNgay, lblDen, _dtpDenNgay,
                _btnHomNay, _btn7Ngay, _btn30Ngay, _btnThangNay,
                _btnLoc, _btnLamMoi
            });

            _pnlTopBar.Controls.AddRange(new Control[] { flpFilter, _lblTitle, _lblSubTitle });

            // ================= 2. KPI Summary Cards =================
            var pnlKpiContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = AppTheme.Background,
                Padding = new Padding(20, 10, 20, 8)
            };

            var tlpKpi = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Card 1: Doanh thu
            var cardDoanhThu = TaoCardKpi("💰 TỔNG DOANH THU KỲ CHỌN", out _lblValDoanhThu, out _lblSubDoanhThu, AppTheme.Primary);
            tlpKpi.Controls.Add(cardDoanhThu, 0, 0);

            // Card 2: Lấp đầy
            var cardLapDay = TaoCardKpi("🏨 TỶ LỆ LẤP ĐẦY TRUNG BÌNH", out _lblValLapDay, out _lblSubLapDay, AppTheme.StatusSuccessText);
            tlpKpi.Controls.Add(cardLapDay, 1, 0);

            // Card 3: Lượt đặt
            var cardLuotDat = TaoCardKpi("📅 TỔNG LƯỢT ĐẶT PHÒNG", out _lblValLuotDat, out _lblSubLuotDat, AppTheme.AccentGold);
            tlpKpi.Controls.Add(cardLuotDat, 2, 0);

            // Card 4: Hiện trạng phòng
            var cardHienTrang = TaoCardKpi("🛏️ PHÒNG ĐANG SỬ DỤNG / TỔNG", out _lblValHienTrang, out _lblSubHienTrang, AppTheme.TextPrimary);
            tlpKpi.Controls.Add(cardHienTrang, 3, 0);

            pnlKpiContainer.Controls.Add(tlpKpi);

            // ================= 3. Main Chart Matrix (TableLayoutPanel 2x2) =================
            var pnlMainCharts = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 0, 20, 14),
                BackColor = AppTheme.Background
            };

            var tlpCharts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            tlpCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63F)); // Bên trái rộng hơn cho chuỗi thời gian
            tlpCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37F)); // Bên phải cho cơ cấu tròn & tóm tắt
            tlpCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 54F));    // Biểu đồ cột & tròn
            tlpCharts.RowStyles.Add(new RowStyle(SizeType.Percent, 46F));    // Biểu đồ đường & tóm tắt

            // --- 3.1 [Hàng 0, Cột 0]: Biểu đồ Cột - Doanh thu theo ngày ---
            var pnlCardColChart = TaoPanelBaoBieuDo("📈 BIỂU ĐỒ CỘT: DOANH THU THEO NGÀY (TIỀN PHÒNG & DỊCH VỤ)", out Panel bodyCol);
            _chartDoanhThu = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            bodyCol.Controls.Add(_chartDoanhThu);
            tlpCharts.Controls.Add(pnlCardColChart, 0, 0);

            // --- 3.2 [Hàng 0, Cột 1]: Biểu đồ Tròn - Cơ cấu trạng thái phòng ---
            var pnlCardPieChart = TaoPanelBaoBieuDo("🎯 BIỂU ĐỒ TRÒN: CƠ CẤU TRẠNG THÁI PHÒNG HIỆN TẠI", out Panel bodyPie);
            _chartTrangThaiPhong = new PieChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            bodyPie.Controls.Add(_chartTrangThaiPhong);
            tlpCharts.Controls.Add(pnlCardPieChart, 1, 0);

            // --- 3.3 [Hàng 1, Cột 0]: Biểu đồ Đường - Xu hướng tỷ lệ lấp đầy (%) ---
            var pnlCardLineChart = TaoPanelBaoBieuDo("📉 BIỂU ĐỒ ĐƯỜNG: XU HƯỚNG TỶ LỆ LẤP ĐẦY PHÒNG (%) THEO NGÀY", out Panel bodyLine);
            _chartLapDay = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            bodyLine.Controls.Add(_chartLapDay);
            tlpCharts.Controls.Add(pnlCardLineChart, 0, 1);

            // --- 3.4 [Hàng 1, Cột 1]: Panel Thống kê tóm tắt & Khuyến nghị ---
            var pnlCardSummary = TaoPanelBaoBieuDo("💡 THÔNG SỐ HIỆU SUẤT & TỔNG KẾT", out Panel bodySummary);
            _lblSummaryTieuDe = new Label
            {
                Text = "Tổng hợp hiệu suất kinh doanh trong kỳ:",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 28
            };
            _lblSummaryNoiDung = new Label
            {
                Text = "Đang tải dữ liệu...",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Fill
            };
            bodySummary.Controls.Add(_lblSummaryNoiDung);
            bodySummary.Controls.Add(_lblSummaryTieuDe);
            tlpCharts.Controls.Add(pnlCardSummary, 1, 1);

            pnlMainCharts.Controls.Add(tlpCharts);

            // ================= Ráp toàn bộ vào Form =================
            Controls.AddRange(new Control[]
            {
                pnlMainCharts,
                pnlKpiContainer,
                _pnlTopBar
            });
        }

        #region Helper Dựng Giao Diện

        private Panel TaoCardKpi(string tieuDe, out Label lblGiaTri, out Label lblPhu, Color mauGiaTri)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(14, 10, 14, 8),
                Margin = new Padding(6)
            };
            AppTheme.ApplyCardPanel(pnl);

            var lblTieuDe = new Label
            {
                Text = tieuDe,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Top,
                Height = 20
            };

            lblGiaTri = new Label
            {
                Text = "0",
                Font = AppTheme.FontHeader,
                ForeColor = mauGiaTri,
                Dock = DockStyle.Top,
                Height = 32
            };

            lblPhu = new Label
            {
                Text = "...",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                Dock = DockStyle.Fill
            };

            pnl.Controls.Add(lblPhu);
            pnl.Controls.Add(lblGiaTri);
            pnl.Controls.Add(lblTieuDe);

            return pnl;
        }

        private Panel TaoPanelBaoBieuDo(string tieuDe, out Panel bodyPanel)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 10, 12, 10),
                Margin = new Padding(6)
            };
            AppTheme.ApplyCardPanel(card);

            var lblHeader = new Label
            {
                Text = tieuDe,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 26
            };

            bodyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(4)
            };

            card.Controls.Add(bodyPanel);
            card.Controls.Add(lblHeader);

            return card;
        }

        #endregion
    }
}
