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

            Text = "Đặt phòng";
            Size = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;

            // --- Pannel Filter ---
            var pnlFilter = new GroupBox { Text = "Tìm phòng trống", Dock = DockStyle.Top, Height = 80 };
            
            var lblNgayNhan = new Label { Text = "Ngày nhận:", Location = new Point(20, 30), AutoSize = true };
            _dtpNgayNhan = new DateTimePicker { Location = new Point(100, 27), Width = 120, Format = DateTimePickerFormat.Short };
            
            var lblNgayTra = new Label { Text = "Ngày trả:", Location = new Point(240, 30), AutoSize = true };
            _dtpNgayTra = new DateTimePicker { Location = new Point(310, 27), Width = 120, Format = DateTimePickerFormat.Short };
            _dtpNgayTra.Value = DateTime.Now.AddDays(1);

            var lblLoaiPhong = new Label { Text = "Loại phòng:", Location = new Point(450, 30), AutoSize = true };
            _cboLoaiPhong = new ComboBox { Location = new Point(530, 27), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            
            var lblSoKhach = new Label { Text = "Số khách:", Location = new Point(700, 30), AutoSize = true };
            _numSoKhach = new NumericUpDown { Location = new Point(770, 27), Width = 60, Minimum = 1, Maximum = 50, Value = 1 };
            
            _btnTimPhong = new Button { Text = "Tìm phòng", Location = new Point(850, 25), Width = 100, Height = 30 };
            _btnTimPhong.Click += async (_, _) => await TimPhongAsync();

            pnlFilter.Controls.AddRange(new Control[] {
                lblNgayNhan, _dtpNgayNhan, lblNgayTra, _dtpNgayTra,
                lblLoaiPhong, _cboLoaiPhong, lblSoKhach, _numSoKhach, _btnTimPhong
            });

            // --- Panel Khách Hàng ---
            var pnlKhachHang = new GroupBox { Text = "Khách hàng", Dock = DockStyle.Left, Width = 300 };
            
            var lblTimKH = new Label { Text = "Nhập CCCD / SĐT:", Location = new Point(10, 30), AutoSize = true };
            _txtTimKhachHang = new TextBox { Location = new Point(10, 50), Width = 180 };
            _btnTimKhachHang = new Button { Text = "Tra cứu", Location = new Point(200, 48), Width = 80, Height = 25 };
            _btnTimKhachHang.Click += async (_, _) => await TimKhachHangAsync();

            _lblThongTinKhachHang = new Label 
            { 
                Text = "(Chưa chọn khách hàng)", 
                Location = new Point(10, 90), 
                AutoSize = true, 
                MaximumSize = new Size(280, 0)
            };

            var btnThemKH = new Button { Text = "Khách hàng mới...", Location = new Point(10, 160), Width = 130, Height = 30 };
            btnThemKH.Click += (_, _) =>
            {
                var frmKH = new FormKhachHang();
                frmKH.ShowDialog(this);
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
            
            var pnlDatPhong = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var btnXoaPhongChon = new Button { Text = "Bỏ chọn ↑", Location = new Point(20, 15), Width = 100, Height = 30 };
            btnXoaPhongChon.Click += BtnXoaPhongChon_Click;

            _lblTongTien = new Label { Text = "Tổng tiền tạm tính: 0", Location = new Point(300, 20), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            
            _btnDatPhong = new Button { Text = "ĐẶT PHÒNG", Location = new Point(620, 10), Width = 120, Height = 40, BackColor = Color.LightGreen };
            _btnDatPhong.Click += async (_, _) => await DatPhongAsync();

            pnlDatPhong.Controls.AddRange(new Control[] { btnXoaPhongChon, _lblTongTien, _btnDatPhong });
            
            grpPhongChon.Controls.Add(_dgvPhongChon);
            grpPhongChon.Controls.Add(pnlDatPhong);

            pnlCenter.Controls.Add(grpPhongChon);
            pnlCenter.Controls.Add(grpPhongTrong);

            Controls.Add(pnlCenter);
            Controls.Add(pnlKhachHang);
            Controls.Add(pnlFilter);
        }

        #endregion
    }
}
