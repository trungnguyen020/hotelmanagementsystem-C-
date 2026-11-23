using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    partial class FormPhong
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
        private Button _btnImportExcel = null!;
        private Button _btnXuatExcel = null!;
        private Button _btnThemMoi = null!;
        private DataGridView _dgv = null!;

        private DataGridViewTextBoxColumn _colSoPhong = null!;
        private DataGridViewTextBoxColumn _colTenLoai = null!;
        private DataGridViewTextBoxColumn _colTrangThai = null!;
        private DataGridViewImageColumn _colEdit = null!;
        private DataGridViewImageColumn _colDelete = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý phòng";
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
                PlaceholderText = "🔍 Tìm số phòng, loại phòng...",
                Location = new Point(16, 14),
                Width = 320,
                Height = 32
            };
            AppTheme.ApplyTextBox(_txtTimKiem);

            _btnImportExcel = new Button
            {
                Text = "📥 Nhập Excel",
                Size = new Size(120, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.ApplySecondaryButton(_btnImportExcel);

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
                _btnImportExcel.Location = new Point(_btnXuatExcel.Left - _btnImportExcel.Width - 12, 13);
            }
            _pnlTop.Resize += (s, e) => CanChinhTopNut();
            CanChinhTopNut();

            _pnlTop.Controls.AddRange(new Control[] { _txtTimKiem, _btnImportExcel, _btnXuatExcel, _btnThemMoi });

            // ================= 2. DataGridView =================
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
                BackgroundColor = AppTheme.Surface
            };
            AppTheme.ApplyDataGridView(_dgv);
            _dgv.RowTemplate.Height = 36;
            _dgv.ColumnHeadersHeight = 40;

            _colSoPhong = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoPhong",
                HeaderText = "Số phòng",
                Width = 140,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = AppTheme.FontBodyBold }
            };

            _colTenLoai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenLoaiPhong",
                HeaderText = "Loại phòng",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 160
            };

            _colTrangThai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TrangThai",
                HeaderText = "Trạng thái",
                Width = 160,
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
                _colSoPhong, _colTenLoai, _colTrangThai, _colEdit, _colDelete
            });

            Controls.Add(_dgv);
            Controls.Add(_pnlTop);
        }

        #endregion
    }
}
