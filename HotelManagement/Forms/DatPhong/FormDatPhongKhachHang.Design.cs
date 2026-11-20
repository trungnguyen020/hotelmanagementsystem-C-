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
        private Label lblTongTienDuKien = null!;
        private Label lblThongBao = null!;
        private Button btnXacNhanDat = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            pnlContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(16),
                AutoScroll = true
            };

            // ================= 1. Nhóm thông tin khách hàng (Read-only) =================
            grpThongTinKhach = new GroupBox
            {
                Text = "Thông tin khách hàng đặt phòng (Tài khoản hiện tại)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 12),
                Size = new Size(1020, 80),
                BackColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            lblHoTenKhach = new Label
            {
                Text = "Họ tên: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 32),
                AutoSize = true
            };

            lblCccdKhach = new Label
            {
                Text = "CCCD: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(280, 32),
                AutoSize = true
            };

            lblSdtKhach = new Label
            {
                Text = "SĐT: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(540, 32),
                AutoSize = true
            };

            lblEmailKhach = new Label
            {
                Text = "Email: Đang tải...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(740, 32),
                AutoSize = true
            };

            grpThongTinKhach.Controls.AddRange(new Control[]
            {
                lblHoTenKhach,
                lblCccdKhach,
                lblSdtKhach,
                lblEmailKhach
            });

            // ================= 2. Nhóm bộ lọc tìm phòng =================
            grpBoLoc = new GroupBox
            {
                Text = "Thời gian lưu trú & Yêu cầu phòng",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 102),
                Size = new Size(1020, 95),
                BackColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            lblNgayNhan = new Label
            {
                Text = "Ngày nhận:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 28),
                AutoSize = true
            };
            dtpNgayNhan = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(16, 52),
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
                Location = new Point(175, 28),
                AutoSize = true
            };
            dtpNgayTra = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(175, 52),
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
                Location = new Point(335, 28),
                AutoSize = true
            };
            numSoKhach = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 50,
                Value = 1,
                Location = new Point(335, 52),
                Size = new Size(120, 28)
            };
            AppTheme.ApplyNumericUpDown(numSoKhach);

            lblLoaiPhong = new Label
            {
                Text = "Loại phòng:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(475, 28),
                AutoSize = true
            };
            cboLoaiPhong = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(475, 52),
                Size = new Size(200, 28)
            };
            AppTheme.ApplyComboBox(cboLoaiPhong);

            btnTimPhong = new Button
            {
                Text = "🔍 Tìm phòng trống",
                Location = new Point(700, 48),
                Size = new Size(170, 34),
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

            // ================= 3. Nhóm danh sách phòng trống & Đặt phòng =================
            grpDanhSachPhong = new GroupBox
            {
                Text = "Danh sách phòng trống khả dụng (Chọn 1 hoặc nhiều phòng để đặt)",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 208),
                Size = new Size(1020, 390),
                BackColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            dgvPhongTrong = new DataGridView
            {
                Location = new Point(16, 28),
                Size = new Size(988, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            AppTheme.ApplyDataGridView(dgvPhongTrong);

            lblTongTienDuKien = new Label
            {
                Text = "Đã chọn: 0 phòng | Ước tính: 0 VNĐ",
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary,
                Location = new Point(16, 320),
                Size = new Size(400, 28),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            lblThongBao = new Label
            {
                Text = string.Empty,
                Font = AppTheme.FontSmallBold,
                ForeColor = AppTheme.StatusDangerText,
                Location = new Point(16, 350),
                Size = new Size(600, 28),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnXacNhanDat = new Button
            {
                Text = "✔ XÁC NHẬN ĐẶT PHÒNG",
                Location = new Point(780, 325),
                Size = new Size(224, 46),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyPrimaryButton(btnXacNhanDat);
            btnXacNhanDat.Click += BtnXacNhanDat_Click;

            grpDanhSachPhong.Controls.AddRange(new Control[]
            {
                dgvPhongTrong,
                lblTongTienDuKien,
                lblThongBao,
                btnXacNhanDat
            });

            pnlContainer.Controls.AddRange(new Control[]
            {
                grpThongTinKhach,
                grpBoLoc,
                grpDanhSachPhong
            });

            this.Controls.Add(pnlContainer);
        }

        #endregion
    }
}

