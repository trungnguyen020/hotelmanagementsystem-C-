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
        private Button _btnTraCuuNangCao = null!;
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
        private Button _btnHuyDatPhong = null!;
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
            AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            // ================= 1. GroupBox: Tra cứu Booking =================
            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu mã booking",
                Location = new Point(16, 12),
                Size = new Size(1132, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold
            };

            var lblMaBooking = new Label
            {
                Text = "Mã Booking:",
                Location = new Point(16, 28),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextSecondary
            };

            _txtMaBooking = new TextBox
            {
                Location = new Point(140, 25),
                Width = 260
            };
            AppTheme.ApplyTextBox(_txtMaBooking);
            _txtMaBooking.Font = AppTheme.FontSubHeader;
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
                Size = new Size(100, 32)
            };
            AppTheme.ApplyPrimaryButton(_btnTim);
            _btnTim.Click += async (_, _) => await TraCuuBookingAsync();

            _btnTraCuuNangCao = new Button
            {
                Text = "Tra cứu nâng cao",
                Location = new Point(525, 23),
                Size = new Size(165, 32)
            };
            AppTheme.ApplySecondaryButton(_btnTraCuuNangCao);
            _btnTraCuuNangCao.Click += async (_, _) =>
            {
                using var frm = new FormTraCuuBooking();
                if (frm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(frm.MaBookingDaChon))
                {
                    _txtMaBooking.Text = frm.MaBookingDaChon;
                    await TraCuuBookingAsync();
                }
            };

            _btnLamMoi = new Button
            {
                Text = "Làm mới",
                Location = new Point(700, 23),
                Size = new Size(90, 32)
            };
            AppTheme.ApplySecondaryButton(_btnLamMoi);
            _btnLamMoi.Click += (_, _) => XoaTrangForm();

            grpTraCuu.Controls.AddRange(new Control[] { lblMaBooking, _txtMaBooking, _btnTim, _btnTraCuuNangCao, _btnLamMoi });

            AppTheme.AlignInputAfterLabel(lblMaBooking, _txtMaBooking);
            _btnTim.Left = _txtMaBooking.Right + 10;
            _btnTraCuuNangCao.Left = _btnTim.Right + 10;
            _btnLamMoi.Left = _btnTraCuuNangCao.Right + 10;

            // ================= 2. GroupBox: Thông tin Đặt phòng & Khách hàng =================
            var grpThongTin = new GroupBox
            {
                Text = "Thông tin đặt phòng & Khách hàng",
                Location = new Point(16, 88),
                Size = new Size(1132, 190),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold
            };

            const int col1L = 20;
            const int row1 = 24, row2 = 50, row3 = 76, row4 = 102, row5 = 128, row6 = 154;

            Label TaoNhan(string text, int x, int y) => new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary
            };

            Label TaoGiaTri(int x, int y, bool bold = false) => new Label
            {
                Text = "-",
                Location = new Point(x, y),
                AutoSize = true,
                Font = bold ? AppTheme.FontBodyBold : AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary
            };

            var nhanCot1 = new[]
            {
                TaoNhan("Mã booking:", col1L, row1),
                TaoNhan("Ngày đặt:", col1L, row2),
                TaoNhan("Trạng thái booking:", col1L, row3),
                TaoNhan("Ngày nhận (dự kiến):", col1L, row4),
                TaoNhan("Ngày trả (dự kiến):", col1L, row5),
                TaoNhan("Ngày nhận thực tế:", col1L, row6)
            };

            int col1V = 185;
            foreach (var lbl in nhanCot1)
            {
                col1V = Math.Max(col1V, lbl.Right + 14);
            }

            int col2L = col1V + 280;

            var nhanCot2 = new[]
            {
                TaoNhan("Khách hàng:", col2L, row1),
                TaoNhan("Số điện thoại:", col2L, row2),
                TaoNhan("CCCD/CMND:", col2L, row3),
                TaoNhan("Email:", col2L, row4),
                TaoNhan("Số khách đăng ký:", col2L, row5)
            };

            int col2V = 620;
            foreach (var lbl in nhanCot2)
            {
                col2V = Math.Max(col2V, lbl.Right + 14);
            }

            _lblMaBookingVal = TaoGiaTri(col1V, row1, bold: true);
            _lblNgayDatVal = TaoGiaTri(col1V, row2);
            _lblTrangThaiVal = TaoGiaTri(col1V, row3 - 2, bold: true);
            _lblNgayNhanDuKienVal = TaoGiaTri(col1V, row4);
            _lblNgayTraDuKienVal = TaoGiaTri(col1V, row5);
            _lblNgayNhanThucTeVal = TaoGiaTri(col1V, row6);
            _lblKhachHangVal = TaoGiaTri(col2V, row1, bold: true);
            _lblSdtVal = TaoGiaTri(col2V, row2);
            _lblCccdVal = TaoGiaTri(col2V, row3);
            _lblEmailVal = TaoGiaTri(col2V, row4);
            _lblSoNguoiVal = TaoGiaTri(col2V, row5);

            grpThongTin.Controls.AddRange(nhanCot1);
            grpThongTin.Controls.AddRange(nhanCot2);
            grpThongTin.Controls.AddRange(new Control[]
            {
                _lblMaBookingVal, _lblNgayDatVal, _lblTrangThaiVal,
                _lblNgayNhanDuKienVal, _lblNgayTraDuKienVal, _lblNgayNhanThucTeVal,
                _lblKhachHangVal, _lblSdtVal, _lblCccdVal, _lblEmailVal, _lblSoNguoiVal
            });

            // ================= 3. GroupBox: Danh sách phòng đặt =================
            var grpDanhSachPhong = new GroupBox
            {
                Text = "Danh sách phòng trong booking",
                Location = new Point(16, 286),
                Size = new Size(1132, 369),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold
            };

            _dgvPhong = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(_dgvPhong);

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
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontCaption
            };

            _btnHuyDatPhong = new Button
            {
                Text = "❌ Hủy đặt phòng",
                Location = new Point(730, 10),
                Size = new Size(150, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Enabled = false
            };
            AppTheme.ApplyDangerButton(_btnHuyDatPhong);
            _btnHuyDatPhong.Click += async (_, _) => await ThucHienHuyDatPhongAsync();

            _btnCheckIn = new Button
            {
                Text = "Xác nhận Check-in",
                Location = new Point(890, 10),
                Size = new Size(150, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Enabled = false
            };
            AppTheme.ApplyPrimaryButton(_btnCheckIn);
            _btnCheckIn.Click += async (_, _) => await ThucHienCheckInAsync();

            _btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(1050, 10),
                Size = new Size(70, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnDong);
            _btnDong.Click += (_, _) => Close();

            pnlBottom.Controls.AddRange(new Control[] { _lblThongBao, _btnHuyDatPhong, _btnCheckIn, _btnDong });

            // Thêm tất cả vào Form
            Controls.AddRange(new Control[] { grpTraCuu, grpThongTin, grpDanhSachPhong, pnlBottom });
        }

        #endregion
    }
}
