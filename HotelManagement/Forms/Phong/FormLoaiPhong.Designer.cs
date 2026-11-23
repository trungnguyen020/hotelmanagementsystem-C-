using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HotelManagement.Common;

namespace HotelManagement.Forms
{
    partial class FormLoaiPhong
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

        private DataGridViewTextBoxColumn _colMaLoai = null!;
        private DataGridViewTextBoxColumn _colTenLoai = null!;
        private DataGridViewTextBoxColumn _colSoGiuong = null!;
        private DataGridViewTextBoxColumn _colSucChua = null!;
        private DataGridViewTextBoxColumn _colGiaNgay = null!;
        private DataGridViewTextBoxColumn _colGiaGio = null!;
        private DataGridViewTextBoxColumn _colMoTa = null!;
        private DataGridViewImageColumn _colEdit = null!;
        private DataGridViewImageColumn _colDelete = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Quản lý loại phòng";
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
                PlaceholderText = "🔍 Tìm mã hoặc tên...",
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

            _colMaLoai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaLoai",
                HeaderText = "Mã loại",
                Width = 90,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colTenLoai = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenLoaiPhong",
                HeaderText = "Tên loại",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 140
            };

            _colSoGiuong = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoGiuong",
                HeaderText = "Số giường",
                Width = 95,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colSucChua = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SucChua",
                HeaderText = "Số người tối đa",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            _colGiaNgay = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Gia",
                HeaderText = "Giá ngày",
                Width = 130,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            };

            _colGiaGio = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GiaTheoGio",
                HeaderText = "Giá giờ",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            };

            _colMoTa = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MoTa",
                HeaderText = "Mô tả",
                Width = 180
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
                _colMaLoai, _colTenLoai, _colSoGiuong, _colSucChua,
                _colGiaNgay, _colGiaGio, _colMoTa, _colEdit, _colDelete
            });

            Controls.Add(_dgv);
            Controls.Add(_pnlTop);
        }

        #endregion
    }
}
