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
        private Button _btnTraCuuNangCao = null!;
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
            AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            // ================= 1. GroupBox: Tra cứu =================
            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu mã booking",
                Location = new Point(16, 10),
                Size = new Size(1232, 65),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold
            };

            var lblMaBooking = new Label
            {
                Text = "Mã Booking:",
                Location = new Point(16, 26),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextSecondary
            };

            _txtMaBooking = new TextBox
            {
                Location = new Point(140, 23),
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
                Location = new Point(415, 21),
                Size = new Size(100, 32)
            };
            AppTheme.ApplyPrimaryButton(_btnTim);
            _btnTim.Click += async (_, _) => await TraCuuBookingAsync();

            _btnTraCuuNangCao = new Button
            {
                Text = "Tra cứu nâng cao",
                Location = new Point(525, 21),
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
                Location = new Point(700, 21),
                Size = new Size(90, 32)
            };
            AppTheme.ApplySecondaryButton(_btnLamMoi);
            _btnLamMoi.Click += (_, _) => XoaTrangForm();

            grpTraCuu.Controls.AddRange(new Control[] { lblMaBooking, _txtMaBooking, _btnTim, _btnTraCuuNangCao, _btnLamMoi });

            AppTheme.AlignInputAfterLabel(lblMaBooking, _txtMaBooking);
            _btnTim.Left = _txtMaBooking.Right + 10;
            _btnTraCuuNangCao.Left = _btnTim.Right + 10;
            _btnLamMoi.Left = _btnTraCuuNangCao.Right + 10;

            // ================= 2. Panel Trái: Thông tin Booking & Thanh toán =================
            var grpThongTin = new GroupBox
            {
                Text = "Thông tin lưu trú - Thanh toán",
                Location = new Point(16, 80),
                Size = new Size(410, 644),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                Font = AppTheme.FontBodyBold
            };

            var tblThongTin = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 13,
                Padding = new Padding(10, 15, 10, 10),
                AutoScroll = true
            };
            tblThongTin.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblThongTin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            void ThemDongThongTin(string nhan, Control giaTri, int row, bool inDamNhan = false)
            {
                var lblNhan = new Label
                {
                    Text = nhan,
                    AutoSize = true,
                    ForeColor = inDamNhan ? AppTheme.TextPrimary : AppTheme.TextSecondary,
                    Font = inDamNhan ? AppTheme.FontBodyBold : AppTheme.FontBody,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    Margin = new Padding(0, 4, 4, 4)
                };
                giaTri.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                giaTri.Margin = new Padding(4, 4, 0, 4);

                tblThongTin.Controls.Add(lblNhan, 0, row);
                tblThongTin.Controls.Add(giaTri, 1, row);
            }

            _lblMaBookingVal = new Label { Text = "-", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary };
            ThemDongThongTin("Mã booking:", _lblMaBookingVal, 0);

            _lblTrangThaiVal = new Label { Text = "-", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary };
            ThemDongThongTin("Trạng thái:", _lblTrangThaiVal, 1);

            _lblKhachHangVal = new Label { Text = "-", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary };
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
            var pnlSep = new Panel { Height = 2, BackColor = AppTheme.BorderLight, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 8) };
            tblThongTin.Controls.Add(pnlSep, 0, 8);
            tblThongTin.SetColumnSpan(pnlSep, 2);

            _lblTongTienPhongVal = new Label { Text = "0 đ", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary };
            ThemDongThongTin("Tiền phòng (tạm tính):", _lblTongTienPhongVal, 9);

            _lblTongTienDichVuVal = new Label { Text = "0 đ", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary };
            ThemDongThongTin("Tiền dịch vụ phát sinh:", _lblTongTienDichVuVal, 10);

            _lblTongCongVal = new Label
            {
                Text = "0 đ",
                AutoSize = true,
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary
            };
            ThemDongThongTin("TỔNG CỘNG:", _lblTongCongVal, 11, inDamNhan: true);

            _lblTrangThaiThanhToanVal = new Label
            {
                Text = "-",
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.StatusWarningText
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
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold
            };

            _dgvPhong = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(_dgvPhong);

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
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = AppTheme.FontBodyBold
            };

            _dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(_dgvDichVu);

            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Visible = false });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenDichVu", HeaderText = "Tên dịch vụ", FillWeight = 38, MinimumWidth = 150 });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoLuong",
                HeaderText = "SL",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DonGia",
                HeaderText = "Đơn giá chốt",
                FillWeight = 22,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThanhTien",
                HeaderText = "Thành tiền",
                FillWeight = 22,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            });
            _dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThoiGian",
                HeaderText = "Thời gian dùng",
                FillWeight = 24,
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
                Enabled = false,
                Font = AppTheme.FontBodyBold
            };

            // Hàng 1: Dịch vụ & Giá
            var lblChonDV = new Label { Text = "Dịch vụ:", Location = new Point(16, 28), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _cboDichVu = new ComboBox
            {
                Location = new Point(95, 24),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            AppTheme.ApplyComboBox(_cboDichVu);
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
                Location = new Point(390, 28),
                AutoSize = true,
                ForeColor = AppTheme.Primary,
                Font = AppTheme.FontBodyBold
            };

            // Hàng 2: Số lượng & Nút thao tác
            var lblSL = new Label { Text = "Số lượng:", Location = new Point(16, 68), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _numSoLuong = new NumericUpDown
            {
                Location = new Point(95, 65),
                Width = 75,
                Minimum = 1,
                Maximum = 99,
                Value = 1
            };
            AppTheme.ApplyNumericUpDown(_numSoLuong);

            _btnThemDichVu = new Button
            {
                Text = "+ Thêm dịch vụ",
                Location = new Point(185, 62),
                Size = new Size(145, 32)
            };
            AppTheme.ApplyPrimaryButton(_btnThemDichVu);
            _btnThemDichVu.Click += async (_, _) => await ThemDichVuAsync();

            _btnXoaDichVu = new Button
            {
                Text = "Xóa DV đã chọn",
                Location = new Point(340, 62),
                Size = new Size(145, 32)
            };
            AppTheme.ApplySecondaryButton(_btnXoaDichVu);
            _btnXoaDichVu.Click += async (_, _) => await XoaDichVuAsync();

            _grpThemDichVu.Controls.AddRange(new Control[] { lblChonDV, _cboDichVu, _lblDonGiaDichVu, lblSL, _numSoLuong, _btnThemDichVu, _btnXoaDichVu });

            AppTheme.AlignInputAfterLabel(lblChonDV, _cboDichVu, 12);
            _lblDonGiaDichVu.Left = _cboDichVu.Right + 16;
            AppTheme.AlignInputAfterLabel(lblSL, _numSoLuong, 12);
            if (_cboDichVu.Left < _numSoLuong.Left) _cboDichVu.Left = _numSoLuong.Left;
            else _numSoLuong.Left = _cboDichVu.Left;
            _lblDonGiaDichVu.Left = _cboDichVu.Right + 16;
            _btnThemDichVu.Left = _numSoLuong.Right + 16;
            _btnXoaDichVu.Left = _btnThemDichVu.Right + 12;

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
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontCaption
            };

            _btnThanhToan = new Button
            {
                Text = "Thanh toán",
                Location = new Point(735, 6),
                Size = new Size(170, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Enabled = false
            };
            AppTheme.ApplySecondaryButton(_btnThanhToan);
            _btnThanhToan.Click += async (_, _) => await MoManHinhThanhToanAsync();

            _btnCheckOut = new Button
            {
                Text = "Xác nhận Check-out",
                Location = new Point(915, 6),
                Size = new Size(200, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Enabled = false
            };
            AppTheme.ApplyPrimaryButton(_btnCheckOut);
            _btnCheckOut.Click += async (_, _) => await ThucHienCheckOutAsync();

            _btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(1125, 6),
                Size = new Size(95, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnDong);
            _btnDong.Click += (_, _) => Close();

            pnlBottom.Controls.AddRange(new Control[] { _lblThongBao, _btnThanhToan, _btnCheckOut, _btnDong });

            // Add all controls to Form
            Controls.AddRange(new Control[] { grpTraCuu, grpThongTin, grpPhong, grpDichVu, _grpThemDichVu, pnlBottom });
        }

        #endregion
    }
}
