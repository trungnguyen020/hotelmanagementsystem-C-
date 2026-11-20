using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormDatPhongKhachHang
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

        private Panel pnlContainer = null!;
        private Panel pnlTopArea = null!;
        private GroupBox grpThongTinKhach = null!;
        private Label lblHoTenKhach = null!;
        private Label lblCccdKhach = null!;
        private Label lblSdtKhach = null!;
        private Label lblEmailKhach = null!;

        private GroupBox grpBoLoc = null!;
        private Label lblNgayNhan = null!;
        private DateTimePicker dtpNgayNhan = null!;
        private Label lblNgayTra = null!;
        private DateTimePicker dtpNgayTra = null!;
        private Label lblSoKhach = null!;
        private NumericUpDown numSoKhach = null!;
        private Label lblLoaiPhong = null!;
        private ComboBox cboLoaiPhong = null!;
        private Button btnTimPhong = null!;

        private GroupBox grpDanhSachPhong = null!;
        private DataGridView dgvPhongTrong = null!;
        private Panel pnlCenterContent = null!;
        private GroupBox grpDichVu = null!;
        private DataGridView dgvDichVu = null!;
        private Panel pnlBottomAction = null!;
        private Panel pnlInfoLeft = null!;
        private Label lblTongTienDuKien = null!;
        private Label lblThongBao = null!;
        private Button btnXacNhanDat = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(1100, 720);
            AppTheme.ApplyFormStyle(this);

            pnlContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(16, 10, 16, 12),
                AutoScroll = true
            };

            // ================= KHỐI TRÊN CÙNG: THÔNG TIN KHÁCH & BỘ LỌC TÌM PHÒNG =================
            pnlTopArea = new Panel
            {
                Dock = DockStyle.Top,
                Height = 175,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            // 1. Nhóm thông tin khách hàng (Read-only)
            grpThongTinKhach = new GroupBox
            {
                Text = "Thông tin khách hàng đặt phòng (Tài khoản hiện tại)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 6, 12, 6)
            };

            lblHoTenKhach = new Label
            {
                Text = "Họ tên: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 28),
                AutoSize = true
            };

            lblCccdKhach = new Label
            {
                Text = "CCCD: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(260, 28),
                AutoSize = true
            };

            lblSdtKhach = new Label
            {
                Text = "SĐT: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(500, 28),
                AutoSize = true
            };

            lblEmailKhach = new Label
            {
                Text = "Email: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(720, 28),
                AutoSize = true
            };

            grpThongTinKhach.Controls.AddRange(new Control[]
            {
                lblHoTenKhach,
                lblCccdKhach,
                lblSdtKhach,
                lblEmailKhach
            });

            var pnlTopSpacer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 8,
                BackColor = Color.Transparent
            };

            // 2. Nhóm bộ lọc tìm phòng
            grpBoLoc = new GroupBox
            {
                Text = "Thời gian lưu trú & Yêu cầu phòng",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 6, 12, 6)
            };

            lblNgayNhan = new Label
            {
                Text = "Ngày nhận:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 22),
                AutoSize = true
            };
            dtpNgayNhan = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(16, 46),
                Size = new Size(140, 28),
                MinDate = DateTime.Today,
                Value = DateTime.Today
            };
            AppTheme.ApplyDateTimePicker(dtpNgayNhan);

            lblNgayTra = new Label
            {
                Text = "Ngày trả:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(175, 22),
                AutoSize = true
            };
            dtpNgayTra = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(175, 46),
                Size = new Size(140, 28),
                MinDate = DateTime.Today.AddDays(1),
                Value = DateTime.Today.AddDays(1)
            };
            AppTheme.ApplyDateTimePicker(dtpNgayTra);

            lblSoKhach = new Label
            {
                Text = "Số lượng khách:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(335, 22),
                AutoSize = true
            };
            numSoKhach = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 50,
                Value = 1,
                Location = new Point(335, 46),
                Size = new Size(110, 28)
            };
            AppTheme.ApplyNumericUpDown(numSoKhach);

            lblLoaiPhong = new Label
            {
                Text = "Loại phòng:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(465, 22),
                AutoSize = true
            };
            cboLoaiPhong = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(465, 46),
                Size = new Size(190, 28)
            };
            AppTheme.ApplyComboBox(cboLoaiPhong);

            btnTimPhong = new Button
            {
                Text = "🔍 Tìm phòng",
                Location = new Point(675, 42),
                Size = new Size(160, 34),
                Cursor = Cursors.Hand
            };
            AppTheme.ApplySecondaryButton(btnTimPhong);
            btnTimPhong.Click += async (s, e) => await TimPhongTrongAsync();

            grpBoLoc.Controls.AddRange(new Control[]
            {
                lblNgayNhan, dtpNgayNhan,
                lblNgayTra, dtpNgayTra,
                lblSoKhach, numSoKhach,
                lblLoaiPhong, cboLoaiPhong,
                btnTimPhong
            });

            pnlTopArea.Controls.Add(grpBoLoc);
            pnlTopArea.Controls.Add(pnlTopSpacer);
            pnlTopArea.Controls.Add(grpThongTinKhach);

            var pnlMidSpacer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 10,
                BackColor = Color.Transparent
            };

            // ================= 3. Khối trung tâm: Danh sách phòng trống & Dịch vụ kèm theo =================
            pnlCenterContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            // 3.1. Nhóm dịch vụ kèm theo (Bên phải, Width = 380)
            grpDichVu = new GroupBox
            {
                Text = "Dịch vụ kèm theo (Tùy chọn)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Right,
                Width = 380,
                BackColor = AppTheme.Surface,
                Padding = new Padding(8, 8, 8, 8)
            };

            dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(dgvDichVu);
            grpDichVu.Controls.Add(dgvDichVu);

            var pnlCenterSplitter = new Panel
            {
                Dock = DockStyle.Right,
                Width = 10,
                BackColor = Color.Transparent
            };

            // 3.2. Nhóm danh sách phòng trống (Bên trái, Dock Fill)
            grpDanhSachPhong = new GroupBox
            {
                Text = "Danh sách phòng trống khả dụng (Chọn 1 hoặc nhiều phòng để đặt)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 8, 12, 10)
            };

            // Bảng danh sách phòng trống
            dgvPhongTrong = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(dgvPhongTrong);
            grpDanhSachPhong.Controls.Add(dgvPhongTrong);

            pnlCenterContent.Controls.Add(grpDanhSachPhong);
            pnlCenterContent.Controls.Add(pnlCenterSplitter);
            pnlCenterContent.Controls.Add(grpDichVu);
            grpDanhSachPhong.BringToFront();

            // ================= 4. Thanh thao tác dưới đáy: Luôn luôn hiển thị rõ ràng trên màn hình =================
            pnlBottomAction = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = AppTheme.Surface,
                Padding = new Padding(0, 6, 0, 0)
            };

            btnXacNhanDat = new Button
            {
                Text = "✔ XÁC NHẬN ĐẶT PHÒNG",
                Dock = DockStyle.Right,
                Width = 260,
                Height = 48,
                Cursor = Cursors.Hand,
                Font = AppTheme.FontBodyBold
            };
            AppTheme.ApplyPrimaryButton(btnXacNhanDat);
            btnXacNhanDat.Click += BtnXacNhanDat_Click;

            pnlInfoLeft = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(4, 2, 4, 2)
            };

            lblTongTienDuKien = new Label
            {
                Text = "Đã chọn: 0 phòng | Ước tính: 0 VNĐ",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(4, 6),
                AutoSize = true
            };

            lblThongBao = new Label
            {
                Text = string.Empty,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                Location = new Point(4, 30),
                AutoSize = true
            };

            pnlInfoLeft.Controls.Add(lblTongTienDuKien);
            pnlInfoLeft.Controls.Add(lblThongBao);

            pnlBottomAction.Controls.Add(pnlInfoLeft);
            pnlBottomAction.Controls.Add(btnXacNhanDat);

            var pnlBottomSpacer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 8,
                BackColor = Color.Transparent
            };

            // Ghép toàn bộ vào pnlContainer
            pnlContainer.Controls.Add(pnlCenterContent);
            pnlContainer.Controls.Add(pnlBottomSpacer);
            pnlContainer.Controls.Add(pnlBottomAction);
            pnlContainer.Controls.Add(pnlMidSpacer);
            pnlContainer.Controls.Add(pnlTopArea);
            pnlCenterContent.BringToFront();

            this.Controls.Add(pnlContainer);
        }

        #endregion
    }
}

