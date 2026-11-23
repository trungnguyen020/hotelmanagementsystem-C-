using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    partial class FormNhanSu
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
        private Button _btnXuatExcel = null!;
        private Button _btnThemMoi = null!;

        private Panel _pnlBottom = null!;
        private Label _lblTaiKhoanInfo = null!;
        private Button _btnDatLaiMk = null!;

        private DataGridView _dgv = null!;

        private DataGridViewTextBoxColumn _colMaNV = null!;
        private DataGridViewTextBoxColumn _colHoTen = null!;
        private DataGridViewTextBoxColumn _colCccd = null!;
        private DataGridViewTextBoxColumn _colSdt = null!;
        private DataGridViewTextBoxColumn _colNgaySinh = null!;
        private DataGridViewTextBoxColumn _colTaiKhoan = null!;
        private DataGridViewTextBoxColumn _colTrangThaiTK = null!;
        private DataGridViewImageColumn _colEdit = null!;
        private DataGridViewImageColumn _colDelete = null!;

        private ContextMenuStrip _ctxMenu = null!;
        private ToolStripMenuItem _mnuDatLaiMk = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý nhân sự";
            Width = 1150;
            Height = 760;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(920, 620);

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
                PlaceholderText = "🔍 Tìm tên, CCCD, SĐT...",
                Location = new Point(16, 14),
                Width = 320,
                Height = 32
            };
            AppTheme.ApplyTextBox(_txtTimKiem);

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
            }
            _pnlTop.Resize += (s, e) => CanChinhTopNut();
            CanChinhTopNut();

            _pnlTop.Controls.AddRange(new Control[] { _txtTimKiem, _btnXuatExcel, _btnThemMoi });

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
                Text = "Chưa chọn nhân sự",
                Location = new Point(16, 17),
                AutoSize = true,
                Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary
            };

            _btnDatLaiMk = new Button
            {
                Text = "🔑 Đặt lại MK (letan123)",
                Size = new Size(185, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            AppTheme.ApplySecondaryButton(_btnDatLaiMk);

            void CanChinhBottomNut()
            {
                _btnDatLaiMk.Location = new Point(_pnlBottom.ClientSize.Width - _btnDatLaiMk.Width - 16, 11);
            }
            _pnlBottom.Resize += (s, e) => CanChinhBottomNut();
            CanChinhBottomNut();

            _pnlBottom.Controls.AddRange(new Control[] { _lblTaiKhoanInfo, _btnDatLaiMk });

            // ================= 3. Context Menu =================
            _ctxMenu = new ContextMenuStrip();
            _mnuDatLaiMk = new ToolStripMenuItem("🔑 Đặt lại mật khẩu (về letan123)");
            _ctxMenu.Items.Add(_mnuDatLaiMk);

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

            _colMaNV = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IdNhanSu",
                HeaderText = "Mã NV",
                Width = 80,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colHoTen = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HoTen",
                HeaderText = "Họ và tên",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 160,
                DefaultCellStyle = { Font = AppTheme.FontBodyBold }
            };

            _colCccd = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cccd",
                HeaderText = "CCCD",
                Width = 130,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colSdt = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Sdt",
                HeaderText = "Số điện thoại",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colNgaySinh = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NgaySinh",
                HeaderText = "Ngày sinh",
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "dd/MM/yyyy" }
            };

            _colTaiKhoan = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenTaiKhoanHienThi",
                HeaderText = "Tài khoản",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colTrangThaiTK = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThaiTaiKhoan",
                HeaderText = "Trạng thái",
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
                _colMaNV, _colHoTen, _colCccd, _colSdt, _colNgaySinh,
                _colTaiKhoan, _colTrangThaiTK, _colEdit, _colDelete
            });

            Controls.Add(_dgv);
            Controls.Add(_pnlBottom);
            Controls.Add(_pnlTop);
        }

        #endregion
    }
}
