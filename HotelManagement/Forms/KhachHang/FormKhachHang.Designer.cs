using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    partial class FormKhachHang
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

        private Panel _pnlTop = null!;
        private TextBox _txtTimKiem = null!;
        private Button _btnDatPhong = null!;
        private Button _btnXuatExcel = null!;
        private Button _btnThemMoi = null!;

        private Panel _pnlBottom = null!;
        private Label _lblTaiKhoanInfo = null!;
        private Button _btnDatLaiMk = null!;
        private Button _btnKhoaMoKhoa = null!;

        private DataGridView _dgv = null!;

        private DataGridViewTextBoxColumn _colMaKH = null!;
        private DataGridViewTextBoxColumn _colTen = null!;
        private DataGridViewTextBoxColumn _colSdt = null!;
        private DataGridViewTextBoxColumn _colCccd = null!;
        private DataGridViewTextBoxColumn _colEmail = null!;
        private DataGridViewTextBoxColumn _colGioiTinh = null!;
        private DataGridViewTextBoxColumn _colNgaySinh = null!;
        private DataGridViewTextBoxColumn _colTaiKhoan = null!;
        private DataGridViewTextBoxColumn _colTrangThaiTK = null!;
        private DataGridViewImageColumn _colEdit = null!;
        private DataGridViewImageColumn _colDelete = null!;

        private ContextMenuStrip _ctxMenu = null!;
        private ToolStripMenuItem _mnuDatPhong = null!;
        private ToolStripMenuItem _mnuDatLaiMk = null!;
        private ToolStripMenuItem _mnuKhoaMoKhoa = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý khách hàng";
            Width = 1150;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(950, 600);

            // ================= 1. Top Bar =================
            _pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(16, 12, 16, 12),
                BackColor = AppTheme.Surface
            };

            _txtTimKiem = new TextBox
            {
                PlaceholderText = "🔍 Tìm tên, SĐT, CCCD...",
                Location = new Point(16, 14),
                Width = 320,
                Height = 32
            };
            AppTheme.ApplyTextBox(_txtTimKiem);

            _btnDatPhong = new Button
            {
                Text = "🏨 Đặt phòng",
                Size = new Size(120, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplyPrimaryButton(_btnDatPhong);

            _btnXuatExcel = new Button
            {
                Text = "Xuất file Excel",
                Size = new Size(130, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplyExcelButton(_btnXuatExcel);

            _btnThemMoi = new Button
            {
                Text = "+ Thêm mới",
                Size = new Size(120, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplyAddButton(_btnThemMoi);

            void CanChinhTopNut()
            {
                _btnThemMoi.Location = new Point(_pnlTop.ClientSize.Width - _btnThemMoi.Width - 16, 13);
                _btnXuatExcel.Location = new Point(_btnThemMoi.Left - _btnXuatExcel.Width - 12, 13);
                _btnDatPhong.Location = new Point(_btnXuatExcel.Left - _btnDatPhong.Width - 12, 13);
            }
            _pnlTop.Resize += (s, e) => CanChinhTopNut();
            CanChinhTopNut();

            _pnlTop.Controls.AddRange(new Control[] { _txtTimKiem, _btnDatPhong, _btnXuatExcel, _btnThemMoi });

            // ================= 2. Bottom Bar (Quản lý tài khoản) =================
            _pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                Padding = new Padding(16, 10, 16, 10),
                BackColor = AppTheme.Surface
            };

            _lblTaiKhoanInfo = new Label
            {
                Text = "Chưa chọn khách hàng",
                Location = new Point(16, 17),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary
            };

            _btnDatLaiMk = new Button
            {
                Text = "🔑 Đặt lại MK (123456)",
                Size = new Size(175, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            AppTheme.ApplySecondaryButton(_btnDatLaiMk);

            _btnKhoaMoKhoa = new Button
            {
                Text = "🔒 Khóa tài khoản",
                Size = new Size(190, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            AppTheme.ApplySecondaryButton(_btnKhoaMoKhoa);

            void CanChinhBottomNut()
            {
                _btnKhoaMoKhoa.Location = new Point(_pnlBottom.ClientSize.Width - _btnKhoaMoKhoa.Width - 16, 11);
                _btnDatLaiMk.Location = new Point(_btnKhoaMoKhoa.Left - _btnDatLaiMk.Width - 10, 11);
            }
            _pnlBottom.Resize += (s, e) => CanChinhBottomNut();
            CanChinhBottomNut();

            _pnlBottom.Controls.AddRange(new Control[] { _lblTaiKhoanInfo, _btnDatLaiMk, _btnKhoaMoKhoa });

            // ================= 3. Context Menu =================
            _ctxMenu = new ContextMenuStrip();
            _mnuDatPhong = new ToolStripMenuItem("🏨 Đặt phòng cho khách này");
            _mnuDatLaiMk = new ToolStripMenuItem("🔑 Đặt lại mật khẩu (về 123456)");
            _mnuKhoaMoKhoa = new ToolStripMenuItem("🔒 Khóa / Mở khóa tài khoản");
            _ctxMenu.Items.AddRange(new ToolStripItem[] { _mnuDatPhong, new ToolStripSeparator(), _mnuDatLaiMk, _mnuKhoaMoKhoa });

            // ================= 4. DataGridView =================
            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                ContextMenuStrip = _ctxMenu,
                BackgroundColor = AppTheme.Surface
            };
            AppTheme.ApplyDataGridView(_dgv);
            _dgv.RowTemplate.Height = 36;
            _dgv.ColumnHeadersHeight = 40;

            _colMaKH = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IdKhachHang",
                HeaderText = "Mã KH",
                Width = 80,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colTen = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ten",
                HeaderText = "Họ và tên",
                Width = 180,
                DefaultCellStyle = { Font = AppTheme.FontBodyBold }
            };

            _colSdt = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Sdt",
                HeaderText = "Số điện thoại",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colCccd = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cccd",
                HeaderText = "CCCD",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colEmail = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 140
            };

            _colGioiTinh = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GioiTinh",
                HeaderText = "Giới tính",
                Width = 85,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colNgaySinh = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NgaySinh",
                HeaderText = "Ngày sinh",
                Width = 100,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "dd/MM/yyyy" }
            };

            _colTaiKhoan = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenTaiKhoan",
                HeaderText = "Tài khoản",
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colTrangThaiTK = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThaiTaiKhoan",
                HeaderText = "TT Tài khoản",
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colEdit = new DataGridViewImageColumn
            {
                HeaderText = "Cập nhật",
                Image = EmbeddedImages.EditSmall,
                Width = 70,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Description = "Sửa",
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colDelete = new DataGridViewImageColumn
            {
                HeaderText = "Xóa",
                Image = EmbeddedImages.DeleteSmall,
                Width = 60,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Description = "Xóa",
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                _colMaKH, _colTen, _colSdt, _colCccd, _colEmail,
                _colGioiTinh, _colNgaySinh, _colTaiKhoan, _colTrangThaiTK,
                _colEdit, _colDelete
            });

            Controls.Add(_dgv);
            Controls.Add(_pnlBottom);
            Controls.Add(_pnlTop);
        }

        #endregion
    }
}
