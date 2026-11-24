using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    partial class FormDichVu
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
        private DataGridView _dgv = null!;

        private DataGridViewTextBoxColumn _colMaDV = null!;
        private DataGridViewTextBoxColumn _colTenDV = null!;
        private DataGridViewTextBoxColumn _colLoaiDV = null!;
        private DataGridViewTextBoxColumn _colGia = null!;
        private DataGridViewImageColumn _colEdit = null!;
        private DataGridViewImageColumn _colDelete = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý dịch vụ";
            Width = 1050;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(880, 580);

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
                PlaceholderText = "🔍 Tìm kiếm dịch vụ...",
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

            _colMaDV = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IdDichVu",
                HeaderText = "Mã DV",
                Width = 90,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colTenDV = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenDichVu",
                HeaderText = "Tên dịch vụ",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 180
            };

            _colLoaiDV = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LoaiDichVu",
                HeaderText = "Loại dịch vụ",
                Width = 140,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft }
            };

            _colGia = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MucGia",
                HeaderText = "Đơn giá (VNĐ)",
                Width = 160,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
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
                _colMaDV, _colTenDV, _colLoaiDV, _colGia, _colEdit, _colDelete
            });

            Controls.Add(_dgv);
            Controls.Add(_pnlTop);
        }

        #endregion
    }
}
