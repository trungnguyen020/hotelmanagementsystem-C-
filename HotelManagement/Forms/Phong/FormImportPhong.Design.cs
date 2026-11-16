using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormImportPhong
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

        // Header controls
        private Label _lblTieuDe = null!;
        private Label _lblPhuDe = null!;
        private Button _btnTaiMau = null!;

        // File selection controls
        private TextBox _txtDuongDan = null!;
        private Button _btnChonFile = null!;
        private Button _btnKiemTra = null!;

        // Summary KPI cards
        private Label _lblTongSoDong = null!;
        private Label _lblSoDongHopLe = null!;
        private Label _lblSoDongLoi = null!;

        // Data grid
        private DataGridView _dgvKetQua = null!;
        private CheckBox _chkChiHienThiLoi = null!;

        // Bottom actions
        private Label _lblTrangThaiXuLy = null!;
        private ProgressBar _prgTienTrinh = null!;
        private Button _btnHuyThaoTac = null!;
        private Button _btnImport = null!;
        private Button _btnDong = null!;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "IMPORT DANH SÁCH PHÒNG TỪ EXCEL";
            Size = new Size(1100, 720);
            MinimumSize = new Size(900, 580);
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            // ========================================================
            // 1. TOP HEADER BANNER (Dock = Top)
            // ========================================================
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 76,
                BackColor = AppTheme.Surface,
                Padding = new Padding(18, 12, 18, 12)
            };

            var pnlHeaderBorder = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = AppTheme.BorderLight
            };
            pnlHeader.Controls.Add(pnlHeaderBorder);

            // Nút tải file mẫu cố định bên phải header
            _btnTaiMau = new Button
            {
                Text = "📥  Tải file mẫu Excel",
                Dock = DockStyle.Right,
                Width = 175
            };
            AppTheme.ApplySecondaryButton(_btnTaiMau);
            pnlHeader.Controls.Add(_btnTaiMau);

            // Container chứa tiêu đề bên trái
            var pnlHeaderTitles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            _lblTieuDe = new Label
            {
                Text = "IMPORT DANH SÁCH PHÒNG TỪ EXCEL",
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 28
            };
            pnlHeaderTitles.Controls.Add(_lblTieuDe);

            _lblPhuDe = new Label
            {
                Text = "Tự động kiểm tra tính hợp lệ từng dòng — Gom lỗi chi tiết, không dừng ở lỗi đầu tiên",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Top,
                Height = 22
            };
            pnlHeaderTitles.Controls.Add(_lblPhuDe);

            pnlHeader.Controls.Add(pnlHeaderTitles);
            Controls.Add(pnlHeader);

            // ========================================================
            // 2. PANEL ĐIỀU KHIỂN DƯỚI ĐÁY (Dock = Bottom)
            // ========================================================
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = AppTheme.Surface,
                Padding = new Padding(18, 12, 18, 12)
            };

            var pnlBottomBorder = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = AppTheme.BorderLight
            };
            pnlBottom.Controls.Add(pnlBottomBorder);

            // Nhóm nút bấm cố định bên phải đáy
            var flpBottomButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            _btnDong = new Button
            {
                Text = "Đóng",
                Size = new Size(95, 38),
                Margin = new Padding(8, 0, 0, 0)
            };
            AppTheme.ApplySecondaryButton(_btnDong);
            flpBottomButtons.Controls.Add(_btnDong);

            _btnHuyThaoTac = new Button
            {
                Text = "⛔  Hủy thao tác",
                Size = new Size(130, 38),
                Visible = false,
                Margin = new Padding(8, 0, 0, 0)
            };
            AppTheme.ApplyDangerButton(_btnHuyThaoTac);
            flpBottomButtons.Controls.Add(_btnHuyThaoTac);

            _btnImport = new Button
            {
                Text = "⚡  Tiến hành Import vào hệ thống",
                Size = new Size(260, 38),
                Enabled = false,
                Margin = new Padding(0)
            };
            AppTheme.ApplyPrimaryButton(_btnImport);
            flpBottomButtons.Controls.Add(_btnImport);

            pnlBottom.Controls.Add(flpBottomButtons);

            // Nhóm trạng thái và lọc bên trái đáy
            var flpBottomStatus = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 6, 0, 0)
            };

            _chkChiHienThiLoi = new CheckBox
            {
                Text = "Chỉ hiển thị các dòng bị lỗi",
                AutoSize = true,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 3, 15, 0)
            };
            AppTheme.ApplyCheckBox(_chkChiHienThiLoi);
            _chkChiHienThiLoi.ForeColor = AppTheme.StatusDangerText;
            flpBottomStatus.Controls.Add(_chkChiHienThiLoi);

            _prgTienTrinh = new ProgressBar
            {
                Size = new Size(160, 22),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Visible = false,
                Margin = new Padding(0, 3, 10, 0)
            };
            flpBottomStatus.Controls.Add(_prgTienTrinh);

            _lblTrangThaiXuLy = new Label
            {
                Text = "Chưa có dữ liệu nào được nạp.",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0)
            };
            flpBottomStatus.Controls.Add(_lblTrangThaiXuLy);

            pnlBottom.Controls.Add(flpBottomStatus);
            Controls.Add(pnlBottom);

            // ========================================================
            // 3. PANEL CHỌN FILE VÀ KPI THỐNG KÊ (Dock = Top kế tiếp)
            // ========================================================
            var pnlTopControls = new Panel
            {
                Dock = DockStyle.Top,
                Height = 138,
                Padding = new Padding(18, 10, 18, 8),
                BackColor = AppTheme.Background
            };

            var tlpTop = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tlpTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63f));
            tlpTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37f));
            tlpTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // Khối 1: Chọn file
            var grpChonFile = new GroupBox
            {
                Text = "Chọn file dữ liệu Excel",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 8, 12, 8),
                Margin = new Padding(0, 0, 10, 0)
            };

            var lblDuongDan = new Label
            {
                Text = "Đường dẫn file (*.xlsx, *.xls):",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Top,
                Height = 20
            };
            grpChonFile.Controls.Add(lblDuongDan);

            // Hàng nhập file: TextBox + Nút Chọn file + Nút Kiểm tra
            var pnlFileRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(0, 2, 0, 2)
            };

            _btnKiemTra = new Button
            {
                Text = "🔍 Kiểm tra",
                Dock = DockStyle.Right,
                Width = 92,
                Enabled = false
            };
            AppTheme.ApplySecondaryButton(_btnKiemTra);

            var pnlSpacing = new Panel { Dock = DockStyle.Right, Width = 6 };

            _btnChonFile = new Button
            {
                Text = "📂 Chọn file Excel...",
                Dock = DockStyle.Right,
                Width = 145
            };
            AppTheme.ApplyPrimaryButton(_btnChonFile);

            _txtDuongDan = new TextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                PlaceholderText = "Nhấp 'Chọn file Excel...' để nạp dữ liệu..."
            };
            AppTheme.ApplyTextBox(_txtDuongDan);
            _txtDuongDan.BackColor = AppTheme.SurfaceSecondary;

            pnlFileRow.Controls.Add(_txtDuongDan);
            pnlFileRow.Controls.Add(_btnChonFile);
            pnlFileRow.Controls.Add(pnlSpacing);
            pnlFileRow.Controls.Add(_btnKiemTra);
            grpChonFile.Controls.Add(pnlFileRow);

            var lblGhiChuFile = new Label
            {
                Text = "💡 Hệ thống tự động nhận diện cột: Số phòng, Loại phòng, Trạng thái.",
                Font = AppTheme.FontCaption,
                ForeColor = AppTheme.TextMuted,
                Dock = DockStyle.Bottom,
                Height = 22
            };
            grpChonFile.Controls.Add(lblGhiChuFile);

            tlpTop.Controls.Add(grpChonFile, 0, 0);

            // Khối 2: KPI Thống kê
            var grpThongKe = new GroupBox
            {
                Text = "Kết quả kiểm tra dữ liệu",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 8, 10, 8),
                Margin = new Padding(0)
            };

            var tlpBadges = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tlpBadges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tlpBadges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tlpBadges.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
            tlpBadges.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var pnlBadgeTong = TaoBadgeThongKe("TỔNG DÒNG", "0", AppTheme.StatusNeutralBg, AppTheme.StatusNeutralText, out _lblTongSoDong);
            tlpBadges.Controls.Add(pnlBadgeTong, 0, 0);

            var pnlBadgeHopLe = TaoBadgeThongKe("HỢP LỆ", "0", AppTheme.StatusSuccessBg, AppTheme.StatusSuccessText, out _lblSoDongHopLe);
            tlpBadges.Controls.Add(pnlBadgeHopLe, 1, 0);

            var pnlBadgeLoi = TaoBadgeThongKe("BỊ LỖI", "0", AppTheme.StatusDangerBg, AppTheme.StatusDangerText, out _lblSoDongLoi);
            tlpBadges.Controls.Add(pnlBadgeLoi, 2, 0);

            grpThongKe.Controls.Add(tlpBadges);
            tlpTop.Controls.Add(grpThongKe, 1, 0);

            pnlTopControls.Controls.Add(tlpTop);
            Controls.Add(pnlTopControls);

            // ========================================================
            // 4. CENTER GRID VIEW (Dock = Fill)
            // ========================================================
            var pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 6, 18, 10),
                BackColor = AppTheme.Background
            };

            _dgvKetQua = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoGenerateColumns = false
            };
            AppTheme.ApplyDataGridView(_dgvKetQua);
            _dgvKetQua.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvKetQua.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Định nghĩa cột
            var colDong = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DongThuTu",
                HeaderText = "Dòng",
                Width = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            colDong.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _dgvKetQua.Columns.Add(colDong);

            var colSoPhong = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoPhong",
                HeaderText = "Số phòng",
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            colSoPhong.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _dgvKetQua.Columns.Add(colSoPhong);

            var colLoaiPhong = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenLoaiPhong",
                HeaderText = "Loại phòng",
                Width = 160
            };
            _dgvKetQua.Columns.Add(colLoaiPhong);

            var colTrangThai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            colTrangThai.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _dgvKetQua.Columns.Add(colTrangThai);

            var colKetQua = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "KetQuaKiemTra",
                HeaderText = "Kết quả",
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            colKetQua.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _dgvKetQua.Columns.Add(colKetQua);

            var colChiTietLoi = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ChiTietLoi",
                HeaderText = "Chi tiết lỗi kiểm tra",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 280
            };
            _dgvKetQua.Columns.Add(colChiTietLoi);

            pnlGrid.Controls.Add(_dgvKetQua);
            Controls.Add(pnlGrid);

            // Thứ tự hiển thị Dock
            pnlHeader.BringToFront();
            pnlTopControls.BringToFront();
            pnlBottom.BringToFront();
            pnlGrid.BringToFront();
        }

        private static Panel TaoBadgeThongKe(string tieuDe, string giaTriKhoiTao, Color mauNen, Color mauChu, out Label lblGiaTri)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = mauNen,
                Padding = new Padding(4, 6, 4, 6),
                Margin = new Padding(4)
            };

            var lblTieuDe = new Label
            {
                Text = tieuDe,
                Dock = DockStyle.Top,
                Height = 18,
                Font = AppTheme.FontSmallBold,
                ForeColor = mauChu,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnl.Controls.Add(lblTieuDe);

            lblGiaTri = new Label
            {
                Text = giaTriKhoiTao,
                Dock = DockStyle.Fill,
                Font = AppTheme.FontHeader,
                ForeColor = mauChu,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnl.Controls.Add(lblGiaTri);

            return pnl;
        }

        #endregion
    }
}
