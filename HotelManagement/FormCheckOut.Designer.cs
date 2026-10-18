using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;

namespace HotelManagement.Forms
{
    partial class FormCheckOut
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

        // UI Controls - Thông tin booking & Khách hàng
        private Label _lblMaBookingVal = null!;
        private Label _lblTrangThaiVal = null!;
        private Label _lblKhachHangVal = null!;
        private Label _lblSdtVal = null!;
        private Label _lblCccdVal = null!;
        private Label _lblNgayNhanThucTeVal = null!;
        private Label _lblNgayTraDuKienVal = null!;
        private Label _lblNgayTraThucTeVal = null!;
        private Label _lblTongTienPhongVal = null!;
        private Label _lblTongTienDichVuVal = null!;
        private Label _lblTongCongVal = null!;

        // UI Controls - Bảng phòng và Dịch vụ
        private DataGridView _dgvPhong = null!;
        private DataGridView _dgvDichVu = null!;

        // UI Controls - Thêm dịch vụ phát sinh
        private GroupBox _grpThemDichVu = null!;
        private ComboBox _cboDichVu = null!;
        private Label _lblDonGiaDichVu = null!;
        private NumericUpDown _numSoLuong = null!;
        private Button _btnThemDichVu = null!;
        private Button _btnXoaDichVu = null!;

        // UI Controls - Thao tác Check-out & Thanh toán
        private Label _lblThongBao = null!;
        private Label _lblTrangThaiThanhToanVal = null!;
        private Button _btnThanhToan = null!;
        private Button _btnCheckOut = null!;
        private Button _btnDong = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            Text = "Check-out & Dịch vụ phát sinh — Quản lý lưu trú";
            Width = 1280;
            Height = 850;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 740);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // ================= 1. GroupBox: Tra cứu =================
            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu mã booking",
                Location = new Point(16, 10),
                Size = new Size(1232, 65),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblMaBooking = new Label
            {
                Text = "Mã Booking:",
                Location = new Point(16, 26),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold)
            };

            _txtMaBooking = new TextBox
            {
                Location = new Point(140, 23),
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
                Location = new Point(415, 21),
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
                Location = new Point(525, 21),
                Size = new Size(90, 32),
                Cursor = Cursors.Hand
            };
            _btnLamMoi.Click += (_, _) => XoaTrangForm();

            grpTraCuu.Controls.AddRange(new Control[] { lblMaBooking, _txtMaBooking, _btnTim, _btnLamMoi });

            // ================= 2. Panel Trái: Thông tin Booking & Thanh toán =================
            var grpThongTin = new GroupBox
            {
                Text = "Thông tin lưu trú - Thanh toán",
                Location = new Point(16, 80),
                Size = new Size(410, 644),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            var tblThongTin = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 13,
                Padding = new Padding(10, 15, 10, 10),
                AutoScroll = true
            };
            tblThongTin.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185f));
            tblThongTin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            void ThemDongThongTin(string nhan, Control giaTri, int row, bool inDamNhan = false)
            {
                var lblNhan = new Label
                {
                    Text = nhan,
                    AutoSize = true,
                    ForeColor = inDamNhan ? Color.Black : Color.DimGray,
                    Font = inDamNhan ? new Font(Font, FontStyle.Bold) : Font,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    Margin = new Padding(0, 4, 4, 4)
                };
                giaTri.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                giaTri.Margin = new Padding(4, 4, 0, 4);

                tblThongTin.Controls.Add(lblNhan, 0, row);
                tblThongTin.Controls.Add(giaTri, 1, row);
            }

            _lblMaBookingVal = new Label { Text = "-", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            ThemDongThongTin("Mã booking:", _lblMaBookingVal, 0);

            _lblTrangThaiVal = new Label { Text = "-", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            ThemDongThongTin("Trạng thái:", _lblTrangThaiVal, 1);

            _lblKhachHangVal = new Label { Text = "-", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            ThemDongThongTin("Khách hàng:", _lblKhachHangVal, 2);

            _lblSdtVal = new Label { Text = "-", AutoSize = true };
            ThemDongThongTin("Số điện thoại:", _lblSdtVal, 3);

            _lblCccdVal = new Label { Text = "-", AutoSize = true };
            ThemDongThongTin("CCCD/CMND:", _lblCccdVal, 4);

            _lblNgayNhanThucTeVal = new Label { Text = "-", AutoSize = true };
            ThemDongThongTin("Ngày nhận (thực tế):", _lblNgayNhanThucTeVal, 5);

            _lblNgayTraDuKienVal = new Label { Text = "-", AutoSize = true };
            ThemDongThongTin("Ngày trả (dự kiến):", _lblNgayTraDuKienVal, 6);

            _lblNgayTraThucTeVal = new Label { Text = "-", AutoSize = true };
            ThemDongThongTin("Ngày trả (thực tế):", _lblNgayTraThucTeVal, 7);

            // Đường phân cách
            var pnlSep = new Panel { Height = 2, BackColor = Color.LightGray, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 8) };
            tblThongTin.Controls.Add(pnlSep, 0, 8);
            tblThongTin.SetColumnSpan(pnlSep, 2);

            _lblTongTienPhongVal = new Label { Text = "0 đ", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            ThemDongThongTin("Tiền phòng (tạm tính):", _lblTongTienPhongVal, 9);

            _lblTongTienDichVuVal = new Label { Text = "0 đ", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            ThemDongThongTin("Tiền dịch vụ phát sinh:", _lblTongTienDichVuVal, 10);

            _lblTongCongVal = new Label
            {
                Text = "0 đ",
                AutoSize = true,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.Firebrick
            };
            ThemDongThongTin("TỔNG CỘNG:", _lblTongCongVal, 11, inDamNhan: true);

            _lblTrangThaiThanhToanVal = new Label
            {
                Text = "-",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkOrange
            };
            ThemDongThongTin("Thanh toán:", _lblTrangThaiThanhToanVal, 12, inDamNhan: true);

            grpThongTin.Controls.Add(tblThongTin);

            // ================= 3. Panel Phải - Trên: Danh sách phòng đang ở =================
            int rightX = 438;
            var grpPhong = new GroupBox
            {
                Text = "Danh sách phòng đang lưu trú",
                Location = new Point(rightX, 80),
                Size = new Size(810, 160),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _dgvPhong = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                RowTemplate = { Height = 30 }
            };

            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoPhong", HeaderText = "Số phòng", FillWeight = 20 });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenLoaiPhong", HeaderText = "Loại phòng", FillWeight = 30 });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DonGia",
                HeaderText = "Đơn giá / đêm",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            });
            _dgvPhong.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThaiPhongHienTai", HeaderText = "Trạng thái phòng", FillWeight = 25 });
            _dgvPhong.CellFormatting += DgvPhong_CellFormatting;

            grpPhong.Controls.Add(_dgvPhong);

            // ================= 4. Panel Phải - Giữa: Danh sách dịch vụ phát sinh =================
            var grpDichVu = new GroupBox
            {
                Text = "Dịch vụ phát sinh đã sử dụng",
                Location = new Point(rightX, 245),
                Size = new Size(810, 354),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                RowTemplate = { Height = 30 }
            };

            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Visible = false });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenDichVu", HeaderText = "Tên dịch vụ", FillWeight = 30 });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoLuong",
                HeaderText = "Số lượng",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DonGia",
                HeaderText = "Đơn giá chốt",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThanhTien",
                HeaderText = "Thành tiền",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThoiGian",
                HeaderText = "Thời gian dùng",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });

            grpDichVu.Controls.Add(_dgvDichVu);

            // ================= 5. Panel Phải - Dưới: Thêm dịch vụ phát sinh (2 hàng rộng rãi) =================
            _grpThemDichVu = new GroupBox
            {
                Text = "Thêm dịch vụ phát sinh vào đợt lưu trú",
                Location = new Point(rightX, 609),
                Size = new Size(810, 115),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Enabled = false
            };

            // Hàng 1: Dịch vụ & Giá
            var lblChonDV = new Label { Text = "Dịch vụ:", Location = new Point(16, 28), AutoSize = true };
            _cboDichVu = new ComboBox
            {
                Location = new Point(80, 24),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboDichVu.SelectedIndexChanged += (_, _) =>
            {
                if (_cboDichVu.SelectedItem is DichVuDto dv)
                {
                    _lblDonGiaDichVu.Text = $"Đơn giá hiện tại: {dv.MucGia:N0} đ";
                }
            };

            _lblDonGiaDichVu = new Label
            {
                Text = "Đơn giá: -",
                Location = new Point(360, 28),
                AutoSize = true,
                ForeColor = Color.DarkSlateBlue,
                Font = new Font(Font, FontStyle.Bold)
            };

            // Hàng 2: Số lượng & Nút thao tác
            var lblSL = new Label { Text = "Số lượng:", Location = new Point(16, 68), AutoSize = true };
            _numSoLuong = new NumericUpDown
            {
                Location = new Point(95, 65),
                Width = 70,
                Minimum = 1,
                Maximum = 99,
                Value = 1
            };

            _btnThemDichVu = new Button
            {
                Text = "+ Thêm dịch vụ",
                Location = new Point(180, 62),
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(Font, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnThemDichVu.FlatAppearance.BorderSize = 0;
            _btnThemDichVu.Click += async (_, _) => await ThemDichVuAsync();

            _btnXoaDichVu = new Button
            {
                Text = "Xóa DV đã chọn",
                Location = new Point(330, 62),
                Size = new Size(140, 32),
                Cursor = Cursors.Hand
            };
            _btnXoaDichVu.Click += async (_, _) => await XoaDichVuAsync();

            _grpThemDichVu.Controls.AddRange(new Control[] { lblChonDV, _cboDichVu, _lblDonGiaDichVu, lblSL, _numSoLuong, _btnThemDichVu, _btnXoaDichVu });

            // ================= 6. Bottom Bar: Nút hành động Check-out =================
            var pnlBottom = new Panel
            {
                Location = new Point(16, 740),
                Size = new Size(1232, 55),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _lblThongBao = new Label
            {
                Text = "Vui lòng nhập mã booking và nhấn 'Tra cứu' để kiểm tra thông tin.",
                Location = new Point(6, 16),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray,
                Font = new Font(Font, FontStyle.Italic)
            };

            _btnThanhToan = new Button
            {
                Text = "Thanh toán",
                Location = new Point(735, 6),
                Size = new Size(170, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(39, 174, 96), // Xanh lá cây nổi bật
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnThanhToan.FlatAppearance.BorderSize = 0;
            _btnThanhToan.Click += async (_, _) => await MoManHinhThanhToanAsync();

            _btnCheckOut = new Button
            {
                Text = "Xác nhận Check-out",
                Location = new Point(915, 6),
                Size = new Size(200, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(192, 57, 43), // Đỏ mận sang trọng
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnCheckOut.FlatAppearance.BorderSize = 0;
            _btnCheckOut.Click += async (_, _) => await ThucHienCheckOutAsync();

            _btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(1125, 6),
                Size = new Size(95, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            _btnDong.Click += (_, _) => Close();

            pnlBottom.Controls.AddRange(new Control[] { _lblThongBao, _btnThanhToan, _btnCheckOut, _btnDong });

            // Add all controls to Form
            Controls.AddRange(new Control[] { grpTraCuu, grpThongTin, grpPhong, grpDichVu, _grpThemDichVu, pnlBottom });
        }

        #endregion
    }
}
