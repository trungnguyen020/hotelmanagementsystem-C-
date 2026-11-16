using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormBaoCaoDoanhThu
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

        // Bộ lọc
        private DateTimePicker _dtpTuNgay = null!;
        private DateTimePicker _dtpDenNgay = null!;
        private ComboBox _cboLoaiPhong = null!;
        private Button _btnXemBaoCao = null!;
        private Button _btnXuatExcel = null!;

        // Thẻ KPI Cards
        private Label _lblTongDoanhThuVal = null!;
        private Label _lblTienPhongVal = null!;
        private Label _lblTienDichVuVal = null!;
        private Label _lblTyLeLapDayVal = null!;
        private Label _lblTongBookingVal = null!;

        // Bảng dữ liệu
        private TabControl _tabDuLieu = null!;
        private DataGridView _dgvHoaDon = null!;
        private DataGridView _dgvLoaiPhong = null!;

        // Trạng thái và điều khiển
        private Label _lblTrangThai = null!;
        private ProgressBar _prgTienTrinh = null!;
        private Button _btnHuyThaoTac = null!;
        private Button _btnDong = null!;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "BÁO CÁO DOANH THU & HIỆU SUẤT HOẠT ĐỘNG PHÒNG";
            Size = new Size(1100, 720);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            // 1. Panel Bộ lọc trên cùng (Top Panel)
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = AppTheme.Surface,
                Padding = new Padding(15, 6, 15, 6)
            };

            var grpBoLoc = new GroupBox
            {
                Text = "Bộ lọc báo cáo",
                Dock = DockStyle.Fill,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Padding = new Padding(8, 4, 8, 4)
            };

            var flpFilter = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(4, 3, 4, 2)
            };

            // Từ ngày
            var lblTuNgay = new Label
            {
                Text = "Từ ngày:",
                AutoSize = true,
                Margin = new Padding(2, 6, 2, 0),
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary
            };
            _dtpTuNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 110,
                Margin = new Padding(0, 3, 10, 0)
            };
            AppTheme.ApplyDateTimePicker(_dtpTuNgay);

            // Đến ngày
            var lblDenNgay = new Label
            {
                Text = "Đến ngày:",
                AutoSize = true,
                Margin = new Padding(2, 6, 2, 0),
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary
            };
            _dtpDenNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 110,
                Margin = new Padding(0, 3, 10, 0)
            };
            AppTheme.ApplyDateTimePicker(_dtpDenNgay);

            // Loại phòng
            var lblLoaiPhong = new Label
            {
                Text = "Loại phòng:",
                AutoSize = true,
                Margin = new Padding(2, 6, 2, 0),
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary
            };
            _cboLoaiPhong = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 185,
                Margin = new Padding(0, 3, 15, 0)
            };
            AppTheme.ApplyComboBox(_cboLoaiPhong);

            // Nút Xem báo cáo
            _btnXemBaoCao = new Button
            {
                Text = "🔍 Xem báo cáo",
                Width = 135,
                Height = 32,
                Margin = new Padding(0, 1, 10, 0)
            };
            AppTheme.ApplyPrimaryButton(_btnXemBaoCao);

            // Nút Xuất Excel
            _btnXuatExcel = new Button
            {
                Text = "📊 Xuất Excel",
                Width = 135,
                Height = 32,
                Margin = new Padding(0, 1, 0, 0),
                Enabled = false
            };
            AppTheme.ApplySecondaryButton(_btnXuatExcel);

            flpFilter.Controls.AddRange(new Control[]
            {
                lblTuNgay, _dtpTuNgay,
                lblDenNgay, _dtpDenNgay,
                lblLoaiPhong, _cboLoaiPhong,
                _btnXemBaoCao, _btnXuatExcel
            });

            grpBoLoc.Controls.Add(flpFilter);
            pnlTop.Controls.Add(grpBoLoc);

            // 2. Panel KPI Cards tổng quan
            var pnlKpis = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 85,
                ColumnCount = 5,
                RowCount = 1,
                Padding = new Padding(15, 8, 15, 6),
                BackColor = Color.Transparent
            };

            for (int i = 0; i < 5; i++)
            {
                pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
            }

            Panel CreateCard(string title, out Label valLabel, Color titleColor, Color valColor)
            {
                var card = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = AppTheme.Surface,
                    Margin = new Padding(5, 0, 5, 0),
                    Padding = new Padding(10, 8, 10, 8)
                };

                var lblT = new Label
                {
                    Text = title,
                    Dock = DockStyle.Top,
                    Height = 20,
                    Font = AppTheme.FontSmallBold,
                    ForeColor = titleColor
                };

                valLabel = new Label
                {
                    Text = "0 đ",
                    Dock = DockStyle.Fill,
                    Font = AppTheme.FontHeader,
                    ForeColor = valColor,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                card.Controls.Add(valLabel);
                card.Controls.Add(lblT);
                return card;
            }

            var card1 = CreateCard("TỔNG DOANH THU", out _lblTongDoanhThuVal, AppTheme.TextMuted, AppTheme.Primary);
            var card2 = CreateCard("TIỀN PHÒNG", out _lblTienPhongVal, AppTheme.TextMuted, AppTheme.PrimaryActive);
            var card3 = CreateCard("TIỀN DỊCH VỤ", out _lblTienDichVuVal, AppTheme.TextMuted, AppTheme.StatusSuccessText);
            var card4 = CreateCard("TỶ LỆ LẤP ĐẦY", out _lblTyLeLapDayVal, AppTheme.TextMuted, AppTheme.AccentGold);
            var card5 = CreateCard("TỔNG BOOKING", out _lblTongBookingVal, AppTheme.TextMuted, AppTheme.TextPrimary);

            pnlKpis.Controls.Add(card1, 0, 0);
            pnlKpis.Controls.Add(card2, 1, 0);
            pnlKpis.Controls.Add(card3, 2, 0);
            pnlKpis.Controls.Add(card4, 3, 0);
            pnlKpis.Controls.Add(card5, 4, 0);

            // 3. Panel Bottom trạng thái & nút đóng
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                BackColor = AppTheme.Surface,
                Padding = new Padding(15, 8, 15, 8)
            };

            _lblTrangThai = new Label
            {
                Text = "Sẵn sàng tra cứu báo cáo.",
                Dock = DockStyle.Left,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontCaption
            };

            _btnDong = new Button
            {
                Text = "Đóng",
                Width = 95,
                Dock = DockStyle.Right
            };
            AppTheme.ApplySecondaryButton(_btnDong);
            _btnDong.Click += (_, _) => Close();

            _btnHuyThaoTac = new Button
            {
                Text = "⛔  Hủy",
                Width = 100,
                Dock = DockStyle.Right,
                Visible = false
            };
            AppTheme.ApplyDangerButton(_btnHuyThaoTac);

            _prgTienTrinh = new ProgressBar
            {
                Width = 160,
                Dock = DockStyle.Right,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Visible = false
            };

            pnlBottom.Controls.Add(_lblTrangThai);
            pnlBottom.Controls.Add(_prgTienTrinh);
            pnlBottom.Controls.Add(_btnHuyThaoTac);
            pnlBottom.Controls.Add(_btnDong);

            // 4. TabControl chứa danh sách chi tiết (Center Area)
            var pnlCenter = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 6, 15, 8),
                BackColor = AppTheme.Background
            };

            _tabDuLieu = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.FontBody
            };

            // Tab 1: Danh sách hóa đơn chi tiết
            var tabHoaDon = new TabPage("📋 Danh sách hóa đơn đã thanh toán")
            {
                BackColor = AppTheme.Surface,
                Padding = new Padding(8)
            };
            _dgvHoaDon = CreateStandardDataGridView();
            tabHoaDon.Controls.Add(_dgvHoaDon);

            // Tab 2: Phân tích theo loại phòng
            var tabLoaiPhong = new TabPage("🏢 Phân tích doanh thu theo loại phòng")
            {
                BackColor = AppTheme.Surface,
                Padding = new Padding(8)
            };
            _dgvLoaiPhong = CreateStandardDataGridView();
            tabLoaiPhong.Controls.Add(_dgvLoaiPhong);

            _tabDuLieu.TabPages.Add(tabHoaDon);
            _tabDuLieu.TabPages.Add(tabLoaiPhong);
            pnlCenter.Controls.Add(_tabDuLieu);

            // Ghép toàn bộ vào Form
            Controls.Add(pnlCenter);
            Controls.Add(pnlKpis);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
        }

        private static DataGridView CreateStandardDataGridView()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
            };
            AppTheme.ApplyDataGridView(dgv);
            return dgv;
        }

        #endregion
    }
}
