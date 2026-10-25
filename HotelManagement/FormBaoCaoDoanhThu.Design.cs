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
        private Button _btnDong = null!;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "BÁO CÁO DOANH THU & HIỆU SUẤT HOẠT ĐỘNG PHÒNG";
            Size = new Size(1100, 720);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            BackColor = Color.FromArgb(245, 247, 250);

            // 1. Panel Bộ lọc trên cùng (Top Panel)
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.White,
                Padding = new Padding(15, 6, 15, 6)
            };

            var grpBoLoc = new GroupBox
            {
                Text = "Bộ lọc báo cáo",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 59, 102),
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
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            _dtpTuNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 110,
                Margin = new Padding(0, 3, 10, 0),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            // Đến ngày
            var lblDenNgay = new Label
            {
                Text = "Đến ngày:",
                AutoSize = true,
                Margin = new Padding(2, 6, 2, 0),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            _dtpDenNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 110,
                Margin = new Padding(0, 3, 10, 0),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            // Loại phòng
            var lblLoaiPhong = new Label
            {
                Text = "Loại phòng:",
                AutoSize = true,
                Margin = new Padding(2, 6, 2, 0),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            _cboLoaiPhong = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 185,
                Margin = new Padding(0, 3, 15, 0),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            // Nút Xem báo cáo
            _btnXemBaoCao = new Button
            {
                Text = "🔍 Xem báo cáo",
                Width = 135,
                Height = 30,
                Margin = new Padding(0, 1, 10, 0),
                BackColor = Color.FromArgb(13, 59, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            _btnXemBaoCao.FlatAppearance.BorderSize = 0;

            // Nút Xuất Excel
            _btnXuatExcel = new Button
            {
                Text = "📊 Xuất Excel",
                Width = 135,
                Height = 30,
                Margin = new Padding(0, 1, 0, 0),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            _btnXuatExcel.FlatAppearance.BorderSize = 0;

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
                    BackColor = Color.White,
                    Margin = new Padding(5, 0, 5, 0),
                    Padding = new Padding(10, 8, 10, 8)
                };

                var lblT = new Label
                {
                    Text = title,
                    Dock = DockStyle.Top,
                    Height = 20,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = titleColor
                };

                valLabel = new Label
                {
                    Text = "0 đ",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                    ForeColor = valColor,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                card.Controls.Add(valLabel);
                card.Controls.Add(lblT);
                return card;
            }

            var card1 = CreateCard("TỔNG DOANH THU", out _lblTongDoanhThuVal, Color.FromArgb(108, 117, 125), Color.FromArgb(13, 59, 102));
            var card2 = CreateCard("TIỀN PHÒNG", out _lblTienPhongVal, Color.FromArgb(108, 117, 125), Color.FromArgb(30, 95, 138));
            var card3 = CreateCard("TIỀN DỊCH VỤ", out _lblTienDichVuVal, Color.FromArgb(108, 117, 125), Color.FromArgb(43, 122, 120));
            var card4 = CreateCard("TỶ LỆ LẤP ĐẦY", out _lblTyLeLapDayVal, Color.FromArgb(108, 117, 125), Color.FromArgb(212, 175, 55));
            var card5 = CreateCard("TỔNG BOOKING", out _lblTongBookingVal, Color.FromArgb(108, 117, 125), Color.FromArgb(44, 62, 80));

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
                BackColor = Color.White,
                Padding = new Padding(15, 8, 15, 8)
            };

            _lblTrangThai = new Label
            {
                Text = "Sẵn sàng tra cứu báo cáo.",
                Dock = DockStyle.Left,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(108, 117, 125),
                Font = new Font("Segoe UI", 9f, FontStyle.Italic)
            };

            _btnDong = new Button
            {
                Text = "Đóng",
                Width = 95,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(240, 242, 245),
                ForeColor = Color.FromArgb(33, 37, 41),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };
            _btnDong.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            _btnDong.Click += (_, _) => Close();

            pnlBottom.Controls.Add(_lblTrangThai);
            pnlBottom.Controls.Add(_btnDong);

            // 4. TabControl chứa danh sách chi tiết (Center Area)
            var pnlCenter = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 6, 15, 8)
            };

            _tabDuLieu = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            // Tab 1: Danh sách hóa đơn chi tiết
            var tabHoaDon = new TabPage("📋 Danh sách hóa đơn đã thanh toán");
            _dgvHoaDon = CreateStandardDataGridView();
            tabHoaDon.Controls.Add(_dgvHoaDon);

            // Tab 2: Phân tích theo loại phòng
            var tabLoaiPhong = new TabPage("🏢 Phân tích doanh thu theo loại phòng");
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
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                ColumnHeadersHeight = 36,
                GridColor = Color.FromArgb(233, 236, 239)
            };

            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(13, 59, 102),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 6, 8, 6)
            };

            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                SelectionBackColor = Color.FromArgb(227, 242, 253),
                SelectionForeColor = Color.FromArgb(13, 59, 102),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Padding = new Padding(6, 4, 6, 4)
            };

            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(249, 251, 252)
            };

            dgv.RowTemplate.Height = 34;
            return dgv;
        }

        #endregion
    }
}
