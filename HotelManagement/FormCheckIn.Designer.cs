using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormCheckIn
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // UI Controls - Tìm kiếm
        private TextBox _txtMaBooking = null!;
        private Button _btnTim = null!;
        private Button _btnLamMoi = null!;

        // UI Controls - Thông tin booking & khách hàng
        private Label _lblMaBookingVal = null!;
        private Label _lblTrangThaiVal = null!;
        private Label _lblKhachHangVal = null!;
        private Label _lblSdtVal = null!;
        private Label _lblCccdVal = null!;
        private Label _lblEmailVal = null!;
        private Label _lblNgayDatVal = null!;
        private Label _lblNgayNhanDuKienVal = null!;
        private Label _lblNgayTraDuKienVal = null!;
        private Label _lblNgayNhanThucTeVal = null!;
        private Label _lblSoNguoiVal = null!;

        // UI Controls - Danh sách phòng
        private DataGridView _dgvPhong = null!;

        // UI Controls - Thao tác
        private Label _lblThongBao = null!;
        private Button _btnCheckIn = null!;
        private Button _btnDong = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            Text = "Check-in khách sạn — Tra cứu mã booking";
            Width = 1180;
            Height = 780;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(950, 680);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // ================= 1. GroupBox: Tra cứu Booking =================
            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu mã booking",
                Location = new Point(16, 12),
                Size = new Size(1132, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblMaBooking = new Label
            {
                Text = "Mã Booking:",
                Location = new Point(16, 28),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold)
            };

            _txtMaBooking = new TextBox
            {
                Location = new Point(140, 25),
                Width = 260,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular)
            };
            _txtMaBooking.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await TraCuuBookingAsync();
                }
            };

            _btnTim = new Button
            {
                Text = "Tra cứu",
                Location = new Point(415, 23),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(Font, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnTim.FlatAppearance.BorderSize = 0;
            _btnTim.Click += async (_, _) => await TraCuuBookingAsync();

            _btnLamMoi = new Button
            {
                Text = "Làm mới",
                Location = new Point(525, 23),
                Size = new Size(90, 32),
                Cursor = Cursors.Hand
            };
            _btnLamMoi.Click += (_, _) => XoaTrangForm();

            grpTraCuu.Controls.AddRange(new Control[] { lblMaBooking, _txtMaBooking, _btnTim, _btnLamMoi });

            // ================= 2. GroupBox: Thông tin Đặt phòng & Khách hàng =================
            var grpThongTin = new GroupBox
            {
                Text = "Thông tin đặt phòng & Khách hàng",
                Location = new Point(16, 88),
                Size = new Size(1132, 190),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int col1L = 20, col1V = 185;
            int col2L = 480, col2V = 620;

            // Dòng 1: Mã booking & Khách hàng
            grpThongTin.Controls.Add(new Label { Text = "Mã booking:", Location = new Point(col1L, 24), AutoSize = true, ForeColor = Color.DimGray });
            _lblMaBookingVal = new Label { Text = "-", Location = new Point(col1V, 24), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            grpThongTin.Controls.Add(_lblMaBookingVal);

            grpThongTin.Controls.Add(new Label { Text = "Khách hàng:", Location = new Point(col2L, 24), AutoSize = true, ForeColor = Color.DimGray });
            _lblKhachHangVal = new Label { Text = "-", Location = new Point(col2V, 24), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            grpThongTin.Controls.Add(_lblKhachHangVal);

            // Dòng 2: Ngày đặt & Số điện thoại
            grpThongTin.Controls.Add(new Label { Text = "Ngày đặt:", Location = new Point(col1L, 50), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayDatVal = new Label { Text = "-", Location = new Point(col1V, 50), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayDatVal);

            grpThongTin.Controls.Add(new Label { Text = "Số điện thoại:", Location = new Point(col2L, 50), AutoSize = true, ForeColor = Color.DimGray });
            _lblSdtVal = new Label { Text = "-", Location = new Point(col2V, 50), AutoSize = true };
            grpThongTin.Controls.Add(_lblSdtVal);

            // Dòng 3: Trạng thái & CCCD
            grpThongTin.Controls.Add(new Label { Text = "Trạng thái booking:", Location = new Point(col1L, 76), AutoSize = true, ForeColor = Color.DimGray });
            _lblTrangThaiVal = new Label { Text = "-", Location = new Point(col1V, 74), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            grpThongTin.Controls.Add(_lblTrangThaiVal);

            grpThongTin.Controls.Add(new Label { Text = "CCCD/CMND:", Location = new Point(col2L, 76), AutoSize = true, ForeColor = Color.DimGray });
            _lblCccdVal = new Label { Text = "-", Location = new Point(col2V, 76), AutoSize = true };
            grpThongTin.Controls.Add(_lblCccdVal);

            // Dòng 4: Ngày nhận dự kiến & Email
            grpThongTin.Controls.Add(new Label { Text = "Ngày nhận (dự kiến):", Location = new Point(col1L, 102), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayNhanDuKienVal = new Label { Text = "-", Location = new Point(col1V, 102), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayNhanDuKienVal);

            grpThongTin.Controls.Add(new Label { Text = "Email:", Location = new Point(col2L, 102), AutoSize = true, ForeColor = Color.DimGray });
            _lblEmailVal = new Label { Text = "-", Location = new Point(col2V, 102), AutoSize = true };
            grpThongTin.Controls.Add(_lblEmailVal);

            // Dòng 5: Ngày trả dự kiến & Số khách
            grpThongTin.Controls.Add(new Label { Text = "Ngày trả (dự kiến):", Location = new Point(col1L, 128), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayTraDuKienVal = new Label { Text = "-", Location = new Point(col1V, 128), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayTraDuKienVal);

            grpThongTin.Controls.Add(new Label { Text = "Số khách đăng ký:", Location = new Point(col2L, 128), AutoSize = true, ForeColor = Color.DimGray });
            _lblSoNguoiVal = new Label { Text = "-", Location = new Point(col2V, 128), AutoSize = true };
            grpThongTin.Controls.Add(_lblSoNguoiVal);

            // Dòng 6: Ngày nhận thực tế
            grpThongTin.Controls.Add(new Label { Text = "Ngày nhận thực tế:", Location = new Point(col1L, 154), AutoSize = true, ForeColor = Color.DimGray });
            _lblNgayNhanThucTeVal = new Label { Text = "-", Location = new Point(col1V, 154), AutoSize = true };
            grpThongTin.Controls.Add(_lblNgayNhanThucTeVal);

            // ================= 3. GroupBox: Danh sách phòng đặt =================
            var grpDanhSachPhong = new GroupBox
            {
                Text = "Danh sách phòng trong booking",
                Location = new Point(16, 286),
                Size = new Size(1132, 369),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _dgvPhong = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                RowTemplate = { Height = 32 }
            };

            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdPhong", HeaderText = "ID", Visible = false });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoPhong", HeaderText = "Số phòng", FillWeight = 20 });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenLoaiPhong", HeaderText = "Loại phòng", FillWeight = 30 });
            
            var colGia = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DonGia",
                HeaderText = "Đơn giá (VNĐ/đêm)",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            };
            _dgvPhong.Columns.Add(colGia);

            var colTrangThai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThaiPhongHienTai",
                HeaderText = "Trạng thái phòng hiện tại",
                FillWeight = 25
            };
            _dgvPhong.Columns.Add(colTrangThai);

            _dgvPhong.CellFormatting += DgvPhong_CellFormatting;

            grpDanhSachPhong.Controls.Add(_dgvPhong);

            // ================= 4. Panel dưới: Thao tác & Thông báo =================
            var pnlBottom = new Panel
            {
                Location = new Point(16, 665),
                Size = new Size(1132, 60),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _lblThongBao = new Label
            {
                Text = "Vui lòng nhập mã booking và nhấn 'Tra cứu' để kiểm tra thông tin.",
                Location = new Point(4, 18),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray,
                Font = new Font(Font, FontStyle.Italic)
            };

            _btnCheckIn = new Button
            {
                Text = "Xác nhận Check-in",
                Location = new Point(890, 10),
                Size = new Size(150, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnCheckIn.FlatAppearance.BorderSize = 0;
            _btnCheckIn.Click += async (_, _) => await ThucHienCheckInAsync();

            _btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(1050, 10),
                Size = new Size(70, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            _btnDong.Click += (_, _) => Close();

            pnlBottom.Controls.AddRange(new Control[] { _lblThongBao, _btnCheckIn, _btnDong });

            // Thêm tất cả vào Form
            Controls.AddRange(new Control[] { grpTraCuu, grpThongTin, grpDanhSachPhong, pnlBottom });
        }

        #endregion
    }
}
