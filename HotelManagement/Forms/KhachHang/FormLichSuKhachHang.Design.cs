using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormLichSuKhachHang
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

        private Panel pnlTopFilter = null!;
        private FlowLayoutPanel flpFilterLeft = null!;
        private Label lblTrangThai = null!;
        private ComboBox cboTrangThai = null!;
        private Button btnLamMoi = null!;
        private Label lblTongSo = null!;

        private Panel pnlButtonRight = null!;
        private Button btnXemHoaDon = null!;

        private DataGridView dgvLichSu = null!;

        private Panel pnlBottomInfo = null!;
        private Label lblHuongDan = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = AppTheme.Background;

            // ================= 1. Top Filter Panel =================
            pnlTopFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 10, 12, 10)
            };

            // 1.1 Khối nút góc phải: Nút Xuất Hóa Đơn PDF (Dock Right, luôn nhìn thấy 100% chữ)
            pnlButtonRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 230,
                BackColor = Color.Transparent
            };

            btnXemHoaDon = new Button
            {
                Text = "📄 Xem / Tải hóa đơn PDF",
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            AppTheme.ApplyPrimaryButton(btnXemHoaDon);
            btnXemHoaDon.Click += async (s, e) => await XuatHoaDonPdfAsync();
            pnlButtonRight.Controls.Add(btnXemHoaDon);

            // 1.2 Khối bộ lọc bên trái: dùng FlowLayoutPanel để tự căn chỉnh không bao giờ đè nhau khi scale
            flpFilterLeft = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 3, 0, 0)
            };

            lblTrangThai = new Label
            {
                Text = "Lọc trạng thái:",
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Margin = new Padding(0, 8, 8, 0)
            };

            cboTrangThai = new ComboBox
            {
                Width = 150,
                Height = 32,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 4, 12, 0)
            };
            cboTrangThai.Items.AddRange(new object[] { "Tất cả", "Đã xác nhận", "Đã check-in", "Đã check-out", "Đã hủy" });
            cboTrangThai.SelectedIndex = 0;
            cboTrangThai.SelectedIndexChanged += async (s, e) => await TaiLichSuAsync();

            btnLamMoi = new Button
            {
                Text = "🔄 Làm mới",
                Size = new Size(110, 32),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 3, 16, 0)
            };
            AppTheme.ApplySecondaryButton(btnLamMoi);
            btnLamMoi.Click += async (s, e) => await TaiLichSuAsync();

            lblTongSo = new Label
            {
                Text = "Tổng số: 0 đơn đặt phòng",
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Margin = new Padding(0, 8, 0, 0)
            };

            flpFilterLeft.Controls.AddRange(new Control[]
            {
                lblTrangThai,
                cboTrangThai,
                btnLamMoi,
                lblTongSo
            });

            // Gắn vào pnlTopFilter
            pnlTopFilter.Controls.Add(flpFilterLeft);
            pnlTopFilter.Controls.Add(pnlButtonRight);

            // ================= 2. Bottom Info Panel =================
            pnlBottomInfo = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 34,
                BackColor = AppTheme.SurfaceSecondary,
                Padding = new Padding(14, 8, 14, 8)
            };

            lblHuongDan = new Label
            {
                Text = "💡 Mẹo: Nhấp đúp vào dòng đơn đặt phòng bất kỳ để xem trước hoặc tải tệp hóa đơn PDF về máy tính.",
                Dock = DockStyle.Fill,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary
            };
            pnlBottomInfo.Controls.Add(lblHuongDan);

            // ================= 3. DataGridView =================
            dgvLichSu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                BackgroundColor = AppTheme.Surface,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                RowTemplate = { Height = 36 }
            };
            AppTheme.ApplyDataGridView(dgvLichSu);

            dgvLichSu.SelectionChanged += DgvLichSu_SelectionChanged;
            dgvLichSu.CellDoubleClick += async (s, e) =>
            {
                if (e.RowIndex >= 0)
                    await XuatHoaDonPdfAsync();
            };

            // Setup columns với chiều rộng tối ưu, không bao giờ bị cắt chữ ...
            dgvLichSu.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "MaBooking",
                    DataPropertyName = "MaBooking",
                    HeaderText = "Mã Booking",
                    Width = 145
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "NgayDat",
                    DataPropertyName = "NgayDat",
                    HeaderText = "Ngày đặt",
                    Width = 145,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "NgayNhan",
                    DataPropertyName = "NgayNhan",
                    HeaderText = "Ngày nhận",
                    Width = 110,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "NgayTra",
                    DataPropertyName = "NgayTra",
                    HeaderText = "Ngày trả",
                    Width = 110,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "SoDem",
                    DataPropertyName = "SoDem",
                    HeaderText = "Số đêm",
                    Width = 75,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "DanhSachPhong",
                    DataPropertyName = "DanhSachPhong",
                    HeaderText = "Phòng & Loại phòng",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    MinimumWidth = 200
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "SoNguoi",
                    DataPropertyName = "SoNguoi",
                    HeaderText = "Số khách",
                    Width = 85,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "TrangThaiBooking",
                    DataPropertyName = "TrangThaiBooking",
                    HeaderText = "Trạng thái",
                    Width = 125
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "TrangThaiThanhToan",
                    DataPropertyName = "TrangThaiThanhToan",
                    HeaderText = "Thanh toán",
                    Width = 135
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "TongTien",
                    DataPropertyName = "TongTien",
                    HeaderText = "Tổng tiền (VNĐ)",
                    Width = 130,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
                }
            });

            dgvLichSu.CellFormatting += DgvLichSu_CellFormatting;

            this.Controls.Add(dgvLichSu);
            this.Controls.Add(pnlBottomInfo);
            this.Controls.Add(pnlTopFilter);
        }

        #endregion
    }
}
