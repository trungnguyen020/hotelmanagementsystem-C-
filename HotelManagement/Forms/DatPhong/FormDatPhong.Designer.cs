using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManagement.Forms
{
    partial class FormDatPhong
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

        private DateTimePicker _dtpNgayNhan = null!;
        private DateTimePicker _dtpNgayTra = null!;
        private ComboBox _cboLoaiPhong = null!;
        private NumericUpDown _numSoKhach = null!;
        private Button _btnTimPhong = null!;

        private TextBox _txtTimKhachHang = null!;
        private Button _btnTimKhachHang = null!;
        private Label _lblThongTinKhachHang = null!;

        private DataGridView _dgvPhongTrong = null!;
        private DataGridView _dgvPhongChon = null!;
        private Label _lblTongTien = null!;
        private Button _btnDatPhong = null!;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            AppTheme.ApplyFormStyle(this);

            Text = "Đặt phòng";
            Size = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;

            // --- Pannel Filter ---
            var pnlFilter = new GroupBox { Text = "Tìm phòng trống", Dock = DockStyle.Top, Height = 80, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.Primary };
            
            var lblNgayNhan = new Label { Text = "Ngày nhận:", AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary };
            _dtpNgayNhan = new DateTimePicker { Width = 130, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today };
            AppTheme.ApplyDateTimePicker(_dtpNgayNhan);
            
            var lblNgayTra = new Label { Text = "Ngày trả:", AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary };
            _dtpNgayTra = new DateTimePicker { Width = 130, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today.AddDays(1) };
            _dtpNgayTra.Value = DateTime.Today.AddDays(1);
            AppTheme.ApplyDateTimePicker(_dtpNgayTra);

            var lblLoaiPhong = new Label { Text = "Loại phòng:", AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary };
            _cboLoaiPhong = new ComboBox { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            AppTheme.ApplyComboBox(_cboLoaiPhong);
            
            var lblSoKhach = new Label { Text = "Số khách:", AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary };
            _numSoKhach = new NumericUpDown { Width = 65, Minimum = 1, Maximum = 50, Value = 1 };
            AppTheme.ApplyNumericUpDown(_numSoKhach);
            
            _btnTimPhong = new Button { Text = "🔍 Tìm phòng", Width = 120, Height = 34 };
            AppTheme.ApplyPrimaryButton(_btnTimPhong);
            _btnTimPhong.Click += async (_, _) => await TimPhongAsync();

            void CanChinhFilter()
            {
                lblNgayNhan.Location = new Point(16, 34);
                _dtpNgayNhan.Location = new Point(lblNgayNhan.Right + 8, 30);

                lblNgayTra.Location = new Point(_dtpNgayNhan.Right + 18, 34);
                _dtpNgayTra.Location = new Point(lblNgayTra.Right + 8, 30);

                lblLoaiPhong.Location = new Point(_dtpNgayTra.Right + 18, 34);
                _cboLoaiPhong.Location = new Point(lblLoaiPhong.Right + 8, 30);

                lblSoKhach.Location = new Point(_cboLoaiPhong.Right + 18, 34);
                _numSoKhach.Location = new Point(lblSoKhach.Right + 8, 30);

                _btnTimPhong.Location = new Point(_numSoKhach.Right + 20, 28);
            }
            pnlFilter.Resize += (_, _) => CanChinhFilter();

            pnlFilter.Controls.AddRange(new Control[] {
                lblNgayNhan, _dtpNgayNhan, lblNgayTra, _dtpNgayTra,
                lblLoaiPhong, _cboLoaiPhong, lblSoKhach, _numSoKhach, _btnTimPhong
            });
            CanChinhFilter();

            // --- Panel Khách Hàng ---
            var pnlKhachHang = new GroupBox { Text = "Khách hàng", Dock = DockStyle.Left, Width = 300, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.Primary };
            
            var lblTimKH = new Label { Text = "Nhập CCCD / SĐT:", Location = new Point(14, 28), AutoSize = true, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary };
            _txtTimKhachHang = new TextBox { Location = new Point(14, 52), Width = 175 };
            AppTheme.ApplyTextBox(_txtTimKhachHang);
            _btnTimKhachHang = new Button { Text = "Tra cứu", Location = new Point(198, 50), Width = 85, Height = 30 };
            AppTheme.ApplySecondaryButton(_btnTimKhachHang);
            _btnTimKhachHang.Click += async (_, _) => await TimKhachHangAsync();

            _lblThongTinKhachHang = new Label 
            { 
                Text = "(Chưa chọn khách hàng)", 
                Location = new Point(10, 90), 
                AutoSize = true, 
                MaximumSize = new Size(280, 0)
            };

            var btnThemKH = new Button { Text = "Chọn khách hàng...", Location = new Point(10, 200), Width = 160, Height = 30 };
            btnThemKH.Click += (_, _) =>
            {
                using var frmChon = new FormChonKhachHang();
                if (frmChon.ShowDialog(this) == DialogResult.OK && frmChon.KhachHangDaChon != null)
                {
                    _khachHangDuocChon = frmChon.KhachHangDaChon;
                    _lblThongTinKhachHang.Text =
                        $"Họ tên: {_khachHangDuocChon.Ten}\nCCCD: {_khachHangDuocChon.Cccd}\nSĐT: {_khachHangDuocChon.Sdt}";
                }
            };

            pnlKhachHang.Controls.AddRange(new Control[] { lblTimKH, _txtTimKhachHang, _btnTimKhachHang, _lblThongTinKhachHang, btnThemKH });

            // --- Center Panel ---
            var pnlCenter = new Panel { Dock = DockStyle.Fill };
            
            var grpPhongTrong = new GroupBox { Text = "Danh sách phòng trống", Dock = DockStyle.Top, Height = 300 };
            _dgvPhongTrong = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            var btnChonPhong = new Button { Text = "Thêm phòng đã chọn ↓", Dock = DockStyle.Bottom, Height = 30 };
            btnChonPhong.Click += BtnChonPhong_Click;

            grpPhongTrong.Controls.Add(_dgvPhongTrong);
            grpPhongTrong.Controls.Add(btnChonPhong);

            var grpPhongChon = new GroupBox { Text = "Phòng đã chọn để đặt", Dock = DockStyle.Fill };
            _dgvPhongChon = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            _dgvPhongChon.DataSource = _dsPhongChon;
            
            var pnlDatPhong = new Panel { Dock = DockStyle.Bottom, Height = 95 };
            var btnXoaPhongChon = new Button { Text = "Bỏ chọn ↑", Location = new Point(20, 10), Width = 130, Height = 32 };
            btnXoaPhongChon.Click += BtnXoaPhongChon_Click;

            _btnDatPhong = new Button 
            { 
                Text = "ĐẶT PHÒNG", 
                Location = new Point(20, 48), 
                Width = 130, 
                Height = 38
            };
            _btnDatPhong.Click += async (_, _) => await DatPhongAsync();

            _lblTongTien = new Label 
            { 
                Text = "Tổng tiền tạm tính: 0", 
                Location = new Point(170, 56), 
                AutoSize = true
            };

            pnlDatPhong.Controls.AddRange(new Control[] { btnXoaPhongChon, _btnDatPhong, _lblTongTien });
            
            grpPhongChon.Controls.Add(_dgvPhongChon);
            grpPhongChon.Controls.Add(pnlDatPhong);

            pnlCenter.Controls.Add(grpPhongChon);
            pnlCenter.Controls.Add(grpPhongTrong);

            Controls.Add(pnlCenter);
            Controls.Add(pnlKhachHang);
            Controls.Add(pnlFilter);

            // --- Áp theme chung ---
            AppTheme.ApplyLabel(lblNgayNhan);
            AppTheme.ApplyLabel(lblNgayTra);
            AppTheme.ApplyLabel(lblLoaiPhong);
            AppTheme.ApplyLabel(lblSoKhach);
            AppTheme.ApplyLabel(lblTimKH);
            AppTheme.ApplyLabel(_lblThongTinKhachHang);
            AppTheme.ApplyLabel(_lblTongTien);
            _lblTongTien.Font = AppTheme.FontSubHeader;
            _lblTongTien.ForeColor = AppTheme.Primary;

            AppTheme.ApplyDateTimePicker(_dtpNgayNhan);
            AppTheme.ApplyDateTimePicker(_dtpNgayTra);
            AppTheme.ApplyComboBox(_cboLoaiPhong);
            AppTheme.ApplyNumericUpDown(_numSoKhach);
            AppTheme.ApplyTextBox(_txtTimKhachHang);
            AppTheme.ApplyDataGridView(_dgvPhongTrong);
            AppTheme.ApplyDataGridView(_dgvPhongChon);

            AppTheme.ApplyPrimaryButton(_btnTimPhong);
            AppTheme.ApplyPrimaryButton(_btnDatPhong);
            AppTheme.ApplySecondaryButton(_btnTimKhachHang);
            AppTheme.ApplySecondaryButton(btnThemKH);
            AppTheme.ApplySecondaryButton(btnChonPhong);
            AppTheme.ApplySecondaryButton(btnXoaPhongChon);

            // --- Canh lại bố cục theo font mới ---
            AppTheme.AlignInputAfterLabel(lblNgayNhan, _dtpNgayNhan);
            lblNgayTra.Left = _dtpNgayNhan.Right + 40;
            AppTheme.AlignInputAfterLabel(lblNgayTra, _dtpNgayTra);
            lblLoaiPhong.Left = _dtpNgayTra.Right + 40;
            AppTheme.AlignInputAfterLabel(lblLoaiPhong, _cboLoaiPhong);
            lblSoKhach.Left = _cboLoaiPhong.Right + 40;
            AppTheme.AlignInputAfterLabel(lblSoKhach, _numSoKhach);
            _btnTimPhong.Left = _numSoKhach.Right + 20;

            _txtTimKhachHang.Top = lblTimKH.Bottom + 6;
            _btnTimKhachHang.Top = _txtTimKhachHang.Top;
            _lblThongTinKhachHang.Top = _txtTimKhachHang.Bottom + 12;
            btnThemKH.Top = _lblThongTinKhachHang.Top + 80;
        }

        #endregion
    }
}
