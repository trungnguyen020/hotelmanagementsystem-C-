using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormThanhToan
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

        // UI Controls - Tra cứu
        private TextBox _txtMaBookingTraCuu = null!;
        private Button _btnTraCuu = null!;
        private Button _btnTraCuuNangCao = null!;

        // UI Controls - Thông tin booking & Khách hàng
        private Label _lblMaBooking = null!;
        private Label _lblKhachHang = null!;
        private Label _lblSdt = null!;
        private Label _lblThoiGianLuuTru = null!;
        private Label _lblTrangThaiHienTai = null!;

        // UI Controls - Chi phí
        private Label _lblTienPhongVal = null!;
        private Label _lblTienDichVuVal = null!;
        private NumericUpDown _numGiamGia = null!;
        private Button _btnMienPhiDichVu = null!;
        private Button _btnGiam10 = null!;
        private Button _btnGiam20 = null!;
        private Button _btnHuyKm = null!;
        private Label _lblGhiChuKhuyenMai = null!;
        private Label _lblTongCongVal = null!;

        // UI Controls - Hình thức thanh toán
        private RadioButton _rdoTienMat = null!;
        private RadioButton _rdoChuyenKhoan = null!;

        // Panel Tiền mặt
        private Panel _pnlTienMat = null!;
        private NumericUpDown _numTienKhachDua = null!;
        private Label _lblTienThuaVal = null!;

        // Panel Chuyển khoản QR tĩnh
        private Panel _pnlChuyenKhoan = null!;
        private PictureBox _picQr = null!;
        private Label _lblSoTienChuyenKhoanVal = null!;
        private Label _lblNoiDungChuyenKhoanVal = null!;

        // UI Controls - Thao tác
        private Button _btnXuatPdf = null!;
        private Button _btnHuyPdf = null!;
        private Button _btnXacNhan = null!;
        private Button _btnHuy = null!;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            Text = "Thanh toán hóa đơn — Quản lý khách sạn (DCKLTTQ-76)";
            Size = new Size(880, 850);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            // ================= 1. GroupBox: Tra cứu mã booking =================
            var grpTraCuu = new GroupBox
            {
                Text = "Tra cứu hóa đơn đặt phòng",
                Location = new Point(20, 10),
                Size = new Size(825, 65),
                Font = AppTheme.FontBodyBold
            };

            var lblMaBookingPrompt = new Label
            {
                Text = "Mã booking:",
                Location = new Point(20, 26),
                AutoSize = true,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary
            };

            _txtMaBookingTraCuu = new TextBox
            {
                Location = new Point(120, 23),
                Width = 240
            };
            AppTheme.ApplyTextBox(_txtMaBookingTraCuu);
            _txtMaBookingTraCuu.Font = AppTheme.FontSubHeader;
            _txtMaBookingTraCuu.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await TaiDuLieuHoaDonAsync(_txtMaBookingTraCuu.Text.Trim());
                }
            };

            _btnTraCuu = new Button
            {
                Text = "Tra cứu",
                Location = new Point(375, 21),
                Size = new Size(100, 32)
            };
            AppTheme.ApplyPrimaryButton(_btnTraCuu);
            _btnTraCuu.Click += async (_, _) => await TaiDuLieuHoaDonAsync(_txtMaBookingTraCuu.Text.Trim());

            _btnTraCuuNangCao = new Button
            {
                Text = "Nâng cao",
                Location = new Point(490, 21),
                Size = new Size(120, 32)
            };
            AppTheme.ApplySecondaryButton(_btnTraCuuNangCao);
            _btnTraCuuNangCao.Click += async (_, _) =>
            {
                using var frm = new FormTraCuuBooking();
                if (frm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(frm.MaBookingDaChon))
                {
                    _txtMaBookingTraCuu.Text = frm.MaBookingDaChon;
                    await TaiDuLieuHoaDonAsync(frm.MaBookingDaChon);
                }
            };

            grpTraCuu.Controls.AddRange(new Control[] { lblMaBookingPrompt, _txtMaBookingTraCuu, _btnTraCuu, _btnTraCuuNangCao });

            AppTheme.AlignInputAfterLabel(lblMaBookingPrompt, _txtMaBookingTraCuu);
            _btnTraCuu.Left = _txtMaBookingTraCuu.Right + 12;
            _btnTraCuuNangCao.Left = _btnTraCuu.Right + 10;

            // ================= 2. GroupBox: Thông tin lưu trú =================
            var grpThongTin = new GroupBox
            {
                Text = "Thông tin đặt phòng & Khách hàng",
                Location = new Point(20, 85),
                Size = new Size(825, 140),
                Font = AppTheme.FontBodyBold
            };

            var tblTT = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 3,
                Font = AppTheme.FontBody,
                Padding = new Padding(10, 8, 10, 5)
            };
            tblTT.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblTT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tblTT.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblTT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            _lblMaBooking = new Label { Text = "-", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary };
            _lblTrangThaiHienTai = new Label { Text = "-", AutoSize = true, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.StatusWarningText };
            _lblKhachHang = new Label { Text = "-", AutoSize = true };
            _lblSdt = new Label { Text = "-", AutoSize = true };
            _lblThoiGianLuuTru = new Label { Text = "-", AutoSize = true };

            Label TaoNhanTT(string text) => new Label { Text = text, AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };

            tblTT.Controls.Add(TaoNhanTT("Mã booking:"), 0, 0);
            tblTT.Controls.Add(_lblMaBooking, 1, 0);
            tblTT.Controls.Add(TaoNhanTT("Trạng thái thanh toán:"), 2, 0);
            tblTT.Controls.Add(_lblTrangThaiHienTai, 3, 0);

            tblTT.Controls.Add(TaoNhanTT("Khách hàng:"), 0, 1);
            tblTT.Controls.Add(_lblKhachHang, 1, 1);
            tblTT.Controls.Add(TaoNhanTT("Số điện thoại:"), 2, 1);
            tblTT.Controls.Add(_lblSdt, 3, 1);

            tblTT.Controls.Add(TaoNhanTT("Thời gian lưu trú:"), 0, 2);
            tblTT.Controls.Add(_lblThoiGianLuuTru, 1, 2);
            tblTT.SetColumnSpan(_lblThoiGianLuuTru, 3);

            grpThongTin.Controls.Add(tblTT);

            // ================= 3. GroupBox: Chi phí hóa đơn =================
            var grpChiPhi = new GroupBox
            {
                Text = "Chi tiết chi phí hóa đơn & Khuyến mãi",
                Location = new Point(20, 235),
                Size = new Size(825, 200),
                Font = AppTheme.FontBodyBold
            };

            var tblChiPhi = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Font = AppTheme.FontBody,
                Padding = new Padding(20, 8, 20, 8)
            };
            tblChiPhi.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblChiPhi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            _lblTienPhongVal = new Label { Text = "0 đ", AutoSize = true, Font = AppTheme.FontSubHeader, ForeColor = AppTheme.TextPrimary };
            _lblTienDichVuVal = new Label { Text = "0 đ", AutoSize = true, Font = AppTheme.FontSubHeader, ForeColor = AppTheme.TextPrimary };

            _numGiamGia = new NumericUpDown
            {
                Width = 140,
                Minimum = 0,
                Maximum = 1000000000,
                Increment = 10000,
                ThousandsSeparator = true
            };
            AppTheme.ApplyNumericUpDown(_numGiamGia);
            _numGiamGia.ValueChanged += (_, _) => CapNhatTongTien();

            var flpKm = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Margin = new Padding(0)
            };

            _btnMienPhiDichVu = new Button
            {
                Text = "🎁 Miễn phí dịch vụ",
                AutoSize = true,
                Height = 28,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontSmallBold,
                Margin = new Padding(8, 0, 4, 0)
            };
            AppTheme.ApplyPrimaryButton(_btnMienPhiDichVu);

            _btnGiam10 = new Button
            {
                Text = "🏷️ Giảm 10%",
                AutoSize = true,
                Height = 28,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontSmallBold,
                Margin = new Padding(4, 0, 4, 0)
            };
            AppTheme.ApplySecondaryButton(_btnGiam10);

            _btnGiam20 = new Button
            {
                Text = "🏷️ Giảm 20%",
                AutoSize = true,
                Height = 28,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontSmallBold,
                Margin = new Padding(4, 0, 4, 0)
            };
            AppTheme.ApplySecondaryButton(_btnGiam20);

            _btnHuyKm = new Button
            {
                Text = "❌ Xóa KM",
                AutoSize = true,
                Height = 28,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontSmallBold,
                Margin = new Padding(4, 0, 4, 0)
            };
            AppTheme.ApplySecondaryButton(_btnHuyKm);

            _lblGhiChuKhuyenMai = new Label
            {
                Text = string.Empty,
                AutoSize = true,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusSuccessText,
                Margin = new Padding(8, 5, 0, 0)
            };

            flpKm.Controls.AddRange(new Control[] {
                _numGiamGia, _btnMienPhiDichVu, _btnGiam10, _btnGiam20, _btnHuyKm, _lblGhiChuKhuyenMai
            });

            _lblTongCongVal = new Label
            {
                Text = "0 đ",
                AutoSize = true,
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary
            };

            Label TaoNhanChiPhi(string text, bool bold = false) => new Label
            {
                Text = text,
                AutoSize = true,
                Font = bold ? AppTheme.FontBodyBold : AppTheme.FontBody,
                ForeColor = bold ? AppTheme.TextPrimary : AppTheme.TextSecondary
            };

            tblChiPhi.Controls.Add(TaoNhanChiPhi("1. Tiền phòng (tổng các đêm):"), 0, 0);
            tblChiPhi.Controls.Add(_lblTienPhongVal, 1, 0);

            tblChiPhi.Controls.Add(TaoNhanChiPhi("2. Tiền dịch vụ phát sinh:"), 0, 1);
            tblChiPhi.Controls.Add(_lblTienDichVuVal, 1, 1);

            tblChiPhi.Controls.Add(TaoNhanChiPhi("3. Khuyến mãi / Giảm giá (VNĐ):"), 0, 2);
            tblChiPhi.Controls.Add(flpKm, 1, 2);

            tblChiPhi.Controls.Add(TaoNhanChiPhi("TỔNG TIỀN PHẢI TRẢ:", bold: true), 0, 3);
            tblChiPhi.Controls.Add(_lblTongCongVal, 1, 3);

            grpChiPhi.Controls.Add(tblChiPhi);

            // ================= 4. GroupBox: Hình thức thanh toán =================
            var grpHinhThuc = new GroupBox
            {
                Text = "Hình thức thanh toán",
                Location = new Point(20, 445),
                Size = new Size(825, 280),
                Font = AppTheme.FontBodyBold
            };

            _rdoTienMat = new RadioButton
            {
                Text = "Tiền mặt",
                Location = new Point(30, 28),
                AutoSize = true,
                Checked = true,
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.TextPrimary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            _rdoTienMat.CheckedChanged += (_, _) => HinhThucThanhToanChanged();

            _rdoChuyenKhoan = new RadioButton
            {
                Text = "Chuyển khoản (Mã QR ngân hàng tĩnh)",
                Location = new Point(200, 28),
                AutoSize = true,
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.TextPrimary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            _rdoChuyenKhoan.CheckedChanged += (_, _) => HinhThucThanhToanChanged();

            // Panel Tiền mặt
            _pnlTienMat = new Panel
            {
                Location = new Point(25, 65),
                Size = new Size(775, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = AppTheme.Surface
            };

            var lblTienKhachDua = new Label { Text = "Tiền khách đưa:", Location = new Point(15, 22), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary };
            _numTienKhachDua = new NumericUpDown
            {
                Location = new Point(160, 18),
                Width = 220,
                Minimum = 0,
                Maximum = 1000000000,
                Increment = 50000,
                ThousandsSeparator = true
            };
            AppTheme.ApplyNumericUpDown(_numTienKhachDua);
            _numTienKhachDua.Font = AppTheme.FontSubHeader;
            _numTienKhachDua.ValueChanged += (_, _) => TinhTienThua();

            // Gợi ý tiền nhanh
            var btnDuaDu = new Button { Text = "Đưa đủ", Location = new Point(395, 17), Width = 80, Height = 30 };
            AppTheme.ApplySecondaryButton(btnDuaDu);
            btnDuaDu.Click += (_, _) => { _numTienKhachDua.Value = _tongTienSauGiamGia; };

            var btn500k = new Button { Text = "500k", Location = new Point(480, 17), Width = 65, Height = 30 };
            AppTheme.ApplySecondaryButton(btn500k);
            btn500k.Click += (_, _) => { _numTienKhachDua.Value = 500000; };

            var btn1m = new Button { Text = "1.000k", Location = new Point(550, 17), Width = 75, Height = 30 };
            AppTheme.ApplySecondaryButton(btn1m);
            btn1m.Click += (_, _) => { _numTienKhachDua.Value = 1000000; };

            var btn2m = new Button { Text = "2.000k", Location = new Point(630, 17), Width = 75, Height = 30 };
            AppTheme.ApplySecondaryButton(btn2m);
            btn2m.Click += (_, _) => { _numTienKhachDua.Value = 2000000; };

            // Tiền thừa trả khách: dùng FlowLayoutPanel tự động canh chỉnh ngang, không bao giờ bị đè nhãn
            var pnlRowTienThua = new FlowLayoutPanel
            {
                Location = new Point(15, 78),
                Size = new Size(500, 32),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var lblTienThua = new Label
            {
                Text = "Tiền thừa trả khách:",
                AutoSize = true,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Margin = new Padding(0, 4, 8, 0)
            };
            _lblTienThuaVal = new Label
            {
                Text = "0 đ",
                AutoSize = true,
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.StatusSuccessText,
                Margin = new Padding(0)
            };
            pnlRowTienThua.Controls.AddRange(new Control[] { lblTienThua, _lblTienThuaVal });

            var lblGhiChuTienMat = new Label
            {
                Text = "(*) Quy trình: Lễ tân nhận tiền mặt từ khách, đếm tiền và gửi lại tiền thừa cho khách trước khi bấm xác nhận.",
                Location = new Point(15, 140),
                AutoSize = true,
                ForeColor = AppTheme.TextMuted,
                Font = AppTheme.FontCaption
            };

            _pnlTienMat.Controls.AddRange(new Control[] {
                lblTienKhachDua, _numTienKhachDua, btnDuaDu, btn500k, btn1m, btn2m,
                pnlRowTienThua, lblGhiChuTienMat
            });

            // Panel Chuyển khoản QR tĩnh
            _pnlChuyenKhoan = new Panel
            {
                Location = new Point(25, 65),
                Size = new Size(775, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = AppTheme.Surface,
                Visible = false
            };

            _picQr = new PictureBox
            {
                Location = new Point(15, 12),
                Size = new Size(175, 175),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = AppTheme.SurfaceSecondary
            };

            int infoX = 205;

            var lblTieuDeTK = new Label
            {
                Text = "TÀI KHOẢN NGÂN HÀNG THỤ HƯỞNG:",
                Location = new Point(infoX, 10),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary
            };

            var lblNganHang = new Label
            {
                Text = "• Ngân hàng: VIETCOMBANK (Chi nhánh TP.HCM)",
                Location = new Point(infoX, 33),
                AutoSize = true,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary
            };

            var lblSoTK = new Label
            {
                Text = "• Số tài khoản: 0123456789",
                Location = new Point(infoX, 55),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary
            };

            var lblChuTK = new Label
            {
                Text = "• Chủ tài khoản: NGUYEN DUC TRUNG",
                Location = new Point(infoX, 77),
                AutoSize = true,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary
            };

            // Dòng Số tiền cần chuyển: FlowLayoutPanel tự co dãn để giá trị luôn nằm ngay sau nhãn prompt
            var pnlRowSoTien = new FlowLayoutPanel
            {
                Location = new Point(infoX, 100),
                Size = new Size(560, 28),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var lblSoTienPrompt = new Label
            {
                Text = "• Số tiền cần chuyển:",
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Margin = new Padding(0, 3, 8, 0)
            };
            _lblSoTienChuyenKhoanVal = new Label
            {
                Text = "0 đ",
                AutoSize = true,
                Font = AppTheme.FontHeader,
                ForeColor = AppTheme.Primary,
                Margin = new Padding(0)
            };
            pnlRowSoTien.Controls.AddRange(new Control[] { lblSoTienPrompt, _lblSoTienChuyenKhoanVal });

            // Dòng Nội dung chuyển khoản: FlowLayoutPanel tự co dãn
            var pnlRowNoiDung = new FlowLayoutPanel
            {
                Location = new Point(infoX, 129),
                Size = new Size(560, 26),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var lblNoiDungPrompt = new Label
            {
                Text = "• Nội dung chuyển khoản:",
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.TextPrimary,
                Margin = new Padding(0, 2, 8, 0)
            };
            _lblNoiDungChuyenKhoanVal = new Label
            {
                Text = "BK...",
                AutoSize = true,
                Font = AppTheme.FontSubHeader,
                ForeColor = AppTheme.PrimaryActive,
                Margin = new Padding(0)
            };
            pnlRowNoiDung.Controls.AddRange(new Control[] { lblNoiDungPrompt, _lblNoiDungChuyenKhoanVal });

            var lblGhiChuChuyenKhoan = new Label
            {
                Text = "(*) Khách quét QR tĩnh hoặc chuyển khoản đúng số tiền và nội dung.\nLễ tân kiểm tra app ngân hàng, nhận đủ tiền rồi bấm 'Xác nhận thanh toán'.",
                Location = new Point(infoX, 156),
                Size = new Size(555, 40),
                ForeColor = AppTheme.TextMuted,
                Font = AppTheme.FontCaption
            };

            _pnlChuyenKhoan.Controls.AddRange(new Control[] {
                _picQr, lblTieuDeTK, lblNganHang, lblSoTK, lblChuTK,
                pnlRowSoTien, pnlRowNoiDung, lblGhiChuChuyenKhoan
            });

            grpHinhThuc.Controls.AddRange(new Control[] { _rdoTienMat, _rdoChuyenKhoan, _pnlTienMat, _pnlChuyenKhoan });

            // ================= 5. Panel Nút bấm =================
            var pnlActions = new Panel
            {
                Location = new Point(20, 715),
                Size = new Size(825, 60)
            };

            _btnXuatPdf = new Button
            {
                Text = "📄 Xuất hóa đơn PDF",
                Location = new Point(215, 8),
                Size = new Size(205, 44),
                Enabled = false
            };
            AppTheme.ApplySecondaryButton(_btnXuatPdf);
            _btnXuatPdf.Click += async (_, _) => await XuatHoaDonPdfAsync();

            _btnXacNhan = new Button
            {
                Text = "Xác nhận thanh toán",
                Location = new Point(430, 8),
                Size = new Size(265, 44),
                Enabled = false
            };
            AppTheme.ApplyPrimaryButton(_btnXacNhan);
            _btnXacNhan.Click += async (_, _) => await XacNhanThanhToanAsync();

            _btnHuy = new Button
            {
                Text = "Đóng",
                Location = new Point(705, 8),
                Size = new Size(110, 44)
            };
            AppTheme.ApplySecondaryButton(_btnHuy);
            _btnHuy.Click += (_, _) => Close();

            _btnHuyPdf = new Button
            {
                Text = "⛔ Hủy",
                Location = new Point(100, 8),
                Size = new Size(105, 44),
                Visible = false
            };
            AppTheme.ApplyDangerButton(_btnHuyPdf);

            pnlActions.Controls.AddRange(new Control[] { _btnHuyPdf, _btnXuatPdf, _btnXacNhan, _btnHuy });

            Controls.AddRange(new Control[] { grpTraCuu, grpThongTin, grpChiPhi, grpHinhThuc, pnlActions });
        }

        #endregion
    }
}
